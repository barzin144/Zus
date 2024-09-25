using System.Data;
using Zus.Cli.Helpers;
using Zus.Cli.Models;
using Zus.Cli.Services;

namespace Zus.Cli.Commands;

public class ManageVariables
{
    private readonly IVariablesService _variableService;

    public ManageVariables(IVariablesService variablesService)
    {
        _variableService = variablesService;
    }
    internal async Task<CommandResult> DeleteAsync(string name)
    {
        var retypedName = Display.ConfirmMessage("Retype the name to confirm: ");
        if (retypedName == name)
        {
            await _variableService.DeleteAsync(name);
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
        return await _variableService.GetDeserializeAsync();
    }

    internal async Task<CommandResult> ListAsync()
    {
        string variables = await _variableService.GetAsync();
        return new CommandResult
        {
            Result = variables
        };
    }

    internal async Task<CommandResult> SaveAsync(LocalVariable variable, bool force)
    {
        try
        {
            await _variableService.SaveAsync(variable, force);
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
