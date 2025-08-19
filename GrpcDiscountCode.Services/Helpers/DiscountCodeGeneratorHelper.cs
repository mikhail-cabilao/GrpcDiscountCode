using System.Security.Cryptography;

namespace GrpcDiscountCode.Services.Helpers
{
    public class DiscountCodeGeneratorHelper
    {
        private static readonly char[] Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

        public static IEnumerable<string> GenerateBatch(int count, int? fixedLength = null)
        {
            if (count <= 0) yield break;
            int length = fixedLength ?? (RandomNumberGenerator.GetInt32(0, 2) == 0 ? 7 : 8);
            if (length < 7 || length > 8) throw new ArgumentOutOfRangeException(nameof(fixedLength));

            int buffer = Math.Min(count, 4096);
            var bytes = new byte[length * buffer];
            int produced = 0;
            while (produced < count)
            {
                int toMake = Math.Min(buffer, count - produced);
                RandomNumberGenerator.Fill(bytes.AsSpan(0, length * toMake));
                for (int i = 0; i < toMake; i++)
                {
                    var span = bytes.AsSpan(i * length, length);
                    Span<char> chars = stackalloc char[length];
                    for (int j = 0; j < length; j++)
                        chars[j] = Alphabet[span[j] % Alphabet.Length];
                    yield return new string(chars);
                }
                produced += toMake;
            }
        }
    }
}
