using GrpcDiscountCode.Data.Models;
using System.Linq.Expressions;

namespace GrpcDiscountCode.Data.Repositories
{
    public interface IDiscountCodeRepository
    {
        Task<(bool, List<string>)> AddRangeTransactAsync(List<DiscountCode> discountCodes, CancellationToken ct = default);
        Task<int> DeleteCodeAsync(DiscountCode discount, CancellationToken ct = default);
        Task<IEnumerable<T>> GetDiscountCodesAsync<T>(Expression<Func<DiscountCode, bool>> predicate, Expression<Func<DiscountCode, T>> selector, CancellationToken ct = default);
    }
}