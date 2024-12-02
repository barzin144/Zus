using Zus.Cli.Models;

namespace Zus.Cli.Test.Models;

public class RequestTests
{
    [Fact]
    public void Request_Should_SetIdAsName()
    {
        //Arrange
        Request target = new("http://test.com", null, RequestMethod.Get, "");
        target.Name = "Test";
        //Assert
        Assert.Equal("Test", target.Id);
    }

    [Fact]
    public void Request_Should_SetIdEmpty_When_NameIsNull()
    {
        //Arrange
        Request target = new("http://test.com", null, RequestMethod.Get);
        //Assert
        Assert.Equal(string.Empty, target.Id);
    }
}
