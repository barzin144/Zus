using System.Net;
using Zus.Cli.Commands;
using Zus.Cli.Models;
using Zus.Cli.Services;

namespace Zus.Cli.Test.Commands;

public class SendRequestTests
{
    private readonly Mock<IFileService<Request>> _mockFileService;
    private readonly Mock<IHttpHandler> _mockHttpHandler;
    private readonly SendRequest _target;

    public SendRequestTests()
    {
        _mockFileService = new Mock<IFileService<Request>>();
        _mockHttpHandler = new Mock<IHttpHandler>();
        _target = new SendRequest(_mockFileService.Object, _mockHttpHandler.Object);
    }

    [Fact]
    public async void DeleteAsync_Should_CallDelete_When_RetypedNameIsMatch()
    {
        //Arrange
        string requestName = "request_name";
        _mockFileService.Setup(x => x.DeleteAsync(requestName)).Returns(Task.CompletedTask);
        using StringWriter output = new();
        Console.SetOut(output);
        using StringReader input = new(requestName);
        Console.SetIn(input);
        //Act
        var result = await _target.DeleteAsync(requestName);
        //Assert
        _mockFileService.Verify(x => x.DeleteAsync(requestName), Times.Once);
        Assert.NotNull(result.Result);
        Assert.Null(result.Error);
        Assert.True(result.Success);
    }

    [Fact]
    public async void DeleteAsync_Should_ReturnError_When_RetypedNameIsNotMatch()
    {
        //Arrange
        string requestName = "request_name";
        _mockFileService.Setup(x => x.DeleteAsync(requestName)).Returns(Task.CompletedTask);
        using StringWriter output = new();
        Console.SetOut(output);
        using StringReader input = new("retyped_name");
        Console.SetIn(input);
        //Act
        var result = await _target.DeleteAsync(requestName);
        //Assert
        _mockFileService.Verify(x => x.DeleteAsync(requestName), Times.Never);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
    }

    [Fact]
    public async void ResendAsync_Should_ReturnError_When_RequestIsNotFound()
    {
        //Arrange
        string requestName = "request_name";
        _mockFileService.Setup(x => x.GetAsync(requestName)).Returns(Task.FromResult(null as Request));
        //Act
        var result = await _target.ResendAsync(requestName);
        //Assert
        _mockFileService.Verify(x => x.GetAsync(requestName), Times.Once);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
    }

    [Fact]
    public async void ResendAsync_Should_CallHttpClientGet()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Get);

        _mockFileService.Setup(x => x.GetAsync(It.IsAny<string>())).Returns(Task.FromResult<Request?>(request));
        _mockHttpHandler.Setup(x => x.GetAsync(It.IsAny<string>()))
        .Returns(
            Task.FromResult(
                new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("ok")
                }
            )
        );
        //Act
        var result = await _target.ResendAsync("request_name");
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
        Assert.True(result.Success);
    }
}
