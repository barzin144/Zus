using System.Runtime.InteropServices;
using Zus.Cli.Commands;
using Zus.Cli.Models;

namespace Zus.Cli.Services;

internal static class ServiceFactory
{
    private static string GetFilePath(ZusFileType zusFileType)
    {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var specialFolder = isWindows ? Environment.SpecialFolder.MyDocuments : Environment.SpecialFolder.UserProfile;
        switch (zusFileType)
        {
            case ZusFileType.Requests:
                return Path.Combine(Environment.GetFolderPath(specialFolder), "zus-requests.json");
            case ZusFileType.Responses:
                return Path.Combine(Environment.GetFolderPath(specialFolder), "zus-responses.json");
            case ZusFileType.Variables:
                return Path.Combine(Environment.GetFolderPath(specialFolder), "zus-variables.json");
            default: throw new NotSupportedException();
        }
    }

    internal static SendRequest GetSendRequestService()
    {
        string filePath = GetFilePath(ZusFileType.Requests);
        IFileStreamFactory fileStreamFactory = new FileStreamFactory();
        IHttpHandler httpHandler = new HttpHandler(TimeSpan.FromSeconds(5));
        IFileService<Request> fileService = new FileService<Request>(fileStreamFactory, filePath);
        VariablesService variablesService = GetVariablesService();
        IFileService<Response> responsesService = GetResponsesService();
        return new SendRequest(fileService, httpHandler, variablesService, responsesService);
    }

    internal static ManageVariables GetManageVariables()
    {
        VariablesService variablesService = GetVariablesService();
        return new ManageVariables(variablesService);
    }

    internal static VariablesService GetVariablesService()
    {
        string filePath = GetFilePath(ZusFileType.Variables);
        IFileStreamFactory fileStreamFactory = new FileStreamFactory();
        IFileService<LocalVariable> fileService = new FileService<LocalVariable>(fileStreamFactory, filePath);
        return new VariablesService(fileService);
    }

    internal static FileService<Response> GetResponsesService()
    {
        string filePath = GetFilePath(ZusFileType.Responses);
        IFileStreamFactory fileStreamFactory = new FileStreamFactory();
        return new FileService<Response>(fileStreamFactory, filePath);
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
