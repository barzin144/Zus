using System.Security.Cryptography;
using System.Text;
using Zus.Cli.Models;

namespace Zus.Cli.Commands;

internal static class Sha256
{
    internal static CommandResult Hash(string data, string? secret)
    {
        try
        {
            using HMACSHA256 hmac = string.IsNullOrEmpty(secret) ? new HMACSHA256(Encoding.UTF8.GetBytes(string.Empty)) : new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            byte[] dataByte = Encoding.UTF8.GetBytes(data);
            byte[] hashedData = hmac.ComputeHash(dataByte);

            return new CommandResult { Result = Convert.ToBase64String(hashedData) };
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }
}
