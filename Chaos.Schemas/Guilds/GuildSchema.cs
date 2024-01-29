using System.Text.Json.Serialization;

namespace Chaos.Schemas.Guilds;

/// <summary>
///     Represents the serializable schema of a guild
/// </summary>
public sealed record GuildSchema
{
    /// <summary>
    ///     The GUID of the guild
    /// </summary>
    [JsonRequired]
    public string Guid { get; set; } = null!;

    /// <summary>
    ///     The name of the guild. This must be unique and match the directory name
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;
}