using System.Text.Json.Nodes;
using System.Text.Json;
using System.Net.Http.Json;
using System.Text;

namespace Zus.Cli.Helpers;

internal static class ExtensionMethods
{
    private readonly static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    internal static string BeautifyJson(this string data)
    {
        JsonNode? json = JsonNode.Parse(data);
        return json!.ToJsonString(_jsonSerializerOptions);
    }

    internal static string GetPropertyValue(this JsonElement jsonElement, string propertyName)
    {
        try
        {
            return jsonElement.GetProperty(propertyName).ToString();
        }
        catch
        {
            throw new KeyNotFoundException($"{propertyName} is not found in response.");
        }
    }

    internal static async Task<string> BeautifyHttpResponse(this HttpResponseMessage responseMessage)
    {
        try
        {
            var response = new
            {
                Status = responseMessage.StatusCode.ToString(),
                Content = await responseMessage.Content.ReadFromJsonAsync<object>()
            };

            return JsonSerializer.Serialize(response, _jsonSerializerOptions);
        }
        catch
        {
            var response = new
            {
                Status = responseMessage.StatusCode.ToString(),
                Content = await responseMessage.Content.ReadAsStringAsync()
            };

            return JsonSerializer.Serialize(response, _jsonSerializerOptions);
        }
    }

    internal static async Task<JsonElement> ToJsonElement(this HttpResponseMessage responseMessage)
    {
        try
        {
            return await responseMessage.Content.ReadFromJsonAsync<JsonElement>();
        }
        catch
        {
            string content = await responseMessage.Content.ReadAsStringAsync();
            string response = JsonSerializer.Serialize(new { Content = content });

            return JsonSerializer.Deserialize<JsonElement>(response);
        }
    }

    internal static StringContent ToJsonStringContent(this string data)
    {
        var dataDic = ConvertStringDataToDictionary(data);
        return new StringContent(JsonSerializer.Serialize(dataDic), Encoding.UTF8, "application/json");
    }

    internal static FormUrlEncodedContent ToFormUrlEncodedContent(this string data)
    {
        var dataDic = ConvertStringDataToDictionary(data);

        return new FormUrlEncodedContent(dataDic);
    }

    internal static Dictionary<string, string> ConvertStringDataToDictionary(string data)
    {
        Dictionary<string, string> dataDic = [];

        string[] keyValueList = data.Split(',');
        foreach (var keyValue in keyValueList)
        {
            string[] separatedKeyValue = keyValue.Split(':');
            dataDic.Add(separatedKeyValue[0], separatedKeyValue[1]);
        }

        return dataDic;
    }
}
