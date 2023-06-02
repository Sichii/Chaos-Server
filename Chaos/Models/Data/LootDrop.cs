namespace Chaos.Models.Data;

public sealed record LootDrop
{
    public required decimal DropChance { get; init; }
    public required string ItemTemplateKey { get; init; }
}