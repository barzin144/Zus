using System.Text.Json;
using System.Text.Json.Nodes;

namespace Zus.Commands
{
    internal static class Jwt
    {
        private readonly static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
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

                result.Add(BeautifyJson(Base64.Decode(header)));
                result.Add(BeautifyJson(Base64.Decode(payload)));

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

        private static string BeautifyJson(string data)
        {
            JsonNode? json = JsonObject.Parse(data);
            return JsonSerializer.Serialize(json, jsonSerializerOptions);
        }
    }
}
