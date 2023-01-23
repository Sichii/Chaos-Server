using System.Text.Json.Serialization;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record AislingSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BodyColor BodyColor { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BodySprite BodySprite { get; set; }
    public IDictionary<string, int> Counters { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Direction Direction { get; set; }
    public ICollection<EffectSchema> Effects { get; set; } = Array.Empty<EffectSchema>();
    [JsonRequired]
    public EnumCollection Enums { get; set; } = null!;
    public int FaceSprite { get; set; }
    public Location? FallbackLocation { get; set; }
    [JsonRequired]
    public FlagCollection Flags { get; set; } = null!;
    public int GamePoints { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Gender Gender { get; set; }
    public int Gold { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public ICollection<string> IgnoreList { get; set; } = Array.Empty<string>();
    public bool IsAdmin { get; set; }
    public bool IsDead { get; set; }
    [JsonRequired]
    public string MapInstanceId { get; set; } = null!;
    [JsonRequired]
    public string Name { get; set; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Nation Nation { get; set; }
    [JsonRequired]
    public UserStatSheetSchema StatSheet { get; set; } = null!;
    public ICollection<string> Titles { get; set; } = Array.Empty<string>();
    [JsonRequired]
    public UserOptionsSchema UserOptions { get; set; } = null!;
    public int X { get; set; }
    public int Y { get; set; }
}