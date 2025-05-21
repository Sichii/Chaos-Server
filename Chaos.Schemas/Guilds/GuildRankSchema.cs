#region
using System.Text.Json.Serialization;
#endregion

namespace Chaos.Schemas.Guilds;

/// <summary>
///     Represents the serializable schema of a guild rank
/// </summary>
public sealed record GuildRankSchema
{
    /// <summary>
    ///     The members of this rank
    /// </summary>
    public ICollection<string> Members { get; set; } = [];

    /// <summary>
    ///     The name of the rank. This must be unique within the guild and match the file name
    /// </summary>
    [JsonRequired]
    public string RankName { get; set; } = null!;

    /// <summary>
    ///     The tier of the rank
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public int Tier { get; set; }
}