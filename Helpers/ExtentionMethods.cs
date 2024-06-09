using System.Text.Json.Nodes;
using System.Text.Json;
using System.Net.Http.Json;

namespace Zus.Helpers
{
    internal static class ExtentionMethods
    {
        private readonly static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

        internal static string BeautifyJson(this string data)
        {
            JsonNode? json = JsonNode.Parse(data);
            return json!.ToJsonString(_jsonSerializerOptions);
        }
        internal static async Task<string> BeautifyHttpResponse(this HttpResponseMessage responseMessage)
        {
            try
            {
                var response = new
                {
                    responseMessage.StatusCode,
                    Content = await responseMessage.Content.ReadFromJsonAsync<object>()
                };

                return JsonSerializer.Serialize(response, _jsonSerializerOptions);
            }
            catch
            {
                return await responseMessage.Content.ReadAsStringAsync();
            }
        }
    }
}
