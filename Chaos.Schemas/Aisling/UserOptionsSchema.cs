using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of the object that contains all user options
/// </summary>
public sealed record UserOptionsSchema
{
    /// <summary>
    ///     Whether or not the player is accepting exchanges.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Exchange { get; set; }
    /// <summary>
    ///     Not current used rly
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool FastMove { get; set; }
    /// <summary>
    ///     Whether or not the player is accepting group invites.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Group { get; set; }
    /// <summary>
    ///     Whether or not the player wishes to see guild chat
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool GuildChat { get; set; }
    /// <summary>
    ///     Whether or not the player wishes to see animations
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Magic { get; set; }
    /// <summary>
    ///     Whether or not the player wishes to see shouts
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Shout { get; set; }
    /// <summary>
    ///     Whether or not the player wishes to see whispers
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Whisper { get; set; }
    /// <summary>
    ///     Whether or not the player wishes to hear sounds
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Wisdom { get; set; }
}