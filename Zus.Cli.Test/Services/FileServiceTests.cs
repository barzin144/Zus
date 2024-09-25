using System.Data;
using System.Text;
using System.Text.Json;
using Zus.Cli.Models;
using Zus.Cli.Services;

namespace Zus.Cli.Test.Services;

public class FileServiceTests
{
    private readonly MemoryStream _mockMemoryStream;
    private readonly Mock<IFileStreamFactory> _mockFileStreamFactory;
    private readonly Mock<StreamReader> _mockStreamReader;
    private readonly Mock<StreamWriter> _mockStreamWriter;
    private readonly FileService<Request> _target;
    private readonly List<Request> _requests = new();
    private readonly string _requestJson;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    public FileServiceTests()
    {
        _requests.Add(new Request("http://test.com", null, RequestMethod.Get) { Name = "test" });
        _requestJson = JsonSerializer.Serialize(_requests, _jsonSerializerOptions);
        byte[] fileBytes = Encoding.UTF8.GetBytes(_requestJson);

        _mockMemoryStream = new MemoryStream(fileBytes);

        _mockFileStreamFactory = new Mock<IFileStreamFactory>();
        _mockStreamReader = new Mock<StreamReader>(_mockMemoryStream);
        _mockStreamWriter = new Mock<StreamWriter>(_mockMemoryStream);
        _mockFileStreamFactory.Setup(x => x.Reader(It.IsAny<string>())).Returns(_mockStreamReader.Object);
        _mockFileStreamFactory.Setup(x => x.Writer(It.IsAny<string>())).Returns(_mockStreamWriter.Object);
        _mockStreamReader.Setup(x => x.ReadToEndAsync()).ReturnsAsync(_requestJson);

        _target = new FileService<Request>(_mockFileStreamFactory.Object, "file.json", null);
    }

    [Fact]
    public async void FileService_Should_SetJsonSerializerOptions()
    {
        //Arrange
        var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = false };
        var requestJson = JsonSerializer.Serialize(_requests, jsonSerializerOptions);
        FileService<Request> fileService = new FileService<Request>(
            _mockFileStreamFactory.Object, "file.json", jsonSerializerOptions);
        //Act
        await fileService.SaveAsync(_requests);
        //Assert
        _mockStreamWriter.Verify(x => x.WriteAsync(JsonSerializer.Serialize(_requests, new JsonSerializerOptions { WriteIndented = false })));
    }

    [Fact]
    public async void SaveAsync_Should_CallWrite()
    {
        //Act
        await _target.SaveAsync(_requests);
        //Assert
        _mockFileStreamFactory.Verify(x => x.Writer(It.IsAny<string>()), Times.Once);
        _mockStreamWriter.Verify(x => x.WriteAsync(JsonSerializer.Serialize(_requests, _jsonSerializerOptions)));
        _mockStreamWriter.Verify(x => x.Close());
    }

    [Fact]
    public async void GetAsync_Should_CallReader()
    {
        //Act
        await _target.GetAsync();
        //Assert
        _mockFileStreamFactory.Verify(x => x.Reader(It.IsAny<string>()), Times.Once);
        _mockStreamReader.Verify(x => x.ReadToEndAsync());
        _mockStreamReader.Verify(x => x.Close());
    }

    [Fact]
    public async void GetDeserializeAsync_Should_Return_DeserializeData()
    {
        //Arrange
        //Act
        var result = await _target.GetDeserializeAsync();
        //Assert
        Assert.Equal("test", result.First().Name);
    }

    [Fact]
    public async void SaveAsync_Should_OverwriteARequest()
    {
        //Arrange
        var newRequest = new Request("http://test.com", "ABC", RequestMethod.Get) { Name = "test" };
        var expectedRequests = new List<Request> { newRequest };
        //Act
        await _target.SaveAsync(newRequest, true);
        //Assert
        _mockStreamWriter.Verify(x => x.WriteAsync(JsonSerializer.Serialize(expectedRequests, _jsonSerializerOptions)));
    }

    [Fact]
    public async void SaveAsync_Should_ThrowAnException_When_OverwriteIsFalse()
    {
        //Arrange
        var newRequest = new Request("http://test.com", "ABC", RequestMethod.Get) { Name = "test" };
        //Act & Assert
        await Assert.ThrowsAsync<DuplicateNameException>(async () => await _target.SaveAsync(newRequest, false));
    }

    [Fact]
    public async void SaveAsync_Should_SaveRequest()
    {
        //Arrange
        var newRequest = new Request("http://test.com", "ABC", RequestMethod.Get) { Name = "newRequest" };
        //Act
        await _target.SaveAsync(newRequest, true);
        //Assert
        _requests.Add(newRequest);
        _mockStreamWriter.Verify(x => x.WriteAsync(JsonSerializer.Serialize(_requests, _jsonSerializerOptions)));
    }

    [Fact]
    public async void DeleteAsync_Should_DeleteRequest()
    {
        //Arrange
        //Act
        await _target.DeleteAsync("test");
        //Assert
        _mockStreamWriter.Verify(x => x.WriteAsync("[]"));
    }

    [Fact]
    public async void DeleteAsync_When_NameIsNotFound()
    {
        //Arrange
        //Act
        await _target.DeleteAsync("randomName");
        //Assert
        _mockStreamWriter.Verify(x => x.WriteAsync(_requestJson));
    }

    [Fact]
    public async void GetAsync_Should_ReturnARequest()
    {
        //Arrange
        //Act
        var result = await _target.GetAsync("test");
        //Assert
        Assert.NotNull(result);
        Assert.Equal("test", result?.Id);
    }

    [Fact]
    public async void GetAsync_Should_ReturnNull_When_NameIsNotFound()
    {
        //Arrange
        //Act
        var result = await _target.GetAsync("randomName");
        //Assert
        Assert.Null(result);
    }
}
