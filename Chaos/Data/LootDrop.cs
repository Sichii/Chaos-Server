using Chaos.Objects.Panel;

namespace Chaos.Data;

public record LootDrop
{
    public required string ItemTemplateKey { get; init; }
    public required int DropChance { get; init; }
}