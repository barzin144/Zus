using System.Text;
using Zus.Cli.Models;

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
}
