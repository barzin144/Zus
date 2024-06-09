using Zus.Models;
using Zus.Helpers;
using System.Net.Http.Headers;
using System.Text;

namespace Zus.Commands
{
    internal static class SendRequest
    {
        internal static async Task<string> Resend(string name)
        {
            Request? request = await Helper.ReadRequestFromFile(name);

            if (request == null)
            {
                return $"Error: The request with the name '{name}' was not found. Please ensure the request name is correct and try again.";
            }

            return await SendAsync(request, null, false);
        }
        internal static async Task<string> SendAsync(Request request, string? name, bool force)
        {
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };

            if (string.IsNullOrEmpty(request.Auth) == false)
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Auth}");
            }
            if (string.IsNullOrEmpty(request.Data) == false)
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            if (string.IsNullOrEmpty(name) == false)
            {
                request.Name = name;
                await Helper.SaveRequestToFile(request, force);
            }

            try
            {
                switch (request.RequestMethod)
                {
                    case RequestMethod.Get:
                        return await GetAsync(client, request.Url);
                    case RequestMethod.Post:
                        return await PostAsync(client, request.Url, request.Data);
                    default:
                        return "";
                }
            }
            catch (TaskCanceledException)
            {
                return $"Error: The request to '{request.Url}' has timed out and cannot be completed. Please check your connection and try again later.";
            }
            catch (HttpRequestException)
            {
                return $"Error: Please check your connection and try again later.";
            }

        }

        private static async Task<string> GetAsync(HttpClient client, string url)
        {
            HttpResponseMessage result = await client.GetAsync(url);
            return await result.BeautifyHttpResponse();
        }

        private static async Task<string> PostAsync(HttpClient client, string url, string data)
        {
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync(url, content);
            return await result.BeautifyHttpResponse();
        }
    }
}
