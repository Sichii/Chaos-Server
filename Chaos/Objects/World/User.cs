using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Containers.Interfaces;
using Chaos.Core.Data;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World;

public class User : Creature
{
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public int FaceSprite { get; set; }
    public Gender Gender { get; set; }
    public Group? Group { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime LastRefresh { get; set; }
    public byte[] Portrait { get; set; }
    public string ProfileMessage { get; set; }
    public SocialStatus SocialStatus { get; set; }
    public UserState UserState { get; set; }
    public Bank Bank { get; }
    public IWorldClient Client { get; }
    public IEquipment Equipment { get; }
    public IgnoreList IgnoreList { get; }
    public IInventory Inventory { get; }
    public Legend Legend { get; }
    public UserOptions Options { get; }
    public IPanel<Skill> SkillBook { get; }
    public IPanel<Spell> SpellBook { get; }
    public override UserStatSheet StatSheet { get; }
    public TitleList Titles { get; }
    public override CreatureType Type => CreatureType.User;
    private readonly IExchangeFactory ExchangeFactory = null!;
    public ActiveObject ActiveObject { get; }

    public User(IWorldClient worldClient, string name, IExchangeFactory exchangeFactory)
        : this(
            name,
            Gender.Unisex,
            0,
            0)
    {
        Client = worldClient;
        ExchangeFactory = exchangeFactory;
        worldClient.User = this;
        StatSheet = new UserStatSheet();
        ActiveObject = new ActiveObject();
    }

    //default user
    public User(
        string name,
        Gender gender,
        int hairStyle,
        DisplayColor hairColor
    )
        : base(
            name,
            null!,
            new Point(),
            0)
    {
        Gender = gender;
        BodyColor = BodyColor.White;
        BodySprite = Gender == Gender.Male ? BodySprite.Male : BodySprite.Female;
        HairStyle = hairStyle;
        HairColor = hairColor;
        StatSheet = UserStatSheet.NewCharacter;
        Titles = new TitleList();
        Options = new UserOptions();
        IgnoreList = new IgnoreList();
        Legend = new Legend();
        Bank = new Bank();
        Equipment = new Equipment();
        Inventory = new Inventory();
        SkillBook = new SkillBook();
        SpellBook = new SpellBook();
        ActiveObject = new ActiveObject();
        Client = null!;
        Portrait = Array.Empty<byte>();
        ProfileMessage = string.Empty;

        /*
         *         Nation = Nation.None;
        MaxWeight = 40;
        _toNextLevel = 100;
        _str = 1;
        _int = 1;
        _wis = 1;
        _con = 1;
        _dex = 1;
        _currentHp = 100;
        _maximumHp = 100;
        _currentMp = 50;
        _maximumMp = 50;
        _level = 1;
         */
    }

    public bool CanCarry(params Item[] items)
    {
        if (!items.Any())
            return false;

        var weightSum = items.Sum(item => item.Template.Weight);

        if (StatSheet.CurrentWeight + weightSum > StatSheet.MaxWeight)
            return false;

        if (Inventory.AvailableSlots < items.Length)
            return false;

        return true;
    }

    public void GiveGold(int amount)
    {
        Gold += amount;
        Client.SendAttributes(StatUpdateType.ExpGold);
    }

    public override void GoldDroppedOn(int amount, User source)
    {
        if (!TryStartExchange(source, out var exchange))
            return;

        exchange.SetGold(source, amount);
    }

    public override void ItemDroppedOn(byte slot, byte count, User source)
    {
        if (!TryStartExchange(source, out var exchange))
            return;
        
        if(count == 0)
            exchange.AddItem(source, slot);
        else
            exchange.AddStackableItem(this, slot, count);
    }

    private bool TryStartExchange(User source, [MaybeNullWhen(false)] out Exchange exchange)
    {
        exchange = ExchangeFactory.CreateExchange(source, this);

        if (!ActiveObject.TrySet(exchange))
        {
            source.Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{Name} is busy right now");
            Client.SendServerMessage(ServerMessageType.ActiveMessage, $"{source.Name} is trying to exchange with you");

            exchange = null;

            return false;
        }

        if (!source.ActiveObject.TrySet(exchange))
        {
            source.ActiveObject.TryRemove(exchange);
            source.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You're already busy");

            exchange = null;

            return false;
        }
        
        exchange.Activate();

        return true;
    }

    public override void OnClicked(User source)
    {
        if (source.Equals(this))
            source.Client.SendSelfProfile();
        else if (IsVisibleTo(source))
            source.Client.SendProfile(this);
    }

    public override void Update(TimeSpan delta) => base.Update(delta);
}