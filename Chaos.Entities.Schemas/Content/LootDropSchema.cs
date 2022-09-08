using Chaos.Entities.Schemas.Aisling;

namespace Chaos.Entities.Schemas.Content;

public record LootDropSchema
{
    public required string ItemTemplateKey { get; init; }
    public required int DropChance { get; init; }
}