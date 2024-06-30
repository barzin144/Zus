using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;
using Zus.Cli.Helpers;
using Zus.Cli.Models;

namespace Zus.Cli.Commands;

internal static partial class SendRequest
{
    internal static async Task<CommandResult> Delete(string name)
    {
        var retypedName = Display.ConfirmMessage("Retype the name to confirm: ");
        if (retypedName == name)
        {
            await Helper.DeleteRequest(name);
            return new CommandResult
            {
                Result = $"Request {name} has been deleted."
            };
        }
        else
        {
            return new CommandResult
            {
                Error = "The names do not match, the request has not been deleted."
            };

        }
    }
    internal static async Task<CommandResult> List()
    {
        string requests = await Helper.AllRequestFromFileString();
        return new CommandResult
        {
            Result = requests
        };
    }

    internal static async Task<CommandResult> ResendAsync(string name)
    {
        try
        {
            HttpResponseMessage result = await ResendRequestAsync(name);
            return new CommandResult
            {
                Result = await result.BeautifyHttpResponse()
            };
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }

    internal static async Task<CommandResult> SendAsync(Request request, string? name, bool force)
    {
        try
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                request.Name = name;
                await Helper.SaveRequestToFile(request, force);
            }

            HttpResponseMessage result = await SendRequestAsync(request);
            return new CommandResult { Result = await result.BeautifyHttpResponse() };
        }
        catch (TaskCanceledException)
        {
            return new CommandResult { Error = $"Error: The request to '{request.Url}' has timed out and cannot be completed. Please check your connection and try again later." };
        }
        catch (HttpRequestException)
        {
            return new CommandResult { Error = $"Error: Please check your connection and try again later." };
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }

    private static async Task<string> ReplacePreRequestVariables(string data, HttpResponseMessage preRequestResponse)
    {
        if (string.IsNullOrEmpty(data) == false)
        {
            var variables = FindPreRequestVariable(data);
            if (variables.Count != 0)
            {
                StringBuilder stringBuilder = new(data);
                foreach (var variable in variables)
                {
                    var responseValue = await preRequestResponse.GetPropertyValue(variable);
                    stringBuilder = stringBuilder.Replace($"{{pr.{variable}}}", responseValue);
                }

                return stringBuilder.ToString();
            }
        }
        return data;
    }

    private static List<string> FindPreRequestVariable(string text)
    {
        return PreRequestVariableRegex().Matches(text).Select(x => x.Groups["PR"].Value).ToList();
    }

    private static async Task<HttpResponseMessage> ResendRequestAsync(string name)
    {

        Request? request = await Helper.ReadRequestFromFile(name);

        if (request == null)
        {
            throw new Exception($"Error: The request with the name '{name}' was not found. Please ensure the request name is correct and try again.");
        }

        return await SendRequestAsync(request);
    }

    private static async Task<HttpResponseMessage> SendRequestAsync(Request request)
    {
        if (string.IsNullOrEmpty(request.PreRequest) == false)
        {
            HttpResponseMessage preRequestResponse = await ResendRequestAsync(request.PreRequest);
            request.Data = await ReplacePreRequestVariables(request.Data, preRequestResponse);
            request.Auth = await ReplacePreRequestVariables(request.Auth!, preRequestResponse);
        }

        using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(5) };

        if (string.IsNullOrEmpty(request.Auth) == false)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Auth}");
        }
        if (string.IsNullOrEmpty(request.Data) == false)
        {
            if (request.FormFormat.HasValue && request.FormFormat == true)
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            }
            else
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        try
        {
            switch (request.RequestMethod)
            {
                case RequestMethod.Get:
                    return await GetAsync(client, request.Url);
                case RequestMethod.Post:
                    return await PostAsync(client, request.Url, request.Data, request.FormFormat ?? false);
                default:
                    throw new Exception("Request method is not valid.");
            }
        }
        catch
        {
            throw;
        }
    }

    private static async Task<HttpResponseMessage> GetAsync(HttpClient client, string url)
    {
        return await client.GetAsync(url);
    }

    private static async Task<HttpResponseMessage> PostAsync(HttpClient client, string url, string data, bool form)
    {
        if (form)
        {
            return await client.PostAsync(url, data.ToFormUrlEncodedContent());
        }
        else
        {
            return await client.PostAsync(url, data.ToJsonStringContent());
        }
    }

    [GeneratedRegex(@"\{pr\.(?<PR>\w+)\}", RegexOptions.Compiled)]
    private static partial Regex PreRequestVariableRegex();

}
