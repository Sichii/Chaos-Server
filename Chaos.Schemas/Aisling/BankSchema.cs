namespace Chaos.Schemas.Aisling;

public sealed record BankSchema
{
    public uint Gold { get; set; }
    public ICollection<ItemSchema> Items { get; set; } = Array.Empty<ItemSchema>();
}