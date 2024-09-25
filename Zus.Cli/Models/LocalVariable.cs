using Zus.Cli.Services;

namespace Zus.Cli.Models;

public class LocalVariable : IData
{
    public LocalVariable(string name, string value)
    {
        Name = name;
        Value = value;
    }
    public string Name { get; set; }
    public string Value { get; set; }
    public string Id { get => Name ?? string.Empty; set => Name = value; }
}
