namespace Zus.Cli.Services;

public interface IFileStreamFactory
{
    StreamReader Reader(string filePath);
    StreamWriter Writer(string filePath);
}

public class FileStreamFactory : IFileStreamFactory
{
    public StreamReader Reader(string filePath)
    {
        FileStreamOptions fileStreamOptions = new FileStreamOptions { Mode = FileMode.OpenOrCreate };
        return new StreamReader(filePath, fileStreamOptions);
    }

    public StreamWriter Writer(string filePath)
    {
        return new StreamWriter(filePath);
    }
}