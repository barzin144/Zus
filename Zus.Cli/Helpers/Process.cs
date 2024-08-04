namespace Zus.Cli.Helpers;

internal static class Process
{
    internal static void OpenFile(string filePath)
    {

        if (string.IsNullOrEmpty(filePath) == false)
        {
            new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
    }
}
