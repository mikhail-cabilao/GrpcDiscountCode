
namespace GrpcDiscountCode.Services
{
    public interface IDiscountCodeService
    {
        Task<bool> GenerateAsync(uint requestedCount, byte? fixedLength = null, CancellationToken ct = default);
        Task<byte> UseCodeAsync(string code, CancellationToken ct = default);
    }
}