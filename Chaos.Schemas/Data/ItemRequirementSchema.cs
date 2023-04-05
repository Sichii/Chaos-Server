using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

/// <summary>
///     Represents the serializable schema of the requirements of an item needed to learn an ability
/// </summary>
public sealed record ItemRequirementSchema
{
    /// <summary>
    ///     The amount of this item required to learn the ability
    /// </summary>
    public int AmountRequired { get; set; } = 1;
    /// <summary>
    ///     The key of the item template required to learn the ability
    /// </summary>
    [JsonRequired]
    public string ItemTemplateKey { get; set; } = null!;
}