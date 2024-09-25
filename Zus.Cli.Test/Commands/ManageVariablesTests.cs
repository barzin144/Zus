using System.Data;
using Zus.Cli.Commands;
using Zus.Cli.Models;
using Zus.Cli.Services;

namespace Zus.Cli.Test.Commands;

public class ManageVariablesTests
{
    private readonly Mock<IVariablesService> _mockVariablesService;
    private readonly ManageVariables _target;

    public ManageVariablesTests()
    {
        _mockVariablesService = new Mock<IVariablesService>();
        _target = new ManageVariables(_mockVariablesService.Object);
    }

    [Fact]
    public async void ListAsync_Should_CallFileGetAsync()
    {
        //Act
        var result = await _target.ListAsync();
        //Assert
        _mockVariablesService.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async void DeleteAsync_Should_CallDelete_When_RetypedNameIsMatch()
    {
        //Arrange
        string variableName = "variable_name";
        _mockVariablesService.Setup(x => x.DeleteAsync(variableName));
        using StringWriter output = new();
        Console.SetOut(output);
        using StringReader input = new(variableName);
        Console.SetIn(input);
        //Act
        var result = await _target.DeleteAsync(variableName);
        //Assert
        _mockVariablesService.Verify(x => x.DeleteAsync(variableName), Times.Once);
        Assert.NotNull(result.Result);
        Assert.Null(result.Error);
        Assert.True(result.Success);
    }

    [Fact]
    public async void DeleteAsync_Should_ReturnError_When_RetypedNameIsNotMatch()
    {
        //Arrange
        string variableName = "variable_name";
        _mockVariablesService.Setup(x => x.DeleteAsync(variableName));
        using StringWriter output = new();
        Console.SetOut(output);
        using StringReader input = new("retyped_name");
        Console.SetIn(input);
        //Act
        var result = await _target.DeleteAsync(variableName);
        //Assert
        _mockVariablesService.Verify(x => x.DeleteAsync(variableName), Times.Never);
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
    }

    [Fact]
    public async void SaveAsync_Should_SaveVariable()
    {
        //Arrange
        LocalVariable variable = new LocalVariable("name", "value");
        _mockVariablesService.Setup(x => x.SaveAsync(It.IsAny<LocalVariable>(), It.IsAny<bool>()));
        //Act
        var result = await _target.SaveAsync(variable, true);
        //Assert
        _mockVariablesService.Verify(x => x.SaveAsync(It.IsAny<LocalVariable>(), true), Times.Once);
        Assert.Null(result.Error);
        Assert.NotNull(result.Result);
        Assert.True(result.Success);
    }

    [Fact]
    public async void SaveAsync_Should_ThrowException_When_NameIsDuplicate()
    {
        //Arrange
        LocalVariable variable = new LocalVariable("name", "value");

        _mockVariablesService.Setup(x => x.SaveAsync(It.IsAny<LocalVariable>(), false)).ThrowsAsync(new DuplicateNameException());

        //Act
        var result = await _target.SaveAsync(variable, false);

        //Assert
        Assert.NotNull(result.Error);
        Assert.Null(result.Result);
        Assert.False(result.Success);
        _mockVariablesService.Verify(x => x.SaveAsync(It.IsAny<LocalVariable>(), false), Times.Once);
    }
}
