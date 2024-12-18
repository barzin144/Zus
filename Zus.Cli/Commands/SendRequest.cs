﻿using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;
using Zus.Cli.Helpers;
using Zus.Cli.Models;
using Zus.Cli.Services;
using System.Data;
using System.Text.Json;

namespace Zus.Cli.Commands;

internal partial class SendRequest : IDisposable
{
	private readonly IFileService<Request> _fileService;
	private readonly IHttpHandler _httpHandler;
	private readonly IVariablesService _variablesService;
	private readonly IFileService<Response> _responsesService;

	internal SendRequest(IFileService<Request> fileService, IHttpHandler httpHandler, IVariablesService variablesService, IFileService<Response> responsesService)
	{
		_fileService = fileService;
		_httpHandler = httpHandler;
		_variablesService = variablesService;
		_responsesService = responsesService;
	}
	internal async Task<CommandResult> DeleteAsync(string name)
	{
		var retypedName = Display.ConfirmMessage("Retype the name to confirm: ");
		if (retypedName == name)
		{
			await _fileService.DeleteAsync(name);
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

	internal async Task<CommandResult> ListAsync()
	{
		string requests = await _fileService.GetAsync();
		return new CommandResult
		{
			Result = requests
		};
	}

	internal async Task<CommandResult> ResendAsync(string name, bool saveResponse)
	{
		try
		{
			Request request = await GetRequestByName(name);
			HttpResponseMessage result = await SendRequestAsync(request);
			string beautifyHttpResponse = await result.BeautifyHttpResponse();
			if (saveResponse)
			{
				await SaveResponse(request, beautifyHttpResponse);
			}
			return new CommandResult
			{
				Result = beautifyHttpResponse
			};
		}
		catch (KeyNotFoundException ex)
		{
			return new CommandResult { Error = ex.Message };
		}
	}

	internal async Task<CommandResult> SendAsync(Request request, string? name, bool force, bool saveResponse)
	{
		try
		{
			if (string.IsNullOrEmpty(name) == false)
			{
				request.Name = name;
				await _fileService.SaveAsync(request, force);
			}

			HttpResponseMessage result = await SendRequestAsync(request);
			string beautifyHttpResponse = await result.BeautifyHttpResponse();
			if (saveResponse)
			{
				await SaveResponse(request, beautifyHttpResponse);
			}
			return new CommandResult { Result = beautifyHttpResponse };
		}
		catch (TaskCanceledException)
		{
			return new CommandResult { Error = $"Error: The request to '{request.Url}' has timed out and cannot be completed. Please check your connection and try again later." };
		}
		catch (HttpRequestException)
		{
			return new CommandResult { Error = $"Error: Please check your connection and try again later." };
		}
		catch (DuplicateNameException)
		{
			return new CommandResult { Error = $"Error: A request with the name '{name}' already exists. To overwrite the existing request, please use the '-f' flag" };
		}
		catch (Exception ex)
		{
			return new CommandResult { Error = ex.Message };
		}
	}

	private async Task SaveResponse(Request request, string httpResponse)
	{
		Response response = new(JsonSerializer.Deserialize<object>(httpResponse) ?? new { }, request.Url, request.Data);
		await _responsesService.SaveAsync(response, false);
	}

	private string ReplacePreRequestVariables(string data, JsonElement preRequestResponse)
	{
		if (string.IsNullOrEmpty(data) == false)
		{
			var variables = FindPreRequestVariable(data);
			if (variables.Count != 0)
			{
				StringBuilder stringBuilder = new(data);
				foreach (var variable in variables)
				{
					var responseValue = variable == "$" ? preRequestResponse.GetPropertyValue("Content") : preRequestResponse.GetPropertyValue(variable);
					stringBuilder = stringBuilder.Replace($"{{pr.{variable}}}", responseValue);
				}

				return stringBuilder.ToString();
			}
		}
		return data;
	}

	private string ReplaceVariables(string data, List<LocalVariable> savedVariables)
	{
		if (string.IsNullOrEmpty(data) == false)
		{
			var variables = FindVariable(data);
			if (variables.Count != 0)
			{
				StringBuilder stringBuilder = new(data);
				foreach (var variable in variables)
				{
					var value = savedVariables.FirstOrDefault(x => x.Name == variable)?.Value;
					if (value == null)
					{
						throw new Exception($"{variable} variable not found.");
					}
					stringBuilder = stringBuilder.Replace($"{{var.{variable}}}", value);
				}

				return stringBuilder.ToString();
			}
		}
		return data;
	}

	private List<string> FindPreRequestVariable(string text)
	{
		return PreRequestVariableRegex().Matches(text).Select(x => x.Groups["PR"].Value).ToList();
	}

	private List<string> FindVariable(string text)
	{
		return VariableRegex().Matches(text).Select(x => x.Groups["VAR"].Value).ToList();
	}

	private async Task<Request> GetRequestByName(string name)
	{

		Request? request = await _fileService.GetAsync(name);

		if (request == null)
		{
			throw new KeyNotFoundException($"Error: The request with the name '{name}' was not found. Please ensure the request name is correct and try again.");
		}

		return request;
	}

	private async Task<HttpResponseMessage> ResendRequestAsync(string name)
	{

		Request request = await GetRequestByName(name);
		return await SendRequestAsync(request);
	}

	private async Task<HttpResponseMessage> SendRequestAsync(Request request)
	{
		List<LocalVariable> variables = await _variablesService.GetDeserializeAsync();
		request.Url = ReplaceVariables(request.Url, variables);
		request.Data = ReplaceVariables(request.Data, variables);

		if (string.IsNullOrEmpty(request.PreRequest) == false)
		{
			HttpResponseMessage preRequestResponse = await ResendRequestAsync(request.PreRequest);
			try
			{
				JsonElement response = await preRequestResponse.ToJsonElement();
				request.Data = ReplacePreRequestVariables(request.Data, response);
				request.Auth = ReplacePreRequestVariables(request.Auth!, response);
			}
			catch (KeyNotFoundException)
			{
				throw;
			}
			catch (Exception)
			{
				throw new InvalidDataException("Pre-request response is not valid;");
			}
		}

		if (string.IsNullOrEmpty(request.Auth) == false)
		{
			_httpHandler.AddHeader("Authorization", $"Bearer {request.Auth}");
		}

		if (string.IsNullOrEmpty(request.Header) == false)
		{
			foreach (var header in ExtensionMethods.ConvertStringDataToDictionary(request.Header))
			{
				_httpHandler.AddHeader(header.Key, header.Value);
			}
		}

		if (string.IsNullOrEmpty(request.Data) == false)
		{
			if (request.FormFormat.HasValue && request.FormFormat == true)
			{
				_httpHandler.AddHeader(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
			}
			else
			{
				_httpHandler.AddHeader(new MediaTypeWithQualityHeaderValue("application/json"));
			}
		}

		try
		{
			switch (request.RequestMethod)
			{
				case RequestMethod.Get:
					return await GetAsync(request.Url);
				case RequestMethod.Post:
					return await PostAsync(request.Url, request.Data, request.FormFormat ?? false, request.JsonFormat ?? false);
				default:
					throw new Exception("Request method is not valid.");
			}
		}
		catch
		{
			throw;
		}
	}

	private async Task<HttpResponseMessage> GetAsync(string url)
	{
		return await _httpHandler.GetAsync(url);
	}

	private async Task<HttpResponseMessage> PostAsync(string url, string data, bool form, bool json)
	{
		if (form)
		{
			return await _httpHandler.PostAsync(url, data.ToFormUrlEncodedContent());
		}
		else if (json)
		{
			return await _httpHandler.PostAsync(url, data.ToJsonStringContent());
		}
		else
		{
			return await _httpHandler.PostAsync(url, data.ToStringContent());
		}
	}

	[GeneratedRegex(@"\{pr\.(?<PR>(\w+|\$))\}", RegexOptions.Compiled)]
	private partial Regex PreRequestVariableRegex();

	[GeneratedRegex(@"\{var\.(?<VAR>((\w+[a-z-_.]+)|\$))\}", RegexOptions.Compiled)]
	private partial Regex VariableRegex();

	public void Dispose()
	{
		_httpHandler.Dispose();
	}
}
