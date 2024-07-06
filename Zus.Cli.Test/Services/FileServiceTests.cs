using System.Text;
using System.Text.Json;
using Zus.Cli.Models;
using Zus.Cli.Services;

namespace Zus.Cli.Test.Services;

public class FileServiceTests
{
    private readonly MemoryStream _mockMemoryStream;
    private readonly Mock<IFileStreamFactory> _mockFileStreamFactory;
    private readonly FileService<Request> _target;
    private readonly List<Request> _requests = new();
    private readonly string _requestJson;

    public FileServiceTests()
    {
        _requests.Add(new Request("http://test.com", null, RequestMethod.Get) { Name = "test" });
        _requestJson = JsonSerializer.Serialize(_requests, new JsonSerializerOptions { WriteIndented = true });
        byte[] fileBytes = Encoding.UTF8.GetBytes(_requestJson);

        _mockMemoryStream = new MemoryStream(fileBytes);

        _mockFileStreamFactory = new Mock<IFileStreamFactory>();
        _target = new FileService<Request>(_mockFileStreamFactory.Object, "file.json", null);
    }

    [Fact]
    public async void SaveAsync_Should_CallWrite()
    {
        //Arrange
        List<Request> requests = new()
        {
            new Request("abc", null, RequestMethod.Get)
        };
        var mockStreamWriter = new Mock<StreamWriter>(_mockMemoryStream);
        _mockFileStreamFactory.Setup(x => x.Writer(It.IsAny<string>()))
        .Returns(mockStreamWriter.Object);
        //Act
        await _target.SaveAsync(requests);
        //Assert
        _mockFileStreamFactory.Verify(x => x.Writer(It.IsAny<string>()), Times.Once);
        mockStreamWriter.Verify(x => x.WriteAsync(It.IsAny<string>()));
        mockStreamWriter.Verify(x => x.Close());
    }

    [Fact]
    public async void GetAsync_Should_CallReader()
    {
        //Arrange
        var mockStreamReader = new Mock<StreamReader>(_mockMemoryStream);
        _mockFileStreamFactory.Setup(x => x.Reader(It.IsAny<string>()))
        .Returns(mockStreamReader.Object);
        //Act
        await _target.GetAsync();
        //Assert
        _mockFileStreamFactory.Verify(x => x.Reader(It.IsAny<string>()), Times.Once);
        mockStreamReader.Verify(x => x.ReadToEndAsync());
        mockStreamReader.Verify(x => x.Close());
    }

    [Fact]
    public async void GetDeserializeAsync_Should_Return_DeserializeData()
    {
        //Arrange
        var mockStreamReader = new Mock<StreamReader>(_mockMemoryStream);
        _mockFileStreamFactory.Setup(x => x.Reader(It.IsAny<string>()))
        .Returns(mockStreamReader.Object);
        mockStreamReader.Setup(x => x.ReadToEndAsync()).Returns(Task.FromResult(_requestJson));
        //Act
        var result = await _target.GetDeserializeAsync();
        //Assert
        Assert.Equal("test", result.First().Name);
    }
}
