
namespace GrpcDiscountCode.Services
{
    public interface IDiscountCodeService
    {
        Task<IList<string>> GenerateAsync(uint requestedCount, byte? fixedLength = null, CancellationToken ct = default);
    }
}