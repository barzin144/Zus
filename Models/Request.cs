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
        public Request(string url, string? auth, RequestMethod requestMethod)
        {
            Url = url;
            Auth = auth;
            RequestMethod = requestMethod;
        }
        public string Url { get; }
        public string? Auth { get; }
        [JsonConverter(typeof(JsonStringEnumConverter<RequestMethod>))]
        public RequestMethod RequestMethod { get; }
        public string? Name { get; set; }
    }
}
