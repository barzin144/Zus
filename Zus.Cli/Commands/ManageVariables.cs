using System.Data;
using Zus.Cli.Helpers;
using Zus.Cli.Models;
using Zus.Cli.Services;

namespace Zus.Cli.Commands;

public class ManageVariables
{
    private readonly IFileService<LocalVariable> _fileService;

    public ManageVariables(IFileService<LocalVariable> fileService)
    {
        _fileService = fileService;
    }
    internal async Task<CommandResult> DeleteAsync(string name)
    {
        var retypedName = Display.ConfirmMessage("Retype the name to confirm: ");
        if (retypedName == name)
        {
            await _fileService.DeleteAsync(name);
            return new CommandResult
            {
                Result = $"Variable {name} has been deleted."
            };
        }
        else
        {
            return new CommandResult
            {
                Error = "The names do not match, the variable has not been deleted."
            };

        }
    }

    internal async Task<List<LocalVariable>> GetAsync()
    {
        return await _fileService.GetDeserializeAsync();
    }

    internal async Task<CommandResult> ListAsync()
    {
        string variables = await _fileService.GetAsync();
        return new CommandResult
        {
            Result = variables
        };
    }

    internal async Task<CommandResult> SaveAsync(LocalVariable variable, bool force)
    {
        try
        {
            await _fileService.SaveAsync(variable, force);
            return new CommandResult
            {
                Result = $"Variable {variable.Name} has been saved."
            };
        }
        catch (DuplicateNameException)
        {
            return new CommandResult { Error = $"Error: A variable with the name '{variable.Name}' already exists. To overwrite the existing variable, please use the '-f' flag" };
        }
        catch (Exception ex)
        {
            return new CommandResult { Error = ex.Message };
        }
    }
}
