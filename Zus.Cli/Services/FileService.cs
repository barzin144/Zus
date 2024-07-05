using System.Text.Json;

namespace Zus.Cli.Services;

public class FileService<T> : IFileService<T> where T : class, IData
{
    private readonly IFileStreamFactory _fileStreamFactory;
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public FileService(IFileStreamFactory fileStreamFactory, string filePath, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _fileStreamFactory = fileStreamFactory;
        _filePath = filePath;
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions { WriteIndented = true };
    }

    public async Task SaveAsync(List<T> data)
    {
        var _streamWriter = _fileStreamFactory.Writer(_filePath);
        await _streamWriter.WriteAsync(JsonSerializer.Serialize(data, _jsonSerializerOptions));
        _streamWriter.Close();
        _streamWriter.Dispose();
    }

    public async Task<string> GetAsync()
    {
        var _streamReader = _fileStreamFactory.Reader(_filePath);
        string data = await _streamReader.ReadToEndAsync();
        _streamReader.Close();
        _streamReader.Dispose();

        return data;
    }

    public async Task<List<T>> GetDeserializeAsync()
    {
        string data = await GetAsync();
        List<T> deserializedData = string.IsNullOrEmpty(data) ? [] : JsonSerializer.Deserialize<List<T>>(data) ?? [];

        return deserializedData;
    }

    public async Task SaveAsync(T data, bool overwrite)
    {
        List<T> allData = await GetDeserializeAsync();

        if (allData.FirstOrDefault(x => x.Id == data.Id) != null)
        {
            if (overwrite == false)
            {
                throw new KeyNotFoundException();
            }
            allData.RemoveAll(x => x.Id == data.Id);
        }

        allData.Add(data);
        await SaveAsync(allData);
    }

    public async Task DeleteAsync(string id)
    {
        List<T> allData = await GetDeserializeAsync();
        allData.RemoveAll(x => x.Id == id);
        await SaveAsync(allData);
    }

    public async Task<T?> GetAsync(string id)
    {
        List<T> allData = await GetDeserializeAsync();
        return allData.FirstOrDefault(x => x.Id == id);
    }
}

public interface IFileService<T> where T : class
{
    public Task SaveAsync(List<T> data);
    public Task SaveAsync(T data, bool overwrite);
    public Task<List<T>> GetDeserializeAsync();
    public Task<string> GetAsync();
    public Task<T?> GetAsync(string id);
    public Task DeleteAsync(string id);
}

public interface IData
{
    public string Id { get; set; }
}