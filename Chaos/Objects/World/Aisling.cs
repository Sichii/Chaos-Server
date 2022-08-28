using Chaos.Clients.Interfaces;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Containers.Interfaces;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Entities.Schemas.World;
using Chaos.Extensions;
using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Observers;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Hosted.Options;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Services.Utility.Interfaces;
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
    public Bank Bank { get; }
    public IEquipment Equipment { get; }
    public IgnoreList IgnoreList { get; }
    public IInventory Inventory { get; }
    public Legend Legend { get; }
    public UserOptions Options { get; }
    public IPanel<Skill> SkillBook { get; }
    public IPanel<Spell> SpellBook { get; }
    public override UserStatSheet StatSheet { get; }
    public TitleList Titles { get; }
    public override CreatureType Type => CreatureType.Aisling;
    protected override ILogger<Aisling> Logger { get; }

    public Aisling(
        AislingSchema schema,
        ISimpleCache simpleCache,
        ICloningService<Item> itemCloner,
        ITypeMapper mapper,
        IExchangeFactory exchangeFactory,
        ILogger<Aisling> logger
    )
        : base(
            schema.Name,
            0,
            simpleCache.GetObject<MapInstance>(schema.MapInstanceId),
            new Point(schema.X, schema.Y))
    {
        ExchangeFactory = exchangeFactory;
        Logger = logger;

        BodyColor = schema.BodyColor;
        BodySprite = schema.BodySprite;
        Direction = schema.Direction;
        FaceSprite = schema.FaceSprite;
        GamePoints = schema.GamePoints;
        Gender = schema.Gender;
        Gold = schema.Gold;
        GuildName = schema.GuildName;
        GuildTitle = schema.GuildTitle;
        HairColor = schema.HairColor;
        HairStyle = schema.HairStyle;
        Name = schema.Name;
        Nation = schema.Nation;
        X = schema.X;
        Y = schema.Y;

        StatSheet = mapper.Map<UserStatSheet>(schema.StatSheet);
        Options = mapper.Map<UserOptions>(schema.UserOptions);
        Titles = new TitleList(schema.Titles);
        IgnoreList = new IgnoreList(schema.IgnoreList);
        Legend = new Legend(schema.Legend.Select(s => new LegendMark(s)));

        Bank = new Bank(
            schema.BankedGold,
            schema.Bank,
            mapper,
            itemCloner);

        Equipment = new Equipment(schema.Equipment, mapper);
        Inventory = new Inventory(itemCloner, schema.Inventory, mapper);
        SkillBook = new SkillBook(schema.SkillBook, mapper);
        SpellBook = new SpellBook(schema.SpellBook, mapper);
        //TODO: Effects
        ActiveObject = new ActiveObject();
        Client = null!;
        Portrait = Array.Empty<byte>();
        ProfileText = string.Empty;

        //add observers
        var inventoryObserver = new InventoryObserver(this);
        var spellBookObserver = new SpellBookObserver(this);
        var skillBookObserver = new SkillBookObserver(this);
        var equipmentObserver = new EquipmentObserver(this);

        Inventory.AddObserver(inventoryObserver);
        SpellBook.AddObserver(spellBookObserver);
        SkillBook.AddObserver(skillBookObserver);
        Equipment.AddObserver(equipmentObserver);

        foreach (var item in Equipment)
            equipmentObserver.OnAdded(item);

        foreach (var item in Inventory)
            inventoryObserver.OnAdded(item);

        foreach (var spell in SpellBook)
            spellBookObserver.OnAdded(spell);

        foreach (var skill in SkillBook)
            skillBookObserver.OnAdded(skill);
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
        StatSheet = UserStatSheet.NewCharacter;
    }

    private Aisling(string name, MapInstance mapInstance, IPoint point)
        : base(
            name,
            0,
            mapInstance,
            point)
    {
        //initialize all the things
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
            var estimatedStacks = set.Count / maxStacks + (set.Count & maxStacks);
            weightSum += set.Item.Template.Weight * estimatedStacks;
            slotSum += estimatedStacks;
        }

        return (StatSheet.CurrentWeight + weightSum <= StatSheet.MaxWeight) && (Inventory.AvailableSlots >= slotSum);
    }

    public bool CanCarry(params (Item Item, int Count)[] hypotheticalItems) => CanCarry(hypotheticalItems.AsEnumerable());

    public void GiveGold(int amount)
    {
        Gold += amount;
        Client.SendAttributes(StatUpdateType.ExpGold);
    }

    public override void OnClicked(Aisling source)
    {
        if (source.Equals(this))
            source.Client.SendSelfProfile();
        else if (IsVisibleTo(source))
            source.Client.SendProfile(this);
    }

    public override void OnGoldDroppedOn(int amount, Aisling source)
    {
        if (!TryStartExchange(source, out var exchange))
            return;

        exchange.SetGold(source, amount);
    }

    public override void OnItemDroppedOn(byte slot, byte count, Aisling source)
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