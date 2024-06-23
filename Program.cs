using Cocona;
using Zus.Commands;
using Zus.Models;
using static Zus.Helpers.Display;

var app = CoconaApp.Create();

app.AddSubCommand("send", x =>
{
    x.AddCommand("get", async ([Argument] string url,
        [Option('a', Description = "Authentication Bearer Token")] string? auth,
        [Option('n', Description = "Name for saving the request")] string? name,
        [Option('p', Description = "Pre-request name")] string? preRequest,
        [Option('f', Description = "Overwrite the existing request")] bool? force) =>
    {
        var request = new Request(url, auth, RequestMethod.Get, preRequest: preRequest);
        DisplayResult(await SendRequest.SendAsync(request, name, force ?? false));
    })
    .WithDescription("Send a Get request");

    x.AddCommand("post", async ([Argument] string url,
        [Option('d', Description = "Data format: Key:Value,Key:Value and wrap your data in double quote. Data will be sent in Json format by default. By adding -x flag change format to form-urlencoded")] string data,
        [Option('a', Description = "Authentication Bearer Token")] string? auth,
        [Option('n', Description = "Name for saving the request")] string? name,
        [Option('p', Description = "Pre-request name")] string? preRequest,
        [Option('x', Description = "Send data in form-urlencoded")] bool? formFormat,
        [Option('f', Description = "Overwrite the existing request")] bool? force) =>
    {
        var request = new Request(url, auth, RequestMethod.Post, data, formFormat ?? false, preRequest);
        DisplayResult(await SendRequest.SendAsync(request, name, force ?? false));
    })
        .WithDescription("Send a Post request");
})
.WithDescription("Send a request.");


app.AddCommand("resend", async ([Argument] string name) => DisplayResult(await SendRequest.ResendAsync(name)))
    .WithDescription("Send a saved request.");


app.AddCommand("base64", ([Argument] string data) => DisplayResult(Base64.Encode(data)))
    .WithDescription("Return encoded base64 of input.");

app.AddCommand("dbase64", ([Argument] string data) => DisplayResult(Base64.Decode(data)))
    .WithDescription("Return decoded base64 of input.");

app.AddCommand("sha256", ([Argument] string data, [Argument] string? secret) => DisplayResult(Sha256.Hash(data, secret)))
    .WithDescription("Hash the input with SHA256 algorithm.");

app.AddCommand("djwt", ([Argument] string data, [Argument] string? secret) => DisplayResult(Jwt.Decode(data, secret)))
    .WithDescription("Return decoded JWT token.");

app.Run();