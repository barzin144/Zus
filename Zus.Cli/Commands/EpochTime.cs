using Zus.Cli.Models;

namespace Zus.Cli.Commands;

public static class EpochTime
{
	internal static CommandResult Convert(string epochTime)
	{
		try
		{
			var utcDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(epochTime)).UtcDateTime;
			TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
			DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, localTimeZone);

			return new CommandResult
			{
				Result =
			$"UTC: {utcDateTime:dddd, MMMM d, yyyy h:mm:ss tt zzz}{Environment.NewLine}Local: {localDateTime:dddd, MMMM d, yyyy h:mm:ss tt zzz}"
			};
		}
		catch (Exception ex)
		{
			return new CommandResult { Error = ex.Message };
		}
	}
}
