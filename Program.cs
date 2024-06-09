using Cocona;
using Zus.Commands;
using Zus.Models;

var app = CoconaApp.Create();

app.AddSubCommand("send", x =>
{
    x.AddCommand("get", async ([Argument] string url,
        [Option('a', Description = "Authentication Bearer Token")] string? auth,
        [Option('n', Description = "Name for saving the request")] string? name,
        [Option('f', Description = "Overwrite the existing request")] bool? force) =>
    {
        var request = new Request(url, auth, RequestMethod.Get);
        Console.WriteLine(await SendRequest.Send(request, name, force ?? false));
    })
    .WithDescription("Send a Get request");

    x.AddCommand("post", () => Console.WriteLine("Send Post"))
        .WithDescription("Send a Post request");
})
.WithDescription("Send a request.");


app.AddCommand("resend", async ([Argument] string name) => Console.WriteLine(await SendRequest.Resend(name)))
    .WithDescription("Send a saved request.");


app.AddCommand("base64", ([Argument] string data) => Console.WriteLine(Base64.Encode(data)))
    .WithDescription("Return encoded base64 of input.");

app.AddCommand("dbase64", ([Argument] string data) => Console.WriteLine(Base64.Decode(data)))
    .WithDescription("Return decoded base64 of input.");

app.AddCommand("sha256", ([Argument] string data, [Argument] string? secret) => Console.WriteLine(Sha256.Hash(data, secret)))
    .WithDescription("Hash the input with SHA256 algorithm.");

app.AddCommand("djwt", ([Argument] string data, [Argument] string? secret) => Console.WriteLine(string.Join("\n", Jwt.Decode(data, secret))))
    .WithDescription("Return decoded JWT token.");

app.Run();