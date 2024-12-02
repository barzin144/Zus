using System.Text.Json.Serialization;
using Zus.Cli.Services;

namespace Zus.Cli.Models;

public enum RequestMethod
{
    Get,
    Post
}
public class Request : IData
{
    public Request(string url, string? auth, RequestMethod requestMethod, string data = "", string? header = "", bool? formFormat = false, bool? jsonFormat = false, string? preRequest = "")
    {
        Url = url;
        Auth = auth;
        RequestMethod = requestMethod;
        Header = header;
        Data = data;
        FormFormat = formFormat;
        JsonFormat = jsonFormat;
        PreRequest = preRequest;
    }
    public string Url { get; set; }
    public string? Auth { get; set; }
    public string Data { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter<RequestMethod>))]
    public RequestMethod RequestMethod { get; }
    public string? Header { get; }
    public string? Name { get; set; }
    public bool? FormFormat { get; }
    public bool? JsonFormat { get; }
    public string? PreRequest { get; }
    public string Id { get => Name ?? string.Empty; set => Name = value; }
}
