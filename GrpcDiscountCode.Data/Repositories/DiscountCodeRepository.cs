using GrpcDiscountCode.Data.Extensions;
using GrpcDiscountCode.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq.Expressions;

namespace GrpcDiscountCode.Data.Repositories
{
    public class DiscountCodeRepository(
        DiscountContext discountContext,
        ILogger<DiscountCodeRepository> logger) : IDiscountCodeRepository
    {
        private readonly DiscountContext _discountContext = discountContext;
        private readonly ILogger<DiscountCodeRepository> _logger = logger;

        public async Task<IEnumerable<T>> GetDiscountCodesAsync<T>(Expression<Func<DiscountCode, bool>> predicate, Expression<Func<DiscountCode, T>> selector, CancellationToken ct = default)
        {
            return await _discountContext.DiscountCodes.Where(predicate).Select(selector).ToListAsync(ct);
        }

        public async Task<(bool, List<string>)> AddRangeTransactAsync(List<DiscountCode> discountCodes, CancellationToken ct = default)
        {
            await using var tx = await _discountContext.Database.BeginTransactionAsync(ct);
            try
            {
                await _discountContext.AddRangeAsync(discountCodes);
                await _discountContext.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return (true, []);
            }
            catch (DbUpdateException)
            {
                await tx.RollbackAsync(ct);

                var duped = await _discountContext.DiscountCodes
                    .Where(x => discountCodes.Select(c => c.Code).Contains(x.Code))
                    .Select(x => x.Code)
                    .ToListAsync(ct);

                // Backoff jitter
                await Task.Delay(TimeSpan.FromMilliseconds(50 + new Random().Next(0, 100)), ct);
                
                return (true, duped);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error: {message}", ex.Message);
                throw;
            }
        }

        // for sql server
        public async Task BulkMergeAsync(List<DiscountCode> discountCodes)
        {
            var dataTable = discountCodes.ToDataTable();

            var param = new SqlParameter("@DiscountCode", dataTable)
            {
                TypeName = "dbo.DiscountCodeType",
                SqlDbType = SqlDbType.Structured
            };

            var sql = @"
                MERGE INTO DiscountCode AS target
                USING @DiscountCode AS source
                ON target.Code = source.Code
                WHEN MATCHED THEN
                    UPDATE SET Code = source.Code, CreatedDate = source.CreatedDate
                WHEN NOT MATCHED THEN
                    INSERT (Code, CreatedDate)
                    VALUES (source.Code, source.CreatedDate);";

            await _discountContext.Database.ExecuteSqlRawAsync(sql, param);
        }
    }
}
