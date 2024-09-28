using Zus.Cli.Models;

namespace Zus.Cli.Commands;

internal static class UniqueID
{
    internal static CommandResult Generate()
    {
        try
        {
            return new CommandResult { Result = Guid.NewGuid().ToString() };
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }
}
