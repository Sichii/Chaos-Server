using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Containers.Abstractions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Observers;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Hosted.Options;
using Microsoft.Extensions.Logging;
using PointExtensions = Chaos.Geometry.Extensions.PointExtensions;

namespace Chaos.Objects.World;

public class Aisling : Creature
{
    private readonly IExchangeFactory ExchangeFactory;
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public IWorldClient Client { get; set; }
    public int FaceSprite { get; set; }
    public Gender Gender { get; set; }
    public Group? Group { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime LastRefresh { get; set; }
    public Nation Nation { get; set; }
    public byte[] Portrait { get; set; }
    public string ProfileText { get; set; }
    public SocialStatus SocialStatus { get; set; }
    public UserState UserState { get; set; }
    public ActiveObject ActiveObject { get; }
    public Bank Bank { get; private set; }
    public IEquipment Equipment { get; private set; }
    public IgnoreList IgnoreList { get; init; }
    public IInventory Inventory { get; private set; }
    public Legend Legend { get; private set; }
    public UserOptions Options { get; init; }
    public IPanel<Skill> SkillBook { get; private set; }
    public IPanel<Spell> SpellBook { get; private set; }
    public override StatSheet StatSheet => UserStatSheet;
    public UserStatSheet UserStatSheet { get; init; }
    public TitleList Titles { get; init; }
    public override CreatureType Type => CreatureType.Aisling;
    protected override ILogger<Aisling> Logger { get; }

    public Aisling(
        string name,
        MapInstance mapInstance,
        IPoint point,
        IExchangeFactory exchangeFactory,
        ILogger<Aisling> logger
    )
        : this(name, mapInstance, point)
    {
        ExchangeFactory = exchangeFactory;
        Logger = logger;
    }

    //default user
    public Aisling(
        string name,
        Gender gender,
        int hairStyle,
        DisplayColor hairColor,
        MapInstance mapInstance,
        IPoint point
    )
        : this(name, mapInstance, point)
    {
        Name = name;
        Gender = gender;
        BodyColor = BodyColor.White;
        BodySprite = Gender == Gender.Male ? BodySprite.Male : BodySprite.Female;
        HairStyle = hairStyle;
        HairColor = hairColor;
        UserStatSheet = UserStatSheet.NewCharacter;
    }

    private Aisling(string name, MapInstance mapInstance, IPoint point)
        : base(
            name,
            0,
            mapInstance,
            point)
    {
        //initialize all the things
        UserStatSheet = new UserStatSheet();
        Titles = new TitleList();
        Options = new UserOptions();
        IgnoreList = new IgnoreList();
        Legend = new Legend();
        Bank = new Bank();
        Equipment = new Equipment();
        Inventory = new Inventory();
        SkillBook = new SkillBook();
        SpellBook = new SpellBook();
        Effects = new EffectsBar(this);
        ActiveObject = new ActiveObject();
        Portrait = Array.Empty<byte>();
        ProfileText = string.Empty;

        //this object is purely intended to be created and immediately serialized
        //these pieces should never come into play
        Client = null!;
        Logger = null!;
        ExchangeFactory = null!;
    }

    public void Initialize(
        IWorldClient client,
        Bank bank,
        Equipment equipment,
        Inventory inventory,
        SkillBook skillBook,
        SpellBook spellBook,
        Legend legend
    )
    {
        Client = client;
        Bank = bank;
        Equipment = equipment;
        Inventory = inventory;
        SkillBook = skillBook;
        SpellBook = spellBook;
        Legend = legend;
        
        //add observers
        var inventoryObserver = new InventoryObserver(this);
        var spellBookObserver = new SpellBookObserver(this);
        var skillBookObserver = new SkillBookObserver(this);
        var equipmentObserver = new EquipmentObserver(this);

        inventory.AddObserver(inventoryObserver);
        spellBook.AddObserver(spellBookObserver);
        skillBook.AddObserver(skillBookObserver);
        equipment.AddObserver(equipmentObserver);

        //trigger observers
        foreach (var item in equipment)
            equipmentObserver.OnAdded(item);

        foreach (var item in inventory)
            inventoryObserver.OnAdded(item);

        foreach (var spell in spellBook)
            spellBookObserver.OnAdded(spell);

        foreach (var skill in skillBook)
            skillBookObserver.OnAdded(skill);
    }

    protected override void ApplyAcModifier(ref float damage)
    {
        if (StatSheet.Ac == 0)
            return;

        var ac = Math.Clamp(StatSheet.Ac, WorldOptions.Instance.MinimumAislingAc, WorldOptions.Instance.MaximumAislingAc);
        var mod = 1 + ac / 100.0f;
        damage *= mod;
    }

    public bool CanCarry(params Item[] items) => CanCarry(items.Select(item => (item, item.Count)));

    public bool CanCarry(IEnumerable<(Item Item, int Count)> hypotheticalItems)
    {
        var weightSum = 0;
        var slotSum = 0;

        //group all separated stacks together by summing their counts
        foreach (var set in hypotheticalItems.GroupBy(
                     set => set.Item.DisplayName,
                     (_, grp) =>
                     {
                         var col = grp.ToList();

                         return (col.First().Item, Count: col.Sum(i => i.Count));
                     }))
        {
            //separate each stack into it's most condensed possible form
            var maxStacks = set.Item.Template.MaxStacks;
            var estimatedStacks = (int)Math.Ceiling(set.Count / (decimal)maxStacks);
            weightSum += set.Item.Template.Weight * estimatedStacks;
            slotSum += estimatedStacks;
        }

        return (UserStatSheet.CurrentWeight + weightSum <= UserStatSheet.MaxWeight) && (Inventory.AvailableSlots >= slotSum);
    }

    public bool CanCarry(params (Item Item, int Count)[] hypotheticalItems) => CanCarry(hypotheticalItems.AsEnumerable());

    public void GiveGold(int amount)
    {
        Gold += amount;
        Client.SendAttributes(StatUpdateType.ExpGold);
    }

    public void GiveExp(long amount)
    {
        if (amount + UserStatSheet.TotalExp > uint.MaxValue)
            amount = uint.MaxValue - UserStatSheet.TotalExp;

        //if you're at max level, you don't gain exp
        if (UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
            return;

        Client.SendServerMessage(ServerMessageType.ActiveMessage, $"You have gained {amount} experience!");

        while (amount > 0)
        {
            var expToGive = Math.Min(amount, UserStatSheet.ToNextLevel);
            UserStatSheet.AddTotalExp(expToGive);
            UserStatSheet.AddTNL(-expToGive);
            
            amount -= expToGive;

            if (UserStatSheet.ToNextLevel <= 0)
                LevelUp();
        }

        Client.SendAttributes(StatUpdateType.Full);
    }

    public void LevelUp()
    {
        UserStatSheet.IncrementLevel();
        //TODO: what should go up with level?
    }

    public void Pickup(GroundItem groundItem, byte destinationSlot)
    {
        var item = groundItem.Item;
        
        if (Inventory.TryAdd(destinationSlot, item))
        {
            Logger.LogDebug("{UserName} picked up {Item}", Name, item);
            MapInstance.RemoveObject(groundItem);
            item.Script.OnPickup(this);
        }
    }
    
    public override void OnClicked(Aisling source)
    {
        if (source.Equals(this))
            source.Client.SendSelfProfile();
        else if (IsVisibleTo(source))
            source.Client.SendProfile(this);
    }

    public override void OnGoldDroppedOn(Aisling source, int amount)
    {
        if (!TryStartExchange(source, out var exchange))
            return;

        exchange.SetGold(source, amount);
    }

    public override void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        if (!TryStartExchange(source, out var exchange))
            return;

        if (count == 0)
            exchange.AddItem(source, slot);
        else
            exchange.AddStackableItem(this, slot, count);
    }

    public void Refresh(bool forceRefresh = false)
    {
        var now = DateTime.UtcNow;

        if (!forceRefresh && (now.Subtract(LastRefresh).TotalMilliseconds < WorldOptions.Instance.RefreshIntervalMs))
            return;

        (var aislings, var doors, var otherVisibles) = MapInstance
                                                       .ObjectsWithinRange<VisibleEntity>(this)
                                                       .SortBySendType();

        LastRefresh = now;
        Client.SendMapInfo();
        Client.SendLocation();
        Client.SendAttributes(StatUpdateType.Full);

        foreach (var nearbyAisling in aislings)
        {
            if (nearbyAisling.Equals(this))
                continue;

            if (IsVisibleTo(nearbyAisling))
                nearbyAisling.Client.SendDisplayAisling(this);

            if (nearbyAisling.IsVisibleTo(this))
                Client.SendDisplayAisling(nearbyAisling);
        }

        Client.SendVisibleObjects(otherVisibles.ThatAreVisibleTo(this));
        Client.SendDoors(doors);
        Client.SendMapLoadComplete();
        Client.SendDisplayAisling(this);
        Client.SendRefreshResponse();

        MapInstance.ActivateReactors(this, ReactorTileType.Walk);
    }

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDisplayAisling(this);

    private bool TryStartExchange(Aisling source, [MaybeNullWhen(false)] out Exchange exchange)
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
    }

