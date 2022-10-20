using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Containers.Abstractions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Formulae.LevelUp;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Observers;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Time;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;
using PointExtensions = Chaos.Extensions.Geometry.PointExtensions;

namespace Chaos.Objects.World;

public sealed class Aisling : Creature
{
    private readonly IExchangeFactory ExchangeFactory;
    public Bank Bank { get; private set; }
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public IWorldClient Client { get; set; }
    public IEquipment Equipment { get; private set; }
    public int FaceSprite { get; set; }
    public Gender Gender { get; set; }
    public Group? Group { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public IgnoreList IgnoreList { get; init; }
    public IInventory Inventory { get; private set; }
    public bool IsAdmin { get; set; }
    public DateTime LastRefresh { get; set; }
    public ResettingCounter WalkCounter { get; set; }
    public DateTime LastEquipOrUnEquip { get; set; }
    public Legend Legend { get; private set; }
    public Nation Nation { get; set; }
    public UserOptions Options { get; init; }
    public byte[] Portrait { get; set; }
    public string ProfileText { get; set; }
    public IPanel<Skill> SkillBook { get; private set; }
    public SocialStatus SocialStatus { get; set; }
    public IPanel<Spell> SpellBook { get; private set; }
    public TitleList Titles { get; init; }
    public UserState UserState { get; set; }
    public UserStatSheet UserStatSheet { get; init; }
    public ResettingCounter ActionThrottle { get; }
    public ActiveObject ActiveObject { get; }
    /// <inheritdoc />
    public override int AssailIntervalMs { get; }
    public override StatSheet StatSheet => UserStatSheet;
    public override CreatureType Type => CreatureType.Aisling;
    protected override ILogger<Aisling> Logger { get; }
    public bool ShouldRefresh => DateTime.UtcNow.Subtract(LastRefresh).TotalMilliseconds > WorldOptions.Instance.RefreshIntervalMs;
    public bool ShouldWalk
    {
        get
        {
            if (WorldOptions.Instance.ProhibitF5Walk && (DateTime.UtcNow.Subtract(LastRefresh).TotalMilliseconds < 150))
                return false;

            if (WorldOptions.Instance.ProhibitItemSwitchWalk && (DateTime.UtcNow.Subtract(LastEquipOrUnEquip).TotalMilliseconds < 150))
                return false;

            if (WorldOptions.Instance.ProhibitSpeedWalk && !WalkCounter.TryIncrement())
            {
                Logger.LogWarning("{Name} is probably speed walking", Name);
                
                return false;
            }

            return true;
        }
    }

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
        ActionThrottle = new ResettingCounter(WorldOptions.Instance.MaxActionsPerSecond, new IntervalTimer(TimeSpan.FromSeconds(1)));
        WalkCounter = new ResettingCounter(10, new IntervalTimer(TimeSpan.FromSeconds(3)));
        AssailIntervalMs = WorldOptions.Instance.AislingAssailIntervalMs;

        //this object is purely intended to be created and immediately serialized
        //these pieces should never come into play
        Client = null!;
        Logger = null!;
        ExchangeFactory = null!;
    }

    /// <inheritdoc />
    public override void ApplyDamage(Creature source, int amount, byte? hitSound = 1)
    {
        StatSheet.SubtractHp(amount);
        Client.SendAttributes(StatUpdateType.Vitality);

        foreach (var obj in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                       .ThatCanSee(this))
            obj.Client.SendHealthBar(this, hitSound);
    }

