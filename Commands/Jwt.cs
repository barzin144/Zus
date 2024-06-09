using Zus.Helpers;

namespace Zus.Commands
{
    internal static class Jwt
    {
        internal static List<string> Decode(string data, string? signature)
        {
            List<string> result = new(3);
            try
            {
                string[] jwt = data.Split('.');
                string header = jwt[0];
                string payload = jwt[1];
                string hashedSignature = jwt[2];
                var secret = signature ?? string.Empty;
                var base64Hashed = Sha256.Hash($"{header}.{payload}", secret).Replace("=", "").Replace("+", "-");

                result.Add(Base64.Decode(header).BeautifyJson());
                result.Add(Base64.Decode(payload).BeautifyJson());

                if (base64Hashed == hashedSignature)
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
                throw new Exception("Invalid inputs");
            }

            return result;
        }
    }
}
