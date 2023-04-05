using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of an effect
/// </summary>
public sealed record EffectSchema
{
    /// <summary>
    ///     The key of the effect
    /// </summary>
    [JsonRequired]
    public string EffectKey { get; set; } = null!;
    /// <summary>
    ///     The amount of time in seconds that has elapsed towards the duration of this effect
    /// </summary>
    public int RemainingSecs { get; set; }
}