using Zus.Models;
using Zus.Helpers;

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

            return await Send(request, null, false);
        }
        internal static async Task<string> Send(Request request, string? name, bool force)
        {
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };

            if (string.IsNullOrEmpty(request.Auth) == false)
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Auth}");
            }

            if (string.IsNullOrEmpty(name) == false)
            {
                request.Name = name;
                await Helper.SaveRequestToFile(request, force);
            }

            switch (request.RequestMethod)
            {
                case RequestMethod.Get:
                    return await Get(client, request.Url);
                case RequestMethod.Post:
                    return "";
                default:
                    return "";
            }
        }
        private static async Task<string> Get(HttpClient client, string url)
        {
            try
            {
                HttpResponseMessage result = await client.GetAsync(url);
                return await result.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                return $"Error: The request to '{url}' has timed out and cannot be completed. Please check your connection and try again later.";
            }
        }
    }
}
