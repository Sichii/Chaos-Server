using System.Text.Json.Serialization;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Content;

/// <summary>
///     Represents the serializable schema of a merchant spawn
/// </summary>
public sealed record MerchantSpawnSchema
{
    /// <summary>
    ///     A collection of points that merchants created by this spawn will not spawn or wander on
    /// </summary>
    public ICollection<IPoint> BlackList { get; set; } = Array.Empty<IPoint>();

    /// <summary>
    ///     Default to null, causing merchants to spawn facing random directions<br />If specified, will spawn merchants facing in the specified direction
    /// </summary>
    public Direction? Direction { get; set; }

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