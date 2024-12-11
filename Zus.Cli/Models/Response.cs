using Zus.Cli.Services;

namespace Zus.Cli.Models;

public class Response : IData
{

    public Response(object result, string url, string data)
    {
        Date = DateTime.Now;
        Name = $"{url}-{Date.ToString("yyyy-MM-dd-hh-mm-ss-fff")}";
        Url = url;
        Data = data;
        Result = result;
    }
    public string Name { get; set; }
    public string Data { get; set; }
    public string Url { get; set; }
    public DateTime Date { get; set; }
    public object Result { get; set; }

    public string Id { get => Name ?? string.Empty; set => Name = value; }
}
