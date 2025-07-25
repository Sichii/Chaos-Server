namespace Chaos.Models.Data;

public sealed record LootDrop
{
    public required decimal DropChance { get; init; }
    public ICollection<string> ExtraScriptKeys { get; set; } = [];
    public required string ItemTemplateKey { get; init; }
    public int MaxAmount { get; init; }
    public int MinAmount { get; init; }
}