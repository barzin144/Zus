﻿using Cocona;
using Zus.Cli.Commands;
using Zus.Cli.Helpers;
using Zus.Cli.Models;
using Zus.Cli.Services;

var app = CoconaApp.Create();

app.AddSubCommand("send", x =>
{
    var sendRequest = ServiceFactory.GetSendRequestService();
    x.AddCommand("get", async ([Argument] string url,
        [Option('a', Description = "Authentication Bearer Token")] string? auth,
        [Option('h', Description = "Add header to request, format: Key:Value,Key:Value and wrap your data in single quote.")] string? header,
        [Option('n', Description = "Name for saving the request")] string? name,
        [Option('s', Description = "Save response")] bool? saveResponse,
        [Option('p', Description = "Pre-request name")] string? preRequest,
        [Option('f', Description = "Overwrite the existing request")] bool? force) =>
    {
        var request = new Request(url, auth, RequestMethod.Get, header: header, preRequest: preRequest);
        Display.Result(await sendRequest.SendAsync(request, name, force ?? false, saveResponse ?? false));
    })
    .WithDescription("Send a Get request");

    x.AddCommand("post", async ([Argument] string url,
        [Option('d', Description = "Data format: Key:Value,Key:Value and wrap your data in single quote. Data will be sent in string format by default. By adding -x flag change format to form-urlencoded or -j for Json format")] string data,
        [Option('a', Description = "Authentication Bearer Token")] string? auth,
        [Option('h', Description = "Add header to request, format: Key:Value,Key:Value and wrap your data in single quote.")] string? header,
        [Option('n', Description = "Name for saving the request")] string? name,
        [Option('s', Description = "Save response")] bool? saveResponse,
        [Option('p', Description = "Pre-request name")] string? preRequest,
        [Option('x', Description = "Convert Key:Value data to form-urlencoded")] bool? formFormat,
        [Option('j', Description = "Convert Key:Value data to Json format")] bool? jsonFormat,
        [Option('f', Description = "Overwrite the existing request")] bool? force) =>
    {
        var request = new Request(url, auth, RequestMethod.Post, data, header, formFormat ?? false, jsonFormat ?? false, preRequest);
        Display.Result(await sendRequest.SendAsync(request, name, force ?? false, saveResponse ?? false));
    })
        .WithDescription("Send a Post request");
})
.WithDescription("Send a request.");

app.AddSubCommand("request", x =>
{
    var sendRequest = ServiceFactory.GetSendRequestService();
    x.AddCommand("list", async () => Display.Result(await sendRequest.ListAsync())).WithDescription("List of saved requests.");
    x.AddCommand("delete", async ([Argument] string name) => Display.Result(await sendRequest.DeleteAsync(name))).WithDescription("Delete a request.");
}
)
.WithDescription("Access to saved requests.");

app.AddSubCommand("var", x =>
{
    var manageVariables = ServiceFactory.GetManageVariables();
    x.AddCommand("list", async () => Display.Result(await manageVariables.ListAsync())).WithDescription("List of saved variables.");
    x.AddCommand("save", async ([Argument] string name, [Argument] string value, [Option('f', Description = "Overwrite the existing variable")] bool? force) =>
    {
        var variable = new LocalVariable(name, value);
        Display.Result(await manageVariables.SaveAsync(variable, force ?? false));
    }).WithDescription("Save a variable.");
    x.AddCommand("delete", async ([Argument] string name) => Display.Result(await manageVariables.DeleteAsync(name))).WithDescription("Delete a variable.");
}
)
.WithDescription("Manage variables.");

app.AddCommand("resend", async ([Argument] string name,
        [Option('s', Description = "Save response")] bool? saveResponse
) => Display.Result(await ServiceFactory.GetSendRequestService().ResendAsync(name, saveResponse ?? false)))
    .WithDescription("Send a saved request.");

app.AddCommand("base64", async ([Option('f', Description = "Read data from file")] bool? file, [Argument] string data) =>
{
    if (file.HasValue && file.Value == true)
    {
        var fileReaderService = ServiceFactory.GetFileReaderService(data);
        Display.Result(await Base64.EncodeFromFile(fileReaderService));
    }
    else
    {
        Display.Result(Base64.Encode(data));
    }
})
    .WithDescription("Return encoded base64 of input.");

app.AddCommand("dbase64", async ([Option('f', Description = "Open decoded data in text editor")] bool? file, [Argument] string data) =>
{
    if (file.HasValue && file.Value == true)
    {
        var tempFileService = ServiceFactory.GetTempFileService();
        var decodeToFileResult = await Base64.DecodeToFile(tempFileService, data);
        Display.Info(decodeToFileResult);
        if (decodeToFileResult.Success)
        {
            Display.WaitForAKey("Press any key to open file.");
            Process.OpenFile(tempFileService.FilePath);
        }
    }
    else
    {
        Display.Result(Base64.Decode(data));
    }
})
    .WithDescription("Return decoded base64 of input.");

app.AddCommand("sha256", ([Argument] string data, [Argument] string? secret) => Display.Result(Sha256.Hash(data, secret)))
    .WithDescription("Hash the input with SHA256 algorithm.");

app.AddCommand("djwt", ([Argument] string data, [Argument] string? secret) => Display.Result(Jwt.Decode(data, secret)))
    .WithDescription("Return decoded JWT token.");

app.AddCommand("guid", () => Display.Result(UniqueID.Generate()))
    .WithDescription("Generate a Guid");

app.AddCommand("dt", ([Argument] string epochTime) =>
{
    Display.Result(EpochTime.Convert(epochTime, false));
    Display.Result(EpochTime.Convert(epochTime, true));
})
    .WithDescription("Convert epoch time to date time.");

app.Run();