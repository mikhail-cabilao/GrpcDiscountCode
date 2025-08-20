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

            Console.WriteLine("============ Generate Code ============");
            Console.WriteLine();

            var count = TryInputValue("The number of codes");
            var length = TryInputValue("Length of the code");

            var gr = await client.GenerateAsync(new GenerateRequest { Count = count, Length = length });

            Console.WriteLine("Generated code {0}.", gr.Result ? "successully" : "failed");

            Console.WriteLine();
            Console.WriteLine("============ Use Code ============");

            Console.Write("Discount code:");
            var code = Console.ReadLine();
            var cr = await client.UseCodeAsync(new UseCodeRequest { Code = code });

            Console.WriteLine("Code redeemed {0}.", cr.Result == 0 ? "successully" : "failed");
            Console.ReadKey();
        }

        static uint TryInputValue(string inputMessage)
        {
            Console.Write("Enter {0}:", inputMessage);
            var countStr = Console.ReadLine();

            if (!uint.TryParse(countStr, out var result))
            {
                Console.WriteLine("Invalid valute, try again!!");
                Console.WriteLine();
                TryInputValue(inputMessage);
            }

            return result;
        }
    }
}
