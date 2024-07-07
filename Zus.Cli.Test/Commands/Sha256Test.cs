using Zus.Cli.Commands;

namespace Zus.Cli.Test.Commands;

public class Sha256Test
{

    [Fact]
    public void Sha256_Should_ReturnHash_NoSecret()
    {
        //Arrange
        string expectedHash = "/XrbFSwF74Dcz1Ch+kwF1aPsbalVdfwxKufF0JGDY1E=";
        //Act
        var result = Sha256.Hash("abc", string.Empty);
        //Assert
        Assert.NotNull(result.Result);
        Assert.True(result.Success);
        Assert.Equal(expectedHash, result.Result);
    }

    [Fact]
    public void Sha256_Should_ReturnHash()
    {
        //Arrange
        string expectedHash = "jxZ3H5+IUbJvTUYPoX3pPicRx+UTN8uKYIoPgeHBtq4=";
        //Act
        var result = Sha256.Hash("abc", "123");
        //Assert
        Assert.NotNull(result.Result);
        Assert.True(result.Success);
        Assert.Equal(expectedHash, result.Result);
    }
}
