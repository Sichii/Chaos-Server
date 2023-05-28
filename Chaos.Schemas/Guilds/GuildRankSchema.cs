using System.Text.Json.Serialization;

namespace Chaos.Schemas.Guilds;

/// <summary>
///     Represents the serializable schema of a guild rank
/// </summary>
public sealed record GuildRankSchema
{
    /// <summary>
    ///     The members of this rank
    /// </summary>
    public ICollection<string> Members { get; init; } = Array.Empty<string>();
    /// <summary>
    ///     The name of the rank. This must be unique within the guild and match the file name
    /// </summary>
    [JsonRequired]
    public string RankName { get; init; } = null!;

    /// <summary>
    ///     The tier of the rank
    /// </summary>
    public int Tier { get; init; }
}