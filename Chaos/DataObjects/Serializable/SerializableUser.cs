using System.Collections.Generic;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;

namespace Chaos.DataObjects.Serializable;

public record SerializableUser
{
    public int Ability { get; set; }
    public int Ac { get; set; }
    public AdvClass AdvClass { get; set; }
    public int AttackSpeed { get; set; }
    public SerializableBank Bank { get; set; } = new();
    public BaseClass BaseClass { get; set; }
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public int Con { get; set; }
    public int CooldownReduction { get; set; }
    public int CurrentHp { get; set; }
    public int CurrentMp { get; set; }
    public int Dex { get; set; }
    public Direction Direction { get; set; }
    public int Dmg { get; set; }
    public List<SerializableEffect> Effects { get; set; } = new();
    public List<SerialiableEquipment> Equipment { get; set; } = new();
    public int FaceSprite { get; set; }
    public int GamePoints { get; set; }
    public Gender Gender { get; set; }
    public int Gold { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public int Hit { get; set; }
    public List<string> IgnoreList { get; set; } = new();
    public int Int { get; set; }
    public List<SerializableItem> Inventory { get; set; } = new();
    public List<SerializableLegendMark> Legend { get; set; } = new();
    public int Level { get; set; }
    public int MagicResistance { get; set; }
    public string MapInstanceId { get; set; } = null!;
    public bool Master { get; set; }
    public int MaximumHp { get; set; }
    public int MaximumMp { get; set; }
    public int MaxWeight { get; set; }
    public string Name { get; set; } = null!;
    public Nation Nation { get; set; }
    public SerializableOptions Options { get; set; } = new();
    public Point Point { get; set; }
    public List<SerializableSkill> SkillBook { get; set; } = new();
    public List<SerializableSpell> SpellBook { get; set; } = new();
    public int Str { get; set; }
    public List<string> Titles { get; set; } = new();
    public int ToNextAbility { get; set; }
    public int ToNextLevel { get; set; }
    public int TotalAbility { get; set; }
    public int TotalExp { get; set; }
    public int UnspentPoints { get; set; }
    public int Wis { get; set; }
}