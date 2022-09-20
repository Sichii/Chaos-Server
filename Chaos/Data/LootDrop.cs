namespace Chaos.Data;

public record LootDrop
{
    public required int DropChance { get; init; }
    public required string ItemTemplateKey { get; init; }
}