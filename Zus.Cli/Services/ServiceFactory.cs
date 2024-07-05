using Zus.Cli.Commands;
using Zus.Cli.Models;

namespace Zus.Cli.Services;

internal static class ServiceFactory
{
    internal static SendRequest GetSendRequest()
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "zus-requests.json");
        IFileStreamFactory fileStreamFactory = new FileStreamFactory();
        IHttpHandler httpHandler = new HttpHandler(TimeSpan.FromSeconds(5));
        IFileService<Request> fileService = new FileService<Request>(fileStreamFactory, filePath);
        return new SendRequest(fileService, httpHandler);
    }
}