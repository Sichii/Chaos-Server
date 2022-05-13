using System.Collections.Generic;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;

namespace Chaos.Objects.Serializable;

public record SerializableUser
{
    public SerializableBank Bank { get; set; } = new();
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public Direction Direction { get; set; }
    public ICollection<SerializableEffect> Effects { get; set; } = new List<SerializableEffect>();
    public ICollection<SerialiableEquipment> Equipment { get; set; } = new List<SerialiableEquipment>();
    public int FaceSprite { get; set; }
    public int GamePoints { get; set; }
    public Gender Gender { get; set; }
    public int Gold { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public ICollection<string> IgnoreICollection { get; set; } = new List<string>();
    public ICollection<SerializableItem> Inventory { get; set; } = new List<SerializableItem>();
    public ICollection<SerializableLegendMark> Legend { get; set; } = new List<SerializableLegendMark>();
    public string MapInstanceId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Nation Nation { get; set; }
    public SerializableOptions Options { get; set; } = new();
    public Point Point { get; set; }
    public ICollection<SerializableSkill> SkillBook { get; set; } = new List<SerializableSkill>();
    public ICollection<SerializableSpell> SpellBook { get; set; } = new List<SerializableSpell>();
    public UserStatSheet StatSheet { get; set; } = new();
    public ICollection<string> Titles { get; set; } = new List<string>();
}