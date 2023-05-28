using System.Text.Json.Serialization;

namespace Chaos.Schemas.Guilds;

/// <summary>
///     Represents the serializable schema of a guild
/// </summary>
public sealed record GuildSchema
{
    /// <summary>
    ///     The name of the guild. This must be unique and match the directory name
    /// </summary>
    [JsonRequired]
    public string Name { get; init; } = null!;
}