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
            var result = await _svc.GenerateAsync(request.Count, length, ct);
            var response = new GenerateResponse { Result = result };
            return response;
        }

        public override async Task<UseCodeResponse> UseCode(UseCodeRequest request, ServerCallContext context)
        {
            var ct = context.CancellationToken;
            var result = await _svc.UseCodeAsync(request.Code, ct);
            var response = new UseCodeResponse { Result = result };
            return response;
        }
    }
}
