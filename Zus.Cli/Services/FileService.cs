using System.Text.Json;

namespace Zus.Cli.Services;

public class FileService<T> : IFileService<T> where T : class, IData
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly FileMode _fileMode;

    public FileService(string filePath, JsonSerializerOptions? jsonSerializerOptions = null, FileMode? fileMode = null)
    {
        _filePath = filePath;
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions { WriteIndented = true };
        _fileMode = fileMode ?? FileMode.OpenOrCreate;
    }

    public async Task Save(List<T> data)
    {
        using StreamWriter outputFile = new StreamWriter(_filePath);
        await outputFile.WriteAsync(JsonSerializer.Serialize(data, _jsonSerializerOptions));
    }

    public async Task<string> Get()
    {
        FileStreamOptions _fileStreamOptions = new FileStreamOptions { Mode = _fileMode };
        StreamReader streamReader = new StreamReader(_filePath, _fileStreamOptions);
        string data = await streamReader.ReadToEndAsync();
        streamReader.Close();
        streamReader.Dispose();

        return data;
    }

    public async Task<List<T>> GetDeserialize()
    {
        string data = await Get();
        List<T> deserializedData = string.IsNullOrEmpty(data) ? [] : JsonSerializer.Deserialize<List<T>>(data) ?? [];

        return deserializedData;
    }

    public async Task Save(T data, bool overwrite)
    {
        List<T> allData = await GetDeserialize();

        if (allData.FirstOrDefault(x => x.Id == data.Id) != null)
        {
            if (overwrite == false)
            {
                throw new KeyNotFoundException();
            }
            allData.RemoveAll(x => x.Id == data.Id);
        }

        allData.Add(data);
        await Save(allData);
    }

    public async Task Delete(string id)
    {
        List<T> allData = await GetDeserialize();
        allData.RemoveAll(x => x.Id == id);
        await Save(allData);
    }

    public async Task<T?> Get(string id)
    {
        List<T> allData = await GetDeserialize();
        return allData.FirstOrDefault(x => x.Id == id);
    }
}

public interface IFileService<T> where T : class
{
    public Task Save(List<T> data);
    public Task Save(T data, bool overwrite);
    public Task<List<T>> GetDeserialize();
    public Task<string> Get();
    public Task<T?> Get(string id);
    public Task Delete(string id);
}

public interface IData
{
    public string Id { get; set; }
}