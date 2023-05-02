namespace Chaos.Data;

public sealed class ItemRequirement
{
    public required int AmountRequired { get; init; }
    public required string ItemTemplateKey { get; init; }
}