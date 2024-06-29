using System.Text.Json;
using Zus.Models;

namespace Zus.Helpers
{
    internal static class Helper
    {
        private readonly static string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "zus-requests.json");
        private readonly static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
        private readonly static FileStreamOptions _fileStreamOptions = new FileStreamOptions { Mode = FileMode.OpenOrCreate };
        private static async Task SaveToFile(List<Request> requests)
        {
            using StreamWriter outputFile = new StreamWriter(_filePath);
            await outputFile.WriteAsync(JsonSerializer.Serialize(requests, _jsonSerializerOptions));
        }

        internal static async Task SaveRequestToFile(Request request, bool overwrite)
        {
            List<Request> requests = await AllRequestFromFile();

            if (requests.FirstOrDefault(x => x.Name == request.Name) != null)
            {
                if (overwrite == false)
                {
                    throw new Exception($"Error: A request with the name '{request.Name}' already exists. To overwrite the existing request, please use the '-f' flag");
                }
                requests.RemoveAll(x => x.Name == request.Name);
            }

            requests.Add(request);
            await SaveToFile(requests);
        }

        internal static async Task DeleteRequest(string name)
        {
            List<Request> requests = await AllRequestFromFile();
            requests.RemoveAll(x => x.Name == name);
            await SaveToFile(requests);
        }

        internal static async Task<Request?> ReadRequestFromFile(string name)
        {
            List<Request> requests = await AllRequestFromFile();
            return requests.FirstOrDefault(x => x.Name == name);
        }

        internal static async Task<List<Request>> AllRequestFromFile()
        {
            string data = await AllRequestFromFileString();
            List<Request> requests = string.IsNullOrEmpty(data) ? [] : JsonSerializer.Deserialize<List<Request>>(data) ?? [];

            return requests;
        }

        internal static async Task<string> AllRequestFromFileString()
        {
            StreamReader streamReader = new StreamReader(_filePath, _fileStreamOptions);
            string data = await streamReader.ReadToEndAsync();
            streamReader.Close();
            streamReader.Dispose();

            return data;
        }
    }
}
