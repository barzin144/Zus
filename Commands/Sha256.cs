using System.Security.Cryptography;
using System.Text;

namespace Zus.Commands
{
    internal static class Sha256
    {
        internal static string Hash(string data, string? secret)
        {
            using HMACSHA256 hmac = string.IsNullOrEmpty(secret) ? new HMACSHA256() : new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            byte[] dataByte = Encoding.UTF8.GetBytes(data);
            byte[] hashedData = hmac.ComputeHash(dataByte);

            return Convert.ToBase64String(hashedData);
        }
    }
}
