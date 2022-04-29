using System;
using System.Linq;
using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.DataObjects;
using Chaos.PanelObjects;
using Chaos.WorldObjects.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.WorldObjects;

public class User : Creature
{
    public Bank Bank { get; }
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public Equipment Equipment { get; }
    public int FaceSprite { get; set; }
    public Gender Gender { get; set; }
    public Group? Group { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public IgnoreList IgnoreList { get; }
    public Inventory Inventory { get; }
    public Legend Legend { get; }
    public UserOptions Options { get; }
    public byte[] Portrait { get; set; }
    public string ProfileMessage { get; set; }
    public SkillBook SkillBook { get; }
    public SocialStatus SocialStatus { get; set; }
    public SpellBook SpellBook { get; }
    public TitleList Titles { get; }
    public IWorldClient Client { get; }
    public override UserStatSheet StatSheet { get; }
    public ILogger Logger { get; }
    public UserState UserState { get; set; }
    public bool IsAdmin { get; set; }
    public override CreatureType Type => CreatureType.User;
    public DateTime LastRefresh { get; set; }

    public User(IWorldClient worldClient, string name, ILogger<User> logger)
        : this(name, Gender.Unisex, 0, 0, logger) =>
        Client = worldClient;

    //default user
    public User(string name, Gender gender, int hairStyle, DisplayColor hairColor, ILogger<User> logger)
        : base(name, null!, new Point(), 0)
    {
        Gender = gender;
        BodyColor = BodyColor.White;
        BodySprite = Gender == Gender.Male ? BodySprite.Male : BodySprite.Female;
        HairStyle = hairStyle;
        HairColor = hairColor;
        StatSheet = new UserStatSheet();
        Titles = new TitleList();
        Options = new UserOptions();
        IgnoreList = new IgnoreList();
        Legend = new Legend();
        Bank = new Bank();
        Equipment = new Equipment(logger);
        Inventory = new Inventory(logger);
        SkillBook = new SkillBook(logger);
        SpellBook = new SpellBook(logger);
        Client = null!;
        Portrait = Array.Empty<byte>();
        ProfileMessage = string.Empty;
        Logger = logger;
    }

    public override void OnClicked(User source)
    {
        if (source.Equals(this))
            source.Client.SendSelfProfile();
        else if (IsVisibleTo(source))
            source.Client.SendProfile(this);
    }


    public override async ValueTask OnUpdated(TimeSpan delta) => await base.OnUpdated(delta).ConfigureAwait(false);

    public override void GoldDroppedOn(int amount, User source)
    {
        //TODO: open exchange
    }

    public override void ItemDroppedOn(byte slot, int count, User source)
    {
        //TODO: open exchange
    }

    public bool CanCarry(params Item[] items)
    {
        if (!items.Any())
            return false;
        
        var weightSum = items.Sum(item => item.Weight);

        if (StatSheet.CurrentWeight + weightSum > StatSheet.MaxWeight)
            return false;

        if (Inventory.AvailableSlots < items.Length)
            return false;

        return true;
    }
}