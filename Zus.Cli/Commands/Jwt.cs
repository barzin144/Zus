using Zus.Helpers;
using Zus.Models;

namespace Zus.Commands
{
    internal static class Jwt
    {
        internal static CommandResult Decode(string data, string? signature)
        {
            List<string> result = new(3);
            try
            {
                string[] jwt = data.Split('.');
                string header = jwt[0];
                string payload = jwt[1];
                string hashedSignature = jwt[2];
                string secret = signature ?? string.Empty;
                CommandResult base64Hashed = Sha256.Hash($"{header}.{payload}", secret);
                CommandResult decodedHeader = Base64.Decode(header);
                CommandResult decodedPayload = Base64.Decode(payload);

                if (decodedHeader.Success && decodedPayload.Success)
                {
                    result.Add(decodedHeader.Result?.BeautifyJson()!);
                    result.Add(decodedPayload.Result?.BeautifyJson()!);
                }
                else
                {
                    return new CommandResult { Error = "Invalid inputs" };
                }

                if (base64Hashed.Success && base64Hashed.Result?.Replace("=", "").Replace("+", "-") == hashedSignature)
                {
                    result.Add("Signature Verified");
                }
                else
                {
                    result.Add("Invalid Signature");
                }
            }
            catch
            {
                return new CommandResult { Error = "Invalid inputs" };
            }

            return new CommandResult { Result = string.Join("\n", result) };
        }
    }
}
