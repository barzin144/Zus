using Zus.Cli.Commands;
using Zus.Cli.Models;

namespace Zus.Cli.Services;

internal static class Factory
{
    internal static SendRequest GetSendRequest()
    {
        string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "zus-requests.json");
        var fileService = new FileService<Request>(_filePath);
        return new SendRequest(fileService);
    }
}
