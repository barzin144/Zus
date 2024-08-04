using Zus.Cli.Commands;
using Zus.Cli.Services;

namespace Zus.Cli.Test;

public class Base64Tests
{
    private readonly Mock<IFileServiceBase> _fileServiceBaseMock;
    public Base64Tests()
    {

        _fileServiceBaseMock = new Mock<IFileServiceBase>();
    }

    [Fact]
    public async void EncodeFromFile_Should_ReturnError_When_FileNotFound()
    {
        //Arrange
        _fileServiceBaseMock.Setup(x => x.GetAsync()).ThrowsAsync(new Exception());
        //Act
        var result = await Base64.EncodeFromFile(_fileServiceBaseMock.Object);
        //Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public async void DecodeToFile_Should_ReturnError_When_DateIsNotValid()
    {
        //Arrange
        var invalidBase64 = "a";
        //Act
        var result = await Base64.DecodeToFile(_fileServiceBaseMock.Object, invalidBase64);
        //Assert
        _fileServiceBaseMock.Verify(x => x.SaveAsync(It.IsAny<string>()), Times.Never);
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
    }

    [Fact]
    public async void EncodeFromFile_Should_ReturnEncodedData()
    {
        //Arrange
        _fileServiceBaseMock.Setup(x => x.GetAsync()).ReturnsAsync("{\"app\":\"zus\"}");
        //Act
        var result = await Base64.EncodeFromFile(_fileServiceBaseMock.Object);
        //Assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
        Assert.Equal("eyJhcHAiOiJ6dXMifQ==", result.Result);
    }

    [Fact]
    public async void DecodeToFile_Should_CallSave()
    {
        //Arrange
        var invalidBase64 = "YQ==";
        //Act
        var result = await Base64.DecodeToFile(_fileServiceBaseMock.Object, invalidBase64);
        //Assert
        _fileServiceBaseMock.Verify(x => x.SaveAsync(It.IsAny<string>()), Times.Once);
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
    }
}
