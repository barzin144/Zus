using System.Net.Http.Headers;

namespace Zus.Cli.Services;

public interface IHttpHandler
{
    void AddHeader(string name, string value);
    void AddHeader(MediaTypeWithQualityHeaderValue mediaTypeWithQualityHeaderValue);
    Task<HttpResponseMessage> GetAsync(string requestUri);
    Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    void Dispose();
}

public class HttpHandler : IHttpHandler
{
    private readonly HttpClient _httpClient;
    public HttpHandler(TimeSpan timeout)
    {
        _httpClient = new HttpClient() { Timeout = timeout };
    }

    public void AddHeader(string name, string value)
    {
        _httpClient.DefaultRequestHeaders.Add(name, value);
    }

    public void AddHeader(MediaTypeWithQualityHeaderValue mediaTypeWithQualityHeaderValue)
    {
        _httpClient.DefaultRequestHeaders.Accept.Add(mediaTypeWithQualityHeaderValue);
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        return await _httpClient.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
    {
        return await _httpClient.PostAsync(requestUri, content);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
