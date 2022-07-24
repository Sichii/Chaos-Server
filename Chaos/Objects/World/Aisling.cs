using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Containers.Interfaces;
using Chaos.Data;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects.World;

public class Aisling : Creature
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
    public bool Loading { get; set; } = true;
    public byte[] Portrait { get; set; }
    public string ProfileMessage { get; set; }
    public SocialStatus SocialStatus { get; set; }
    public UserState UserState { get; set; }
    public ActiveObject ActiveObject { get; }
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
    public override CreatureType Type { get; init; }

    public Aisling(IWorldClient worldClient, string name)
        : this(name)
    {
        Client = worldClient;
        worldClient.Aisling = this;
    }

    //default user
    public Aisling(
        string name,
        Gender gender,
        int hairStyle,
        DisplayColor hairColor
    )
        : this(name)
    {
        Name = name;
        Gender = gender;
        Type = CreatureType.User;
        BodyColor = BodyColor.White;
        BodySprite = Gender == Gender.Male ? BodySprite.Male : BodySprite.Female;
        HairStyle = hairStyle;
        HairColor = hairColor;
        StatSheet = UserStatSheet.NewCharacter;
    }

    public Aisling(string name)
        : base(name)
    {
        StatSheet = new UserStatSheet();
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

    /*
    public override void OnGoldDroppedOn(int amount, User source)
    {
        if (!TryStartExchange(source, out var exchange))
            return;

        exchange.SetGold(source, amount);
    }

    public override void OnItemDroppedOn(byte slot, byte count, User source)
    {
        if (!TryStartExchange(source, out var exchange))
            return;

        if (count == 0)
            exchange.AddItem(source, slot);
        else
            exchange.AddStackableItem(this, slot, count);
    }

    public override void OnClicked(User source)
    {
        if (source.Equals(this))
            source.Client.SendSelfProfile();
        else if (IsVisibleTo(source))
            source.Client.SendProfile(this);
    }
    */

    /*private bool TryStartExchange(User source, [MaybeNullWhen(false)] out Exchange exchange)
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
            ActiveObject.TryRemove(exchange);
            source.Client.SendServerMessage(ServerMessageType.ActiveMessage, "You're already busy");

            exchange = null;

            return false;
        }

        exchange.Activate();

        return true;
    }*/

    public override void Update(TimeSpan delta)
    {
        Equipment.Update(delta);
        Inventory.Update(delta);
        SkillBook.Update(delta);
        SpellBook.Update(delta);
        base.Update(delta);
    }
}