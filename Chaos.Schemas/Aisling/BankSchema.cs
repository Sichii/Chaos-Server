namespace Chaos.Schemas.Aisling;

public sealed record BankSchema
{
    public uint Gold { get; init; }
    public ICollection<ItemSchema> Items { get; init; } = Array.Empty<ItemSchema>();
}