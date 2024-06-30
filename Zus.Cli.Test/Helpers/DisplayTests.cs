
using Zus.Cli.Helpers;
using Zus.Cli.Models;

namespace Zus.Cli.Test.Helpers;

public class DisplayTests
{
    [Fact]
    public void Result_Should_WriteErrorTextInConsole()
    {
        //Arrange
        using StringWriter output = new();
        Console.SetOut(output);

        CommandResult result = new()
        {
            Error = "Error"
        };
        //Act
        Display.Result(result);
        //Assert
        Assert.Equal("Error\r\n", output.ToString());
    }

    [Fact]
    public void Result_Should_WriteResultTextInConsole()
    {
        //Arrange
        using StringWriter output = new();
        Console.SetOut(output);

        CommandResult result = new()
        {
            Result = "Success"
        };
        //Act
        Display.Result(result);
        //Assert
        Assert.Equal("Success\r\n", output.ToString());
    }

    [Fact]
    public void ConfirmMessage_Should_WriteMessageInConsoleAndReturnUserInput()
    {
        //Arrange
        using StringWriter output = new();
        Console.SetOut(output);

        using StringReader input = new("Confirm");
        Console.SetIn(input);

        //Act
        var userInput = Display.ConfirmMessage("Confirm Message");
        //Assert
        Assert.Equal("Confirm Message", output.ToString());
        Assert.Equal("Confirm", userInput);
    }
}
