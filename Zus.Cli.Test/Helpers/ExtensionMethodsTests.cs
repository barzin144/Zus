using System.Net;
using System.Text.Json;
using Zus.Cli.Helpers;

namespace Zus.Cli.Test.Helpers;

public class ExtensionMethodsTests
{
    private readonly HttpResponseMessage _httpResponseMessage;
    private readonly string _content = "{\"PropName\":\"PropValue\"}";
    private readonly static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    public ExtensionMethodsTests()
    {
        //Arrange
        _httpResponseMessage = new()
        {
            StatusCode = HttpStatusCode.OK
        };
    }

    [Fact]
    public async void GetProperty_Should_ReturnPropertyValue()
    {
        //Arrange
        _httpResponseMessage.Content = new StringContent(_content);
        //Act
        string result = await _httpResponseMessage.GetPropertyValue("PropName");
        //Assert
        Assert.Equal("PropValue", result);
    }

    [Fact]
    public async void GetProperty_Should_ThrowAnException_When_ContentIsString()
    {
        //Arrange
        _httpResponseMessage.Content = new StringContent("Test");
        //Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _httpResponseMessage.GetPropertyValue("PropName"));
    }

    [Fact]
    public async void GetProperty_Should_ThrowAnException_When_PropertyNameNotFound()
    {
        //Arrange
        _httpResponseMessage.Content = new StringContent(_content);
        //Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await _httpResponseMessage.GetPropertyValue("TestProp"));
    }

    [Fact]
    public void BeautifyJson_Should_ReturnSerializedDataWithIndent()
    {
        //Arrange
        var contentJson = new
        {
            PropName = "PropValue"
        };
        //Act & Assert
        Assert.Equal(JsonSerializer.Serialize(contentJson, _jsonSerializerOptions), _content.BeautifyJson());
    }

    [Fact]
    public async void BeautifyHttpResponse_Should_ReturnSerializedDataWithIndent_When_ContentIsEmpty()
    {
        //Arrange
        var response = new
        {
            Status = HttpStatusCode.OK.ToString(),
            Content = string.Empty
        };
        //Act & Assert
        Assert.Equal(JsonSerializer.Serialize(response, _jsonSerializerOptions), await _httpResponseMessage.BeautifyHttpResponse());
    }

    [Fact]
    public async void BeautifyHttpResponse_Should_ReturnSerializedDataWithIndent_When_ContentIsJson()
    {
        //Arrange
        _httpResponseMessage.Content = new StringContent(_content);
        var response = new
        {
            Status = HttpStatusCode.OK.ToString(),
            Content = new { PropName = "PropValue" }
        };
        //Act & Assert
        Assert.Equal(JsonSerializer.Serialize(response, _jsonSerializerOptions), await _httpResponseMessage.BeautifyHttpResponse());
    }

    [Fact]
    public async void BeautifyHttpResponse_Should_ReturnSerializedDataWithIndent_When_ContentIsString()
    {
        //Arrange
        _httpResponseMessage.Content = new StringContent("Result");
        var response = new
        {
            Status = HttpStatusCode.OK.ToString(),
            Content = "Result"
        };
        //Act & Assert
        Assert.Equal(JsonSerializer.Serialize(response, _jsonSerializerOptions), await _httpResponseMessage.BeautifyHttpResponse());
    }

    [Fact]
    public void ConvertStringDataToDictionary_Should_ReturnDictionary()
    {
        //Arrange
        string data = "PropName:PropValue";
        //Act
        var dataDic = ExtensionMethods.ConvertStringDataToDictionary(data);
        //Assert
        Assert.Equal("PropValue", dataDic["PropName"]);
    }

    [Theory]
    [InlineData("PropName:PropValue","{\"PropName\":\"PropValue\"}")]
    [InlineData("first:firstValue,second:secondValue","{\"first\":\"firstValue\",\"second\":\"secondValue\"}")]
    public async void ToJsonStringContent_Should_ReturnJsonSerializedString(string data, string expected)
    {
        //Act
        var result = await data.ToJsonStringContent().ReadAsStringAsync();
        //Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("PropName:PropValue","PropName=PropValue")]
    [InlineData("first:firstValue,second:secondValue","first=firstValue&second=secondValue")]
    public async void ToFormUrlEncodedContent_Should_ReturnFormFormatString(string data, string expected)
    {
        //Act
        var result = await data.ToFormUrlEncodedContent().ReadAsStringAsync();
        //Assert
        Assert.Equal(expected, result);
    }
}
