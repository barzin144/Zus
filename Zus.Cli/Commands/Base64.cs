using System.Text;
using Zus.Cli.Models;
using Zus.Cli.Services;

namespace Zus.Cli.Commands;

internal static class Base64
{
    internal static CommandResult Encode(string data)
    {
        try
        {
            var dataByte = Encoding.UTF8.GetBytes(data);
            return new CommandResult { Result = Convert.ToBase64String(dataByte) };
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }

    internal static CommandResult Decode(string data)
    {
        try
        {
            var dataByte = Convert.FromBase64String(data.PadRight(data.Length / 4 * 4 + (data.Length % 4 == 0 ? 0 : 4), '='));
            return new CommandResult { Result = Encoding.UTF8.GetString(dataByte) };
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }

    internal static async Task<CommandResult> EncodeFromFile(IFileServiceBase fileReaderService)
    {
        try
        {
            var data = await fileReaderService.GetAsync();
            return Encode(data);
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }

    internal static async Task<CommandResult> DecodeToFile(IFileServiceBase tempFileService, string data)
    {
        try
        {
            var decodeResult = Decode(data);
            if (decodeResult.Success)
            {
                await tempFileService.SaveAsync(decodeResult.Result!);
                return new CommandResult { Result = $"File created: {tempFileService.FilePath}" };
            }
            else
            {
                return decodeResult;
            }
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }
}
