namespace Chaos.Entities.Schemas.Aisling;

public record BankSchema
{
    public required uint Gold { get; init; }
    public required ICollection<ItemSchema> Items { get; init; } = Array.Empty<ItemSchema>();
}