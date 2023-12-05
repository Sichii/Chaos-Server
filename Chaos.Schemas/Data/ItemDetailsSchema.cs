using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

/// <summary>
///     Represents the serializable schema of the details of an item being sold by a merchant
/// </summary>
public sealed record ItemDetailsSchema
{
    /// <summary>
    ///     The key of the item template being sold
    /// </summary>
    [JsonRequired]
    public string ItemTemplateKey { get; set; } = null!;

    /// <summary>
    ///     The amount of this item the merchant has in stock
    /// </summary>
    [JsonRequired]
    public int Stock { get; set; }
}