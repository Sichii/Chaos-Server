namespace Chaos.Models.Data;

public sealed record LootDrop
{
    public required int DropChance { get; init; }
    public required string ItemTemplateKey { get; init; }
}