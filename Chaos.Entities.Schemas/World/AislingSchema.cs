using Chaos.Common.Definitions;
using Chaos.Geometry.Definitions;

namespace Chaos.Entities.Schemas.World;

public record AislingSchema
{
    public ICollection<ItemSchema> Bank { get; init; } = Array.Empty<ItemSchema>();
    public uint BankedGold { get; init; }
    public BodyColor BodyColor { get; init; }
    public BodySprite BodySprite { get; init; }
    public Direction Direction { get; init; }
    public ICollection<EffectSchema> Effects { get; init; } = Array.Empty<EffectSchema>();
    public ICollection<ItemSchema> Equipment { get; init; } = Array.Empty<ItemSchema>();
    public int FaceSprite { get; init; }
    public int GamePoints { get; init; }
    public Gender Gender { get; init; }
    public int Gold { get; init; }
    public string? GuildName { get; init; }
    public string? GuildTitle { get; init; }
    public DisplayColor HairColor { get; init; }
    public int HairStyle { get; init; }
    public ICollection<string> IgnoreList { get; init; } = Array.Empty<string>();
    public ICollection<ItemSchema> Inventory { get; init; } = Array.Empty<ItemSchema>();
    public ICollection<LegendMarkSchema> Legend { get; init; } = Array.Empty<LegendMarkSchema>();
    public string MapInstanceId { get; init; } = null!;
    public string Name { get; init; } = null!;
    public Nation Nation { get; init; }
    public ICollection<SkillSchema> SkillBook { get; init; } = Array.Empty<SkillSchema>();
    public ICollection<SpellSchema> SpellBook { get; init; } = Array.Empty<SpellSchema>();
    public UserStatSheetSchema StatSheet { get; init; } = null!;
    public ICollection<string> Titles { get; init; } = Array.Empty<string>();
    public UserOptionsSchema UserOptions { get; init; } = null!;
    public int X { get; init; }
    public int Y { get; init; }
}