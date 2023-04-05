using System.Text.Json.Serialization;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Content;

/// <summary>
///     Represents the serializable schema of a merchant spawn
/// </summary>
public sealed record MerchantSpawnSchema
{
    /// <summary>
    ///     The direction the merchant will be facing when spawned
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Direction Direction { get; set; }

    /// <summary>
    ///     A collection of extra merchant script keys to add to the monsters created by this spawn
    /// </summary>
    public ICollection<string> ExtraScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The unique id for the template of the merchant to spawn
    /// </summary>
    [JsonRequired]
    public string MerchantTemplateKey { get; set; } = null!;

    /// <summary>
    ///     The point on ths map where the merchant will spawn
    /// </summary>
    public Point SpawnPoint { get; set; }
}