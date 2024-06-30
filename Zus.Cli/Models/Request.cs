using System.Text.Json.Serialization;

namespace Zus.Models
{
    public enum RequestMethod
    {
        Get,
        Post
    }
    public class Request
    {
        public Request(string url, string? auth, RequestMethod requestMethod, string data = "", bool? formFormat = false, string? preRequest = "")
        {
            Url = url;
            Auth = auth;
            RequestMethod = requestMethod;
            Data = data;
            FormFormat = formFormat;
            PreRequest = preRequest;
        }
        public string Url { get; }
        public string? Auth { get; set; }
        public string Data { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter<RequestMethod>))]
        public RequestMethod RequestMethod { get; }
        public string? Name { get; set; }
        public bool? FormFormat { get; }
        public string? PreRequest { get; }
    }
}
