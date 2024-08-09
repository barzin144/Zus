using Zus.Cli.Models;

namespace Zus.Cli.Helpers;

internal static class Display
{
    internal static void Info(CommandResult result)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Result(result);
        Console.ResetColor();

    }

    internal static void Result(CommandResult result)
    {
        if (result.Success)
        {
            Console.WriteLine(result.Result);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
        }
    }

    internal static string ConfirmMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(message);
        Console.ResetColor();
        return Console.ReadLine() ?? string.Empty;
    }

    internal static void WaitForAKey(string message)
    {
        Console.Write(message);
        Console.ReadKey();
    }
}
