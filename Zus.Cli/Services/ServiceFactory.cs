using Zus.Cli.Commands;
using Zus.Cli.Models;

namespace Zus.Cli.Services;

internal static class ServiceFactory
{
    internal static SendRequest GetSendRequestService()
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "zus-requests.json");
        IFileStreamFactory fileStreamFactory = new FileStreamFactory();
        IHttpHandler httpHandler = new HttpHandler(TimeSpan.FromSeconds(5));
        IFileService<Request> fileService = new FileService<Request>(fileStreamFactory, filePath);
        VariablesService variablesService = GetVariablesService();
        return new SendRequest(fileService, httpHandler, variablesService);
    }

    internal static ManageVariables GetManageVariables()
    {
        VariablesService variablesService = GetVariablesService();
        return new ManageVariables(variablesService);
    }

    internal static VariablesService GetVariablesService()
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "zus-variables.json");
        IFileStreamFactory fileStreamFactory = new FileStreamFactory();
        IFileService<LocalVariable> fileService = new FileService<LocalVariable>(fileStreamFactory, filePath);
        return new VariablesService(fileService);
    }

    internal static FileServiceBase GetFileReaderService(string filePath)
    {
        FileStreamFactory fileStreamFactory = new FileStreamFactory(FileMode.Open);
        return new FileServiceBase(fileStreamFactory, filePath);
    }

    internal static FileServiceBase GetTempFileService()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
        FileStreamFactory fileStreamFactory = new FileStreamFactory(FileMode.Create);
        return new FileServiceBase(fileStreamFactory, filePath);
    }
}