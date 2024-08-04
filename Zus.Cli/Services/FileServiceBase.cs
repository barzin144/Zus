namespace Zus.Cli.Services;

public class FileServiceBase : IFileServiceBase
{
    private readonly IFileStreamFactory _fileStreamFactory;
    private readonly string _filePath;

    public FileServiceBase(IFileStreamFactory fileStreamFactory, string filePath)
    {
        _fileStreamFactory = fileStreamFactory;
        _filePath = filePath;
    }

    public async Task<string> GetAsync()
    {
        var _streamReader = _fileStreamFactory.Reader(_filePath);
        string data = await _streamReader.ReadToEndAsync();
        _streamReader.Close();
        _streamReader.Dispose();

        return data;
    }

    public async Task SaveAsync(string data)
    {
        var _streamWriter = _fileStreamFactory.Writer(_filePath);
        await _streamWriter.WriteAsync(data);
        _streamWriter.Close();
        _streamWriter.Dispose();
    }

    public string FilePath { get { return _filePath; } }
}

public interface IFileServiceBase
{
    public string FilePath { get; }
    public Task<string> GetAsync();
    public Task SaveAsync(string data);
}
