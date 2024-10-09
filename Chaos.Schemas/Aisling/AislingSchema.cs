using System.Text.Json.Serialization;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of an Aisling
/// </summary>
public sealed record AislingSchema
{
    /// <summary>
    ///     The color of the Aisling's body
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BodyColor BodyColor { get; set; }

    /// <summary>
    ///     The sprite of the Aisling's body
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BodySprite BodySprite { get; set; }

    /// <summary>
    ///     The aislings settings for the channels that they are in
    /// </summary>
    public ICollection<ChannelSettingsSchema> ChannelSettings { get; set; } = Array.Empty<ChannelSettingsSchema>();

    /// <summary>
    ///     The direction the aisling is facing
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Direction Direction { get; set; }

    /// <summary>
    ///     The sprite id of the aisling's face
    /// </summary>
    public int FaceSprite { get; set; }

    /// <summary>
    ///     If the aisling is in an instance, this is the location they will be teleported to if they log in and the instance
    ///     no longer exists
    /// </summary>
    public Location? FallbackLocation { get; set; }

    /// <summary>
    ///     The amount of game points the aisling has
    /// </summary>
    public int GamePoints { get; set; }

    /// <summary>
    ///     The gender of the aisling
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Gender Gender { get; set; }

    /// <summary>
    ///     The amount of gold the aisling has
    /// </summary>
    public int Gold { get; set; }

    /// <summary>
    ///     The name of the guild the aisling is in, if any
    /// </summary>
    public string? GuildName { get; set; }

    /// <summary>
    ///     The color of the aisling's hair
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DisplayColor HairColor { get; set; }

    /// <summary>
    ///     The sprite id of the aisling's hair
    /// </summary>
    public int HairStyle { get; set; }

    /// <summary>
    ///     A collection of names of other aislings that this aisling has ignored
    /// </summary>
    public ICollection<string> IgnoreList { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     Whether or not this aisling has admin privileges
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    ///     Whether or not this aisling is dead
    /// </summary>
    public bool IsDead { get; set; }

    /// <summary>
    ///     The instance id of the map the aisling is currently in
    /// </summary>
    [JsonRequired]
    public string MapInstanceId { get; set; } = null!;

    /// <summary>
    ///     The aisling's name
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The nation the aisling is from
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Nation Nation { get; set; }

    /// <summary>
    ///     The aisling's general stats
    /// </summary>
    [JsonRequired]
    public UserStatSheetSchema StatSheet { get; set; } = null!;

    /// <summary>
    ///     A collecti of titles the aisling has
    /// </summary>
    public ICollection<string> Titles { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The aisling's user options
    /// </summary>
    [JsonRequired]
    public UserOptionsSchema UserOptions { get; set; } = null!;

    /// <summary>
    ///     The X coordinate of the aisling's location
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     The Y coordinate of the aisling's location
    /// </summary>
    public int Y { get; set; }
}