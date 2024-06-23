using Zus.Models;

namespace Zus.Helpers
{
    internal static class Display
    {
        internal static void DisplayResult(CommandResult result)
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
    }
}
