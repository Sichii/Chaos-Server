namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of an Aisling's bank
/// </summary>
public sealed record BankSchema
{
    /// <summary>
    ///     The amount of gold in the bank
    /// </summary>
    public uint Gold { get; set; }

    /// <summary>
    ///     A collection of items in the bank
    /// </summary>
    public ICollection<ItemSchema> Items { get; set; } = Array.Empty<ItemSchema>();
}