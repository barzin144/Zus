using System.Text.Json;
using Zus.Models;

namespace Zus.Helpers
{
    internal static class Helper
    {
        private readonly static string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "zus-requests.json");
        private readonly static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        private readonly static FileStreamOptions _fileStreamOptions = new FileStreamOptions { Mode = FileMode.OpenOrCreate };

        internal static async Task SaveRequestToFile(Request request, bool overwrite)
        {
            StreamReader streamReader = new StreamReader(_filePath, _fileStreamOptions);
            string data = await streamReader.ReadToEndAsync();
            streamReader.Close();
            streamReader.Dispose();

            List<Request> requests = string.IsNullOrEmpty(data) ? [] : JsonSerializer.Deserialize<List<Request>>(data) ?? [];

            if (requests.FirstOrDefault(x => x.Name == request.Name) != null)
            {
                if (overwrite == false)
                {
                    Console.WriteLine($"Error: A request with the name '{request.Name}' already exists. To overwrite the existing request, please use the '-f' flag");
                    return;
                }
                requests.RemoveAll(x => x.Name == request.Name);
            }

            requests.Add(request);

            using StreamWriter outputFile = new StreamWriter(_filePath);
            await outputFile.WriteAsync(JsonSerializer.Serialize(requests, _jsonSerializerOptions));
        }

        internal static async Task<Request?> ReadRequestFromFile(string name)
        {
            StreamReader streamReader = new StreamReader(_filePath, _fileStreamOptions);
            string data = await streamReader.ReadToEndAsync();
            streamReader.Close();
            streamReader.Dispose();

            List<Request> requests = string.IsNullOrEmpty(data) ? [] : JsonSerializer.Deserialize<List<Request>>(data) ?? [];
            return requests.FirstOrDefault(x => x.Name == name);
        }
    }
}
