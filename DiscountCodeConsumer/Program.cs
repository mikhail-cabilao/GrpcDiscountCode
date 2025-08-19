using DiscountCodes;
using Grpc.Net.Client;

namespace DiscountCodeConsumer
{
    internal class Program
    {
        static async Task Main()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7039");
            var client = new DiscountCodes.DiscountCodes.DiscountCodesClient(channel);
            var reply = await client.GenerateAsync(new GenerateRequest { Count = 10, Length = 8 });
        }
    }
}