    public void BeginObserving()
    {
        //add observers
        var inventoryObserver = new InventoryObserver(this);
        var spellBookObserver = new SpellBookObserver(this);
        var skillBookObserver = new SkillBookObserver(this);
        var equipmentObserver = new EquipmentObserver(this);

        Inventory.AddObserver(inventoryObserver);
        SpellBook.AddObserver(spellBookObserver);
        SkillBook.AddObserver(skillBookObserver);
        Equipment.AddObserver(equipmentObserver);

        //trigger observers
        foreach (var item in Equipment)
            equipmentObserver.OnAdded(item);

        foreach (var item in Inventory)
            inventoryObserver.OnAdded(item);

        foreach (var spell in SpellBook)
            spellBookObserver.OnAdded(spell);

        foreach (var skill in SkillBook)
            skillBookObserver.OnAdded(skill);
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

    /// <inheritdoc />
    public override bool CanUse(PanelObjectBase panelObject) => base.CanUse(panelObject) && ActionThrottle.TryIncrement();

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

    public void GiveGold(int amount)
    {
        Gold += amount;
        Client.SendAttributes(StatUpdateType.ExpGold);
    }

    public void Initialize(
        string name,
        Bank bank,
        Equipment equipment,
        Inventory inventory,
        SkillBook skillBook,
        SpellBook spellBook,
        Legend legend
    )
    {
        Name = name;
        Bank = bank;
        Equipment = equipment;
        Inventory = inventory;
        SkillBook = skillBook;
        SpellBook = spellBook;
        Legend = legend;
    }

    public void LevelUp()
    {
        //maybe use a diff formula for each class? idk
        var levelUpFormula = new DefaultLevelUpFormula();
        levelUpFormula.LevelUp(this);
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

    public void PickupItem(GroundItem groundItem, byte destinationSlot)
    {
        var item = groundItem.Item;

        if (Inventory.TryAdd(destinationSlot, item))
        {
            Logger.LogDebug("{UserName} picked up {Item}", Name, item);

            if (MapInstance.RemoveObject(groundItem))
                item.Script.OnPickup(this);
        }
    }

    public void PickupMoney(Money money)
    {
        GiveGold(money.Amount);
        MapInstance.RemoveObject(money);
    }

    public void Refresh(bool forceRefresh = false)
    {
        var now = DateTime.UtcNow;

        if (!forceRefresh && !ShouldRefresh)
            return;

        (var aislings, var doors, var otherVisibles) = MapInstance.GetEntitiesWithinRange<VisibleEntity>(this)
                                                                  .PartitionBySendType();

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

        MapInstance.ActivateReactors(this, ReactorActivationType.Walk);
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

    public bool TryUseItem(byte slot)
    {
        if (!Inventory.TryGetObject(slot, out var item))
            return false;

        return TryUseItem(item);
    }

    public bool TryUseItem(Item item)
    {
        if (!CanUse(item))
            return false;

        item.Use(this);

        return true;
    }

    public bool TryUseSkill(byte slot)
    {
        if (!SkillBook.TryGetObject(slot, out var skill))
            return false;

        return TryUseSkill(skill);
    }

    public bool TryUseSpell(byte slot, uint? targetId = null, string? prompt = null)
    {
        if (!SpellBook.TryGetObject(slot, out var spell))
            return false;

        return TryUseSpell(spell, targetId, prompt);
    }

    public override void Update(TimeSpan delta)
    {
        Equipment.Update(delta);
        Inventory.Update(delta);
        SkillBook.Update(delta);
        SpellBook.Update(delta);
        ActionThrottle.Update(delta);
        WalkCounter.Update(delta);
        base.Update(delta);
    }

    public override void Walk(Direction direction)
    {
        //don't allow f5 walking
        if (!ShouldWalk)
            return;
        
        Direction = direction;
        var startPoint = Point.From(this);
        var endPoint = PointExtensions.DirectionalOffset(this, direction);

        //admins can walk through creatures and walls
        if (!IsAdmin && !MapInstance.IsWalkable(endPoint))
        {
            Refresh(true);

            return;
        }

        var objsBeforeWalk = MapInstance.GetEntitiesWithinRange<VisibleEntity>(this)
                                        .PartitionBySendType();

        SetLocation(endPoint);

        var objsAfterWalk = MapInstance.GetEntitiesWithinRange<VisibleEntity>(this)
                                       .PartitionBySendType();

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
        {
            if (visible.IsVisibleTo(this))
                visible.HideFrom(this);

            //handle departure
            if (visible is Creature creature)
                Helpers.HandleDeparture(creature, this);
        }

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
        
        //handle approach
        foreach(var visible in objsBeforeWalk.OtherVisibles.Except(objsAfterWalk.OtherVisibles))
            if (visible is Creature creature)
                Helpers.HandleApproach(creature, this);

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
        MapInstance.ActivateReactors(this, ReactorActivationType.Walk);
    }

    /// <inheritdoc />
    public override void WarpTo(IPoint destinationPoint)
    {
        Hide();

        var creaturesBefore = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                         .ToList();

        SetLocation(destinationPoint);
        Client.SendLocation();
        
        var creaturesAfter = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                        .ToList();
        
        foreach (var creature in creaturesBefore.Except(creaturesAfter))
            Helpers.HandleDeparture(creature, this);
        
        foreach (var creature in creaturesAfter.Except(creaturesBefore))
            Helpers.HandleApproach(creature, this);
        
        Display();
    }

    public void Equip(EquipmentType type, Item item)
    {
        var slot = item.Slot;
        
        //try equip,
        if (Equipment.TryEquip(type, item, out var returnedItem))
        {
            Inventory.Remove(slot);
            item.Script.OnEquipped(this);

            if (returnedItem != null)
                Inventory.TryAddToNextSlot(returnedItem);
            
            LastEquipOrUnEquip = DateTime.UtcNow;
        }
    }

    public void UnEquip(EquipmentSlot slot)
    {
        if (Inventory.IsFull)
            return;

        if (!Equipment.TryGetRemove((byte)slot, out var item))
            return;

        Inventory.TryAddToNextSlot(item);
        item.Script.OnUnEquipped(this);
        LastEquipOrUnEquip = DateTime.UtcNow;
    }
}