using DiscountCodes;
using Grpc.Core;

namespace GrpcDiscountCode.Services
{
    public class DiscountGrpcService : DiscountCodes.DiscountCodes.DiscountCodesBase
    {
        private readonly IDiscountCodeService _svc;
        public DiscountGrpcService(IDiscountCodeService svc) => _svc = svc;

        public override async Task<GenerateResponse> Generate(GenerateRequest request, ServerCallContext context)
        {
            var ct = context.CancellationToken;
            byte? length = request.Length == 0 ? null : (byte)request.Length;
            var codes = await _svc.GenerateAsync(request.Count, length, ct);
            var response = new GenerateResponse { };
            return response;
        }
    }
}
