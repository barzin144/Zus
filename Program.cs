using Cocona;
using Zus.Commands;

var app = CoconaApp.Create();

app.AddCommand("base64", ([Argument] string data) => Console.WriteLine(Base64.Encode(data)))
    .WithDescription("Return encoded base64 of input");

app.AddCommand("dbase64", ([Argument] string data) => Console.WriteLine(Base64.Decode(data)))
    .WithDescription("Return decoded base64 of input");

app.AddCommand("sha256", ([Argument] string data, [Argument] string? secret) => Console.WriteLine(Sha256.Hash(data, secret)))
    .WithDescription("Hash the input with SHA256 algorithm");

app.AddCommand("djwt", ([Argument] string data, [Argument] string? secret) => Console.WriteLine(string.Join("\n", Jwt.Decode(data, secret))))
    .WithDescription("Return decoded JWT token");

app.Run();