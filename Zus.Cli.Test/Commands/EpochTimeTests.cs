using System;
using Zus.Cli.Commands;

namespace Zus.Cli.Test.Commands;

public class EpochTimeTests
{
	[Fact]
	public void Convert_Should_ReturnUTCDateTime_ForValidEpochTime()
	{
		// Arrange
		string epochTime = "1633072800";
		string expectedDateTime = "UTC: Friday, October 1, 2021 7:20:00 AM +00:00";

		// Act
		var result = EpochTime.Convert(epochTime, false);

		// Assert
		Assert.NotNull(result.Result);
		Assert.True(result.Success);
		Assert.Equal(expectedDateTime, result.Result);
	}

	[Fact]
	public void Convert_Should_ReturnError_ForInvalidEpochTime()
	{
		// Arrange
		string epochTime = "invalid";

		// Act
		var result = EpochTime.Convert(epochTime, false);

		// Assert
		Assert.NotNull(result.Error);
		Assert.False(result.Success);
	}

	[Fact]
	public void Convert_Should_ReturnError_ForEmptyEpochTime()
	{
		// Arrange
		string epochTime = "";

		// Act
		var result = EpochTime.Convert(epochTime, false);

		// Assert
		Assert.NotNull(result.Error);
		Assert.False(result.Success);
	}
}
