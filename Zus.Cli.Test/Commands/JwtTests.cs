using Zus.Cli.Commands;

namespace Zus.Cli.Test.Commands;

public class JwtTests
{
    private readonly string _header;
    private readonly string _payload;
    private readonly string _sign;

    public JwtTests()
    {
        _header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
        _payload = "eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Ilp1cyBDbGkgVG9vbCIsImlhdCI6MTUxNjIzOTAyMn0";
        _sign = "xElsnp670YQX-H2s29Hqig5dPGGJCVRLMKk2Y9uMVS8";
    }
    [Fact]
    public void Decode_Should_ReturnError_When_HeaderIsNotValid()
    {
        //Arrange
        string data = $"{_header}=.{_payload}.{_sign}";
        //Act
        var result = Jwt.Decode(data, null);
        //Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Decode_Should_ReturnError_When_PayloadIsNotValid()
    {
        //Arrange
        string data = $"{_header}.{_payload}==.{_sign}";
        //Act
        var result = Jwt.Decode(data, null);
        //Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Decode_Should_ReturnDecodedJwt_When_SignIsValid()
    {
        //Arrange
        string data = $"{_header}.{_payload}.{_sign}";
        //Act
        var result = Jwt.Decode(data, "zus");
        //Assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
        Assert.Contains("Signature Verified", result.Result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Decode_Should_ReturnDecodedJwt_When_SignIsNotValid(string? signature)
    {
        //Arrange
        string data = $"{_header}.{_payload}.{_sign}";
        //Act
        var result = Jwt.Decode(data, signature);
        //Assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
        Assert.Contains("Invalid Signature", result.Result);
    }
}
