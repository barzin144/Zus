using Zus.Models;

namespace Zus.Helpers
{
    internal static class Display
    {
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
    }
}
