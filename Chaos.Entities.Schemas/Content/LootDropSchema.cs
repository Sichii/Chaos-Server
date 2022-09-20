namespace Chaos.Entities.Schemas.Content;

public record LootDropSchema
{
    /// <summary>
    ///     A unique id specific to the template of the item that should drop
    /// </summary>
    public required int DropChance { get; init; }
    /// <summary>
    ///     The chance of the item to drop
    /// </summary>
    public required string ItemTemplateKey { get; init; }
}