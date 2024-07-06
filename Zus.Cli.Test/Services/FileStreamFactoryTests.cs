using Zus.Cli.Services;

namespace Zus.Cli.Test.Services;

public class FileStreamFactoryTests
{
    [Fact]
    public void Reader_Should_ReturnReader()
    {
        //Arrange
        string filePath = "reader.json";
        var target = new FileStreamFactory().Reader(filePath);
        //Act & Assert
        Assert.NotNull(target);
    }

    [Fact]
    public void Writer_Should_ReturnWriter()
    {
        //Arrange
        string filePath = "writer.json";
        var target = new FileStreamFactory().Writer(filePath);
        //Act & Assert
        Assert.NotNull(target);
    }
}
