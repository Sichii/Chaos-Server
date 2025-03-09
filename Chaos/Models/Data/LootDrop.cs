namespace Chaos.Models.Data;

public sealed record LootDrop
{
    public required decimal DropChance { get; init; }
    public ICollection<string> ExtraScriptKeys { get; set; } = Array.Empty<string>();
    public required string ItemTemplateKey { get; init; }
}