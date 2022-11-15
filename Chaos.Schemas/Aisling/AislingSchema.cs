using System.Text.Json.Serialization;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record AislingSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BodyColor BodyColor { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BodySprite BodySprite { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Direction Direction { get; init; }
    public ICollection<EffectSchema> Effects { get; init; } = Array.Empty<EffectSchema>();
    public int FaceSprite { get; init; }
    [JsonRequired]
    public FlagCollection Flags { get; init; } = null!;
    public int GamePoints { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Gender Gender { get; init; }
    public int Gold { get; init; }
    public string? GuildName { get; init; }
    public string? GuildTitle { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DisplayColor HairColor { get; init; }
    public int HairStyle { get; init; }
    public ICollection<string> IgnoreList { get; init; } = Array.Empty<string>();
    public bool IsAdmin { get; init; }
    [JsonRequired]
    public string MapInstanceId { get; init; } = null!;
    [JsonRequired]
    public string Name { get; init; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Nation Nation { get; init; }
    [JsonRequired]
    public UserStatSheetSchema StatSheet { get; init; } = null!;
    public ICollection<string> Titles { get; init; } = Array.Empty<string>();
    [JsonRequired]
    public UserOptionsSchema UserOptions { get; init; } = null!;
    public int X { get; init; }
    public int Y { get; init; }
}