using System.Security.Cryptography;
using System.Text;

namespace ApiTCC.Utils
{
    public class SessaoHelper
    {
        public static string GerarRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public static string Hash(string token)
            => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
