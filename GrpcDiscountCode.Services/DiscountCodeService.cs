using GrpcDiscountCode.Data.Models;
using GrpcDiscountCode.Data.Repositories;
using GrpcDiscountCode.Services.Enums;
using GrpcDiscountCode.Services.Helpers;

namespace GrpcDiscountCode.Services
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private const int MaxPerRequest = 2000;
        private const int MaxChunkInsert = 500;
        private const int MaxRetries = 6;

        private readonly IDiscountCodeRepository _repository;

        public DiscountCodeService(IDiscountCodeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> GenerateAsync(uint requestedCount, byte? fixedLength = null, CancellationToken ct = default)
        {
            if (requestedCount <= 0) throw new ArgumentOutOfRangeException(nameof(requestedCount));
            if (requestedCount > MaxPerRequest) throw new ArgumentOutOfRangeException(nameof(requestedCount), $"Maximum per request is {MaxPerRequest}.");
            if (fixedLength < 7 || fixedLength > 8) throw new ArgumentOutOfRangeException(nameof(fixedLength));

            var created = new List<string>();
            int remaining = (int)requestedCount;
            int attempt = 0;

            try
            {
                while (remaining > 0)
                {
                    ct.ThrowIfCancellationRequested();
                    attempt++;

                    int candidateCount = (int)Math.Ceiling(remaining * 1.2);
                    var candidates = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var code in DiscountCodeGeneratorHelper.GenerateBatch(candidateCount, fixedLength))
                    {
                        candidates.Add(code);
                        if (candidates.Count >= candidateCount) break;
                    }

                    var existing = await _repository.
                        GetDiscountCodesAsync(x => candidates.Contains(x.Code), x => x.Code);

                    candidates.ExceptWith(existing);

                    var toInsertNow = candidates.Take(Math.Min(remaining, candidates.Count)).ToList();

                    int idx = 0;
                    while (idx < toInsertNow.Count)
                    {
                        var chunk = toInsertNow.Skip(idx).Take(MaxChunkInsert)
                            .Select(c => new DiscountCode { Code = c, CreatedDate = DateTime.UtcNow })
                            .ToList();

                        if (chunk.Count == 0) break;

                        var (success, duplicates) = await _repository.AddRangeTransactAsync(chunk);

                        if (!success)
                        {
                            toInsertNow = toInsertNow.Except(duplicates).ToList();
                        }

                        idx += MaxChunkInsert;
                        remaining -= chunk.Count;
                        created.AddRange(chunk.Select(c => c.Code));
                    }

                    if (attempt > MaxRetries && remaining > 0)
                        throw new InvalidOperationException($"Could not generate the requested number of unique codes after {MaxRetries} retries. Created {created.Count}.");
                }

                return true;
            }
            catch(InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<byte> UseCodeAsync(string code, CancellationToken ct = default)
        {
            if (code.Length < 7 || code.Length > 8) throw new ArgumentOutOfRangeException(nameof(code.Length));

            code = code.Trim();

            var codeResult = await _repository.GetDiscountCodesAsync(x => x.Code == code, x => x, ct);

            if (codeResult is null) return (byte)UseCodeStatus.Invalid;

            await _repository.DeleteCodeAsync(codeResult.First());

            return (byte)UseCodeStatus.Success;
        }
    }
}
