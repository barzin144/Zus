using System.Data;
using System.Net;
using System.Net.Http.Headers;
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

    [Fact]
    public async void SendAsync_Should_SaveRequest_When_NameIsNotNull()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Get);

        _mockFileService.Setup(x => x.SaveAsync(It.IsAny<Request>(), It.IsAny<bool>()));
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
        var result = await _target.SendAsync(request, "request_name", true);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        _mockFileService.Verify(x => x.SaveAsync(It.IsAny<Request>(), true), Times.Once);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
        Assert.True(result.Success);
    }

    [Fact]
    public async void SendAsync_Should_ThrowException_When_NameIsDuplicate()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Get);

        _mockFileService.Setup(x => x.SaveAsync(It.IsAny<Request>(), false)).ThrowsAsync(new DuplicateNameException());
        //Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _target.SendAsync(request, "request_name", false));
        _mockFileService.Verify(x => x.SaveAsync(It.IsAny<Request>(), false), Times.Once);
    }

    [Fact]
    public async void SendAsync_Should_ReturnError_When_HttpClientThrowAnException()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Get);

        _mockHttpHandler.Setup(x => x.GetAsync(It.IsAny<string>())).ThrowsAsync(new Exception());
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        _mockFileService.Verify(x => x.SaveAsync(It.IsAny<Request>(), It.IsAny<bool>()), Times.Never);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
    }

    [Fact]
    public async void SendAsync_Should_ReturnError_When_HttpClientThrowHttpRequestException()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Get);

        _mockHttpHandler.Setup(x => x.GetAsync(It.IsAny<string>())).ThrowsAsync(new HttpRequestException());
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        _mockFileService.Verify(x => x.SaveAsync(It.IsAny<Request>(), It.IsAny<bool>()), Times.Never);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
    }

    [Fact]
    public async void SendAsync_Should_ReturnError_When_HttpClientThrowTaskCanceledException()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Get);

        _mockHttpHandler.Setup(x => x.GetAsync(It.IsAny<string>())).ThrowsAsync(new TaskCanceledException());
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        _mockFileService.Verify(x => x.SaveAsync(It.IsAny<Request>(), It.IsAny<bool>()), Times.Never);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
    }

    [Theory]
    [InlineData(RequestMethod.Get)]
    [InlineData(RequestMethod.Post)]
    public async void SendAsync_Should_ReturnResult(RequestMethod requestMethod)
    {
        //Arrange
        Request request = new Request("http://test.com", null, requestMethod, "propName:propValue");

        if (requestMethod == RequestMethod.Get)
        {
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
        }
        else if (requestMethod == RequestMethod.Post)
        {
            _mockHttpHandler.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
            .Returns(
                Task.FromResult(
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("ok")
                    }
                )
            );

        }
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        if (requestMethod == RequestMethod.Get)
        {
            _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        }
        else if (requestMethod == RequestMethod.Post)
        {
            _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
        }
        _mockFileService.Verify(x => x.SaveAsync(It.IsAny<Request>(), It.IsAny<bool>()), Times.Never);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
        Assert.True(result.Success);
    }

    [Fact]
    public async void SendAsync_Should_ReturnError_When_PreRequestNameIsNotFound()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Get, "", false, "preRequest_name");
        _mockFileService.Setup(x => x.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(null as Request));
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Never);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
    }

    [Fact]
    public async void SendAsync_Should_ReturnResult_When_HasPreRequest()
    {
        //Arrange
        Request request = new Request("http://test.com", "{pr.token}", RequestMethod.Post, "requestData:{pr.data}", false, "preRequest_name");
        Request preRequest = new Request("http://prerequest.com", null, RequestMethod.Get);
        _mockFileService.Setup(x => x.GetAsync("preRequest_name")).Returns(Task.FromResult<Request?>(preRequest));
        _mockHttpHandler.Setup(x => x.GetAsync("http://prerequest.com"))
        .Returns(
                Task.FromResult(
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{\"token\":\"abc\",\"data\":\"def\"}")
                    }
                )
                );
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
        _mockHttpHandler.Verify(x => x.AddHeader("Authorization", "Bearer abc"), Times.Once);
        Assert.Equal("requestData:def", request.Data);
        Assert.Equal("abc", request.Auth);
    }

    [Fact]
    public async void SendAsync_Should_ReturnError_When_PreRequestResponseIsString()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Post, "requestData:{pr.data}", false, "preRequest_name");
        Request preRequest = new Request("http://prerequest.com", null, RequestMethod.Get);
        _mockFileService.Setup(x => x.GetAsync("preRequest_name")).Returns(Task.FromResult<Request?>(preRequest));
        _mockHttpHandler.Setup(x => x.GetAsync("http://prerequest.com"))
        .Returns(
                Task.FromResult(
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("InValidResponse")
                    }
                )
                );
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Never);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
    }

    [Theory]
    [InlineData(true, "application/x-www-form-urlencoded")]
    [InlineData(false, "application/json")]
    public async void SendAsync_Should_AddProperHeaderToHttpClient(bool formFormat, string headerValue)
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Post, "requestData:data", formFormat);
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
        _mockHttpHandler.Verify(x => x.AddHeader(new MediaTypeWithQualityHeaderValue(headerValue)), Times.Once);
    }

    [Fact]
    public async void SendAsync_Should_AddAuthToHttpClientHeader()
    {
        //Arrange
        Request request = new Request("http://test.com", "abc", RequestMethod.Get);
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        _mockHttpHandler.Verify(x => x.AddHeader("Authorization", "Bearer abc"), Times.Once);
    }

    [Fact]
    public async void SendAsync_Should_SendRequest_When_RegexInDataIsNotValid()
    {
        //Arrange
        Request request = new Request("http://test.com", null, RequestMethod.Post, "requestData:{prr.data}", false, "preRequest_name");
        Request preRequest = new Request("http://prerequest.com", null, RequestMethod.Get);
        _mockFileService.Setup(x => x.GetAsync("preRequest_name")).Returns(Task.FromResult<Request?>(preRequest));
        _mockHttpHandler.Setup(x => x.GetAsync("http://prerequest.com"))
        .Returns(
                Task.FromResult(
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("{\"data\":\"abc\"}")
                    }
                )
                );
        //Act
        var result = await _target.SendAsync(request, null, false);
        //Assert
        _mockHttpHandler.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
        _mockHttpHandler.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()), Times.Once);
        Assert.Equal("requestData:{prr.data}", request.Data);
    }
}
