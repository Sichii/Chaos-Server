using Chaos.Core.Utilities;
using Chaos.Data;
using Chaos.Geometry.Definitions;
using Chaos.Objects.World;

namespace Chaos.Objects.Serializable;

public record SerializableAisling
{
    public ICollection<SerializableItem> Bank { get; init; }
    public uint BankedGold { get; init; }
    public BodyColor BodyColor { get; init; }
    public BodySprite BodySprite { get; init; }
    public Direction Direction { get; init; }
    public ICollection<SerializableEffect> Effects { get; init; }
    public ICollection<SerializableItem> Equipment { get; init; }
    public int FaceSprite { get; init; }
    public int GamePoints { get; init; }
    public Gender Gender { get; init; }
    public int Gold { get; init; }
    public string? GuildName { get; init; }
    public string? GuildTitle { get; init; }
    public DisplayColor HairColor { get; init; }
    public int HairStyle { get; init; }
    public ICollection<string> IgnoreList { get; init; }
    public ICollection<SerializableItem> Inventory { get; init; }
    public ICollection<SerializableLegendMark> Legend { get; init; }
    public string MapInstanceId { get; init; }
    public string Name { get; init; }
    public Nation Nation { get; init; }
    public SerializableOptions Options { get; init; }
    public ICollection<SerializableSkill> SkillBook { get; init; }
    public ICollection<SerializableSpell> SpellBook { get; init; }
    public UserStatSheet StatSheet { get; init; }
    public ICollection<string> Titles { get; init; }
    public int X { get; init; }
    public int Y { get; init; }

    #pragma warning disable CS8618
    //json constructor
    public SerializableAisling() { }
    #pragma warning restore CS8618

    public SerializableAisling(Aisling aisling)
    {
        MapInstanceId = aisling.MapInstance.InstanceId;
        BankedGold = aisling.Bank.Gold;
        BodyColor = aisling.BodyColor;
        BodySprite = aisling.BodySprite;
        Direction = aisling.Direction;
        FaceSprite = aisling.FaceSprite;
        GamePoints = aisling.GamePoints;
        Gender = aisling.Gender;
        Gold = aisling.Gold;
        GuildName = aisling.GuildName;
        GuildTitle = aisling.GuildTitle;
        HairColor = aisling.HairColor;
        HairStyle = aisling.HairStyle;
        Name = aisling.Name;
        Nation = aisling.Nation;
        X = aisling.X;
        Y = aisling.Y;

        StatSheet = ShallowCopy<UserStatSheet>.Clone(aisling.StatSheet);
        Titles = aisling.Titles.ToList();
        Options = new SerializableOptions(aisling.Options);
        IgnoreList = aisling.IgnoreList.ToList();

        Legend = aisling.Legend
                        .Select(lm => new SerializableLegendMark(lm))
                        .ToList();

        Bank = aisling.Bank
                      .Select(i => new SerializableItem(i))
                      .ToList();

        Equipment = aisling.Equipment
                           .Select(i => new SerializableItem(i))
                           .ToList();

        Inventory = aisling.Inventory
                           .Select(i => new SerializableItem(i))
                           .ToList();

        SkillBook = aisling.SkillBook
                           .Select(s => new SerializableSkill(s))
                           .ToList();

        SpellBook = aisling.SpellBook
                           .Select(s => new SerializableSpell(s))
                           .ToList();

        Effects = aisling.Effects
                         .Select(e => new SerializableEffect(e))
                         .ToList();
    }
}