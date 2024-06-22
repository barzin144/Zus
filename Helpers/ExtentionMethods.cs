using System.Text.Json.Nodes;
using System.Text.Json;
using System.Net.Http.Json;
using System.Text;
using System.Reflection;

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

        internal static async Task<string> GetProperty(this HttpResponseMessage responseMessage, string propertyName)
        {
            try
            {
                JsonElement preRequestResult = await responseMessage.Content.ReadFromJsonAsync<JsonElement>();

                return preRequestResult.GetProperty(propertyName).ToString();
            }
            catch
            {
                throw new Exception($"{propertyName} is not found in response.");
            }
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

        internal static StringContent ToJsonStringContent(this string data)
        {
            Dictionary<string, string> dataDic = [];

            string[] keyValueList = data.Split(',');
            foreach (var keyValue in keyValueList)
            {
                string[] sepratedKeyValue = keyValue.Split(':');
                dataDic.Add(sepratedKeyValue[0], sepratedKeyValue[1]);
            }

            return new StringContent(JsonSerializer.Serialize(dataDic), Encoding.UTF8, "application/json");
        }

        internal static FormUrlEncodedContent ToFormUrlEncodedContent(this string data)
        {
            Dictionary<string, string> dataDic = [];

            string[] keyValueList = data.Split(',');
            foreach (var keyValue in keyValueList)
            {
                string[] sepratedKeyValue = keyValue.Split(':');
                dataDic.Add(sepratedKeyValue[0], sepratedKeyValue[1]);
            }

            return new FormUrlEncodedContent(dataDic);
        }
    }
}
