using Chaos.Clients.Interfaces;
using Chaos.Containers;
using Chaos.Containers.Interfaces;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;
using Chaos.Networking.Model.Server;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Objects.World.Abstractions;
using Chaos.Observers;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Hosted.Options;
using Chaos.Services.Serialization.Interfaces;
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
        SerializableAisling serializableAisling,
        ISimpleCache<MapInstance> mapInstanceCache,
        ICloningService<Item> itemCloner,
        ISerialTransformService<Item, SerializableItem> itemTransformer,
        ISerialTransformService<Skill, SerializableSkill> skillTransformer,
        ISerialTransformService<Spell, SerializableSpell> spellTransformer,
        IExchangeFactory exchangeFactory,
        ILogger<Aisling> logger
    )
        : base(
            serializableAisling.Name,
            0,
            mapInstanceCache.GetObject(serializableAisling.MapInstanceId),
            new Point(serializableAisling.X, serializableAisling.Y))
    {
        ExchangeFactory = exchangeFactory;
        Logger = logger;

        BodyColor = serializableAisling.BodyColor;
        BodySprite = serializableAisling.BodySprite;
        Direction = serializableAisling.Direction;
        FaceSprite = serializableAisling.FaceSprite;
        GamePoints = serializableAisling.GamePoints;
        Gender = serializableAisling.Gender;
        Gold = serializableAisling.Gold;
        GuildName = serializableAisling.GuildName;
        GuildTitle = serializableAisling.GuildTitle;
        HairColor = serializableAisling.HairColor;
        HairStyle = serializableAisling.HairStyle;
        Name = serializableAisling.Name;
        Nation = serializableAisling.Nation;
        X = serializableAisling.X;
        Y = serializableAisling.Y;

        StatSheet = serializableAisling.StatSheet;
        Titles = new TitleList(serializableAisling.Titles);
        Options = new UserOptions(serializableAisling.Options);
        IgnoreList = new IgnoreList(serializableAisling.IgnoreList);
        Legend = new Legend(serializableAisling.Legend.Select(s => new LegendMark(s)));

        Bank = new Bank(
            serializableAisling.BankedGold,
            serializableAisling.Bank,
            itemTransformer,
            itemCloner);

        Equipment = new Equipment(serializableAisling.Equipment, itemTransformer);
        Inventory = new Inventory(itemCloner, serializableAisling.Inventory, itemTransformer);
        SkillBook = new SkillBook(serializableAisling.SkillBook, skillTransformer);
        SpellBook = new SpellBook(serializableAisling.SpellBook, spellTransformer);
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

    public AttributesArgs ToAttributeArgs() => new()
    {
        Ability = (byte)StatSheet.Ability,
        Ac = StatSheet.EffectiveAc,
        //TODO: blind
        Con = StatSheet.EffectiveCon,
        CurrentHp = (uint)Math.Clamp(StatSheet.CurrentHp, 0, int.MaxValue),
        CurrentMp = (uint)Math.Clamp(StatSheet.CurrentMp, 0, int.MaxValue),
        CurrentWeight = (short)StatSheet.CurrentWeight,
        DefenseElement = StatSheet.DefenseElement,
        Dex = StatSheet.EffectiveDex,
        Dmg = StatSheet.EffectiveDmg,
        GamePoints = (uint)GamePoints,
        Gold = (uint)Gold,
        Hit = StatSheet.EffectiveHit,
        Int = StatSheet.EffectiveInt,
        IsAdmin = IsAdmin,
        Level = (byte)StatSheet.Level,
        MagicResistance = StatSheet.EffectiveMagicResistance,
        MailFlags = MailFlag.None, //TODO: mail system
        MaximumHp = StatSheet.EffectiveMaximumHp,
        MaximumMp = StatSheet.EffectiveMaximumMp,
        MaxWeight = (short)StatSheet.MaxWeight,
        OffenseElement = StatSheet.OffenseElement,
        Str = StatSheet.EffectiveStr,
        ToNextAbility = (uint)StatSheet.ToNextAbility,
        ToNextLevel = (uint)StatSheet.ToNextLevel,
        TotalAbility = StatSheet.TotalAbility,
        TotalExp = StatSheet.TotalExp,
        UnspentPoints = (byte)StatSheet.UnspentPoints,
        Wis = StatSheet.EffectiveWis
    };

    public DisplayAislingArgs ToDisplayAislingArgs()
    {
        var weapon = Equipment[EquipmentSlot.Weapon];
        var armor = Equipment[EquipmentSlot.Armor];
        var shield = Equipment[EquipmentSlot.Shield];
        var overHelm = Equipment[EquipmentSlot.OverHelm];
        var helmet = Equipment[EquipmentSlot.Helmet];
        var boots = Equipment[EquipmentSlot.Boots];
        var acc1 = Equipment[EquipmentSlot.Accessory1];
        var acc2 = Equipment[EquipmentSlot.Accessory2];
        var acc3 = Equipment[EquipmentSlot.Accessory3];
        var overcoat = Equipment[EquipmentSlot.Overcoat];

        return new DisplayAislingArgs
        {
            AccessoryColor1 = acc1?.Color ?? DisplayColor.None,
            AccessoryColor2 = acc2?.Color ?? DisplayColor.None,
            AccessoryColor3 = acc3?.Color ?? DisplayColor.None,
            AccessorySprite1 = acc1?.Template.PanelSprite ?? 0,
            AccessorySprite2 = acc2?.Template.PanelSprite ?? 0,
            AccessorySprite3 = acc3?.Template.PanelSprite ?? 0,
            ArmorSprite1 = armor?.Template.PanelSprite ?? 0, //TODO: figure this out again cuz i deleted it
            ArmorSprite2 = armor?.Template.PanelSprite ?? 0,
            BodyColor = BodyColor,
            BodySprite = BodySprite,
            BootsColor = boots?.Color ?? DisplayColor.None,
            BootsSprite = (byte)(boots?.Template.PanelSprite ?? 0),
            CreatureType = Type,
            Direction = Direction,
            FaceSprite = (byte)FaceSprite,
            GameObjectType = GameObjectType.Misc,
            Gender = Gender,
            GroupBoxText = null,
            HeadColor = overHelm?.Template.Color ?? helmet?.Template.Color ?? HairColor,
            HeadSprite = overHelm?.Template.PanelSprite ?? helmet?.Template.PanelSprite ?? (ushort)HairStyle,
            Id = Id,
            IsDead = !IsAlive,
            IsHidden = false, //TODO: invisibility
            IsMaster = StatSheet.Master,
            LanternSize = LanternSize.None, //TODO: if we add lanterns and dark maps later,
            Name = Name,
            NameTagStyle = NameTagStyle.NeutralHover, //TODO: if we add pvp later
            OvercoatColor = overcoat?.Color ?? DisplayColor.None,
            OvercoatSprite = overcoat?.Template.PanelSprite ?? 0,
            X = X,
            Y = Y,
            RestPosition = RestPosition.None, //TODO: if we add rest positions in later,
            ShieldSprite = (byte)(shield?.Template.PanelSprite ?? 0),
            Sprite = Sprite,
            WeaponSprite = weapon?.Template.PanelSprite ?? 0
        };
    }

    public ProfileArgs ToProfileArgs() =>
        new()
        {
            AdvClass = StatSheet.AdvClass,
            BaseClass = StatSheet.BaseClass,
            Equipment = Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, i => i.ToItemInfo())!,
            GroupOpen = Options.Group,
            GuildName = GuildName,
            GuildTitle = GuildTitle,
            Id = Id,
            LegendMarks = Legend.Select(l => l.ToLegendMarkInfo()).ToList(),
            Name = Name,
            Nation = Nation,
            Portrait = Portrait,
            ProfileText = ProfileText,
            SocialStatus = SocialStatus,
            Titles = Titles.ToList()
        };

    public SelfProfileArgs ToSelfProfileArgs() =>
        new()
        {
            AdvClass = StatSheet.AdvClass,
            BaseClass = StatSheet.BaseClass,
            Equipment = Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, i => i.ToItemInfo()),
            GroupOpen = Options.Group,
            GroupString = null, //TODO: when we implement group box
            GuildName = GuildName,
            GuildTitle = GuildTitle,
            IsMaster = StatSheet.Master,
            LegendMarks = Legend.Select(l => l.ToLegendMarkInfo()).ToList(),
            Name = Name,
            Nation = Nation,
            Portrait = Portrait,
            ProfileText = ProfileText,
            SocialStatus = SocialStatus,
            SpouseName = null, //TODO: when we implement marraige i guess
            Titles = Titles.ToList()
        };

    public UserIdArgs ToUserIdArgs() => new()
    {
        BaseClass = StatSheet.BaseClass,
        Direction = Direction,
        Gender = Gender,
        Id = Id
    };

    public WorldListMemberInfo ToWorldListMemberInfo() => new()
    {
        BaseClass = StatSheet.BaseClass,
        Color = WorldListColor.White,
        IsMaster = StatSheet.Master,
        Name = Name,
        SocialStatus = SocialStatus,
        Title = Titles.FirstOrDefault()
    };

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