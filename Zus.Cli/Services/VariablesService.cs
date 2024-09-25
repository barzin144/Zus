using Zus.Cli.Models;

namespace Zus.Cli.Services;

public class VariablesService : IVariablesService
{
    private readonly IFileService<LocalVariable> _fileService;

    public VariablesService(IFileService<LocalVariable> fileService)
    {
        _fileService = fileService;
    }
    public async Task DeleteAsync(string id)
    {
        await _fileService.DeleteAsync(id);
    }

    public async Task<string> GetAsync()
    {
        return await _fileService.GetAsync();
    }

    public async Task<List<LocalVariable>> GetDeserializeAsync()
    {
        return await _fileService.GetDeserializeAsync();
    }

    public async Task SaveAsync(LocalVariable data, bool overwrite)
    {
        await _fileService.SaveAsync(data, overwrite);
    }
}

public interface IVariablesService
{
    public Task SaveAsync(LocalVariable data, bool overwrite);
    public Task<List<LocalVariable>> GetDeserializeAsync();
    public Task<string> GetAsync();
    public Task DeleteAsync(string id);
}
