namespace Chaos.Models.Data;

public sealed class AbilityRequirement
{
    public required byte? Level { get; init; }
    public required string TemplateKey { get; init; }
}