using Zus.Cli.Services;

namespace Zus.Cli.Test.Services;

public class ServiceFactoryTests
{
    [Fact]
    public void GetSendRequest()
    {
        //Act & Assert
        Assert.NotNull(ServiceFactory.GetSendRequest());
    }
}
