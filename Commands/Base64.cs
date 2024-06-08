using System.Text;

namespace Zus.Commands
{
    internal static class Base64
    {
        internal static string Encode(string data)
        {
            var dataByte = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(dataByte);
        }
        internal static string Decode(string data)
        {
            var dataByte = Convert.FromBase64String(data.PadRight(data.Length / 4 * 4 + (data.Length % 4 == 0 ? 0 : 4), '='));
            return Encoding.UTF8.GetString(dataByte);
        }
    }
}
