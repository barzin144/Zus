namespace Zus.Cli.Services;

public interface IFileStreamFactory
{
    StreamReader Reader(string filePath);
    StreamWriter Writer(string filePath);
}

public class FileStreamFactory : IFileStreamFactory
{
    private readonly FileMode _fileMode;

    public FileStreamFactory(FileMode fileMode = FileMode.OpenOrCreate)
    {
        _fileMode = fileMode;
    }
    public StreamReader Reader(string filePath)
    {
        FileStreamOptions fileStreamOptions = new FileStreamOptions { Mode = _fileMode };
        return new StreamReader(filePath, fileStreamOptions);
    }

    public StreamWriter Writer(string filePath)
    {
        return new StreamWriter(filePath);
    }
}