    public override void Update(TimeSpan delta)
    {
        Equipment.Update(delta);
        Inventory.Update(delta);
        SkillBook.Update(delta);
        SpellBook.Update(delta);
        base.Update(delta);
    }

    public override void Walk(Direction direction)
    {
        Direction = direction;
        var startPoint = Point.From(this);
        var endPoint = PointExtensions.DirectionalOffset(this, direction);

        //admins can walk through creatures and walls
        if (!IsAdmin && !MapInstance.IsWalkable(endPoint))
        {
            Refresh(true);

            return;
        }

        var objsBeforeWalk = MapInstance
                             .ObjectsWithinRange<VisibleEntity>(this)
                             .SortBySendType();

        SetLocation(endPoint);

        var objsAfterWalk = MapInstance
                            .ObjectsWithinRange<VisibleEntity>(this)
                            .SortBySendType();

        //for each aisling that just left view range
        //if they were able to see eachother
        //remove eachother from eachother's view
        foreach (var aisling in objsBeforeWalk.Aislings.Except(objsAfterWalk.Aislings))
        {
            if (aisling.IsVisibleTo(this))
                aisling.HideFrom(this);

            if (IsVisibleTo(aisling))
                HideFrom(aisling);
        }

        //for each other object that just left view range
        //if they were able to see eachother
        //remove eachother from eachother's view
        foreach (var visible in objsBeforeWalk.OtherVisibles.Except(objsAfterWalk.OtherVisibles))
            if (visible.IsVisibleTo(this))
                visible.HideFrom(this);

        //for each aisling that came into view
        //if they are able to see eachother
        //show eachother to eachother
        foreach (var aisling in objsAfterWalk.Aislings.Except(objsBeforeWalk.Aislings))
        {
            if (aisling.IsVisibleTo(this))
                aisling.ShowTo(this);

            if (IsVisibleTo(aisling))
                ShowTo(aisling);
        }

        //send any doors that came into view
        Client.SendDoors(objsAfterWalk.Doors.Except(objsBeforeWalk.Doors));

        //send any other visible objs that came into view that we're able to see
        Client.SendVisibleObjects(
            objsAfterWalk.OtherVisibles
                         .Except(objsBeforeWalk.OtherVisibles)
                         .ThatAreVisibleTo(this));

        var aislingsToUpdate = objsBeforeWalk.Aislings
                                             .Intersect(objsAfterWalk.Aislings)
                                             .ThatCanSee(this);

        foreach (var aisling in aislingsToUpdate)
            if (!aisling.Equals(this))
                aisling.Client.SendCreatureWalk(Id, startPoint, direction);

        Client.SendConfirmClientWalk(startPoint, direction);
        MapInstance.ActivateReactors(this, ReactorTileType.Walk);
    }
}