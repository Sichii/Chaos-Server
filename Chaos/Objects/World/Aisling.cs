using Chaos.Clients.Abstractions;
using Chaos.Common.Abstractions;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Containers;
using Chaos.Containers.Abstractions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Observers;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.AislingScripts;
using Chaos.Scripts.AislingScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;
using PointExtensions = Chaos.Extensions.Geometry.PointExtensions;

namespace Chaos.Objects.World;

public sealed class Aisling : Creature, IScripted<IAislingScript>
{
    private readonly IExchangeFactory ExchangeFactory;
    private readonly ICloningService<Item> ItemCloner;
    public Bank Bank { get; private set; }
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public IWorldClient Client { get; set; }
    public CounterTracker Counters { get; init; }
    public EnumCollection Enums { get; init; }
    public IEquipment Equipment { get; private set; }
    public int FaceSprite { get; set; }
    public FlagCollection Flags { get; init; }
    public Gender Gender { get; set; }
    public Group? Group { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public IgnoreList IgnoreList { get; init; }
    public IInventory Inventory { get; private set; }
    public bool IsAdmin { get; set; }
    public DateTime LastEquipOrUnEquip { get; set; }
    public DateTime LastRefresh { get; set; }
    public Containers.Legend Legend { get; private set; }
    public Nation Nation { get; set; }
    public UserOptions Options { get; init; }
    public byte[] Portrait { get; set; }
    public string ProfileText { get; set; }
    public IPanel<Skill> SkillBook { get; private set; }
    public SocialStatus SocialStatus { get; set; }
    public IPanel<Spell> SpellBook { get; private set; }
    public TimedEventCollection TimedEvents { get; private set; }
    public TitleList Titles { get; init; }
    public UserState UserState { get; set; }
    public UserStatSheet UserStatSheet { get; init; }
    public ResettingCounter ActionThrottle { get; }
    public IInterlockedObject<Dialog> ActiveDialog { get; }
    public IInterlockedObject<object> ActiveObject { get; }
    /// <inheritdoc />
    public override int AssailIntervalMs { get; }
    public ChantTimer ChantTimer { get; }
    public override ILogger<Aisling> Logger { get; }
    public IIntervalTimer SaveTimer { get; }
    /// <inheritdoc />
    public override IAislingScript Script { get; }
    /// <inheritdoc />
    public override ISet<string> ScriptKeys { get; }
    public bool ShouldRefresh => DateTime.UtcNow.Subtract(LastRefresh).TotalMilliseconds > WorldOptions.Instance.RefreshIntervalMs;

    public bool ShouldWalk
    {
        get
        {
            if (WorldOptions.Instance.ProhibitF5Walk && (DateTime.UtcNow.Subtract(LastRefresh).TotalMilliseconds < 150))
                return false;

            if (WorldOptions.Instance.ProhibitItemSwitchWalk && (DateTime.UtcNow.Subtract(LastEquipOrUnEquip).TotalMilliseconds < 150))
                return false;

            if ((Sprite == 0) && WorldOptions.Instance.ProhibitSpeedWalk && !WalkCounter.TryIncrement())
            {
                Logger.LogWarning("{Player} is probably speed walking", this);

                return false;
            }

            return true;
        }
    }

    public ResettingCounter SkillThrottle { get; }
    public ResettingCounter SpellThrottle { get; }

    public override StatSheet StatSheet => UserStatSheet;
    public override CreatureType Type => CreatureType.Aisling;
    public ResettingCounter WalkCounter { get; }

    public Aisling(
        string name,
        MapInstance mapInstance,
        IPoint point,
        IExchangeFactory exchangeFactory,
        IScriptProvider scriptProvider,
        ILogger<Aisling> logger,
        ICloningService<Item> itemCloner
    )
        : this(name, mapInstance, point)
    {
        ExchangeFactory = exchangeFactory;
        Logger = logger;
        ItemCloner = itemCloner;
        SaveTimer = new IntervalTimer(TimeSpan.FromMinutes(WorldOptions.Instance.SaveIntervalMins), false);
        ScriptKeys = new HashSet<string> { ScriptBase.GetScriptKey(typeof(DefaultAislingScript)) };
        Script = scriptProvider.CreateScript<IAislingScript, Aisling>(ScriptKeys, this);
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
        Legend = new Containers.Legend();
        Bank = new Bank();
        Equipment = new Equipment();
        Inventory = new Inventory();
        SkillBook = new SkillBook();
        SpellBook = new SpellBook();
        Effects = new EffectsBar(this);
        ActiveObject = new InterlockedObject<object>();
        ActiveDialog = new InterlockedObject<Dialog>();
        ChantTimer = new ChantTimer(WorldOptions.Instance.MaxChantTimeBurdenMs);
        Portrait = Array.Empty<byte>();
        ProfileText = string.Empty;
        ActionThrottle = new ResettingCounter(WorldOptions.Instance.MaxActionsPerSecond);
        SpellThrottle = new ResettingCounter(WorldOptions.Instance.MaxSpellsPerSecond);
        SkillThrottle = new ResettingCounter(WorldOptions.Instance.MaxSkillsPerSecond);
        WalkCounter = new ResettingCounter(3, 5);
        AssailIntervalMs = WorldOptions.Instance.AislingAssailIntervalMs;
        Flags = new FlagCollection();
        Enums = new EnumCollection();
        TimedEvents = new TimedEventCollection();
        ScriptKeys = new HashSet<string>();
        Counters = new CounterTracker();

        //this object is purely intended to be created and immediately serialized
        //these pieces should never come into play
        Client = null!;
        Logger = null!;
        ExchangeFactory = null!;
        ItemCloner = null!;
        Script = null!;
        SaveTimer = null!;
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

        foreach (var effect in Effects)
        {
            effect.Subject = this;
            effect.OnReApplied();
        }
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
            var weightlessAllowance = 0;

            //for stackable items, we can fill the existing stacks in our inventory without adding any weight
            if (set.Item.Template.Stackable)
            {
                var numUniqueStacks = Inventory.Count(i => i.DisplayName.EqualsI(set.Item.DisplayName));
                var totalCount = Inventory.CountOf(set.Item.DisplayName);
                var maxCount = set.Item.Template.MaxStacks * numUniqueStacks;

                //if we have any stacks of this item, we can fill them up without adding any weight
                //if we don't have any stacks of this item, we can fill one stack without adding any weight
                var allowedCount = numUniqueStacks == 0 ? set.Item.Template.MaxStacks : set.Item.Template.MaxStacks - totalCount;

                if (set.Count > allowedCount)
                    return false;

                //so we calculate that value and subtract it from the count we're using to calculate how much this item will weigh
                weightlessAllowance = maxCount - totalCount;
            }

            //separate each stack into it's most condensed possible form
            var maxStacks = set.Item.Template.MaxStacks;
            //the number of stacks we will actually need to add to the inventory
            var countActual = Math.Max(0, set.Count - weightlessAllowance);
            var estimatedStacks = (int)Math.Ceiling(countActual / (decimal)maxStacks);
            weightSum += set.Item.Weight * estimatedStacks;
            slotSum += estimatedStacks;
        }

        return (UserStatSheet.CurrentWeight + weightSum <= UserStatSheet.MaxWeight) && (Inventory.AvailableSlots >= slotSum);
    }

    public bool CanCarry(params (Item Item, int Count)[] hypotheticalItems) => CanCarry(hypotheticalItems.AsEnumerable());

    /// <inheritdoc />
    public override bool CanUse(Skill skill, [MaybeNullWhen(false)] out ActivationContext skillContext)
    {
        skillContext = null;

        if (!Script.CanUseSkill(skill))
            return false;

        if (!skill.Template.IsAssail && (!ActionThrottle.CanIncrement || !SkillThrottle.CanIncrement))
            return false;

        return base.CanUse(skill, out skillContext!);
    }

    /// <inheritdoc />
    public override bool CanUse(
        Spell spell,
        Creature target,
        string? prompt,
        [MaybeNullWhen(false)]
        out SpellContext spellContext
    )
    {
        spellContext = null;

        if (!Script.CanUseSpell(spell))
            return false;

        if (!ActionThrottle.CanIncrement)
            return false;

        if (!SpellThrottle.CanIncrement)
            return false;

        return base.CanUse(
            spell,
            target,
            prompt,
            out spellContext!);
    }

    public bool CanUse(Item item) => Script.CanUseItem(item) && ActionThrottle.CanIncrement && item.CanUse() && item.Script.CanUse(this);

    public void Equip(EquipmentType type, Item item)
    {
        var slot = item.Slot;

        //try equip,
        if (Equipment.TryEquip(type, item, out var returnedItem))
        {
            Inventory.Remove(slot);

            if (returnedItem != null)
                Inventory.TryAddToNextSlot(returnedItem);

            LastEquipOrUnEquip = DateTime.UtcNow;
        }
    }

    public void Initialize(
        string name,
        Bank bank,
        Equipment equipment,
        Inventory inventory,
        SkillBook skillBook,
        SpellBook spellBook,
        Containers.Legend legend,
        EffectsBar effects,
        TimedEventCollection timedEvents
    )
    {
        Name = name;
        Bank = bank;
        Equipment = equipment;
        Inventory = inventory;
        SkillBook = skillBook;
        SpellBook = spellBook;
        Legend = legend;
        Effects = effects;
        TimedEvents = timedEvents;
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
        if (!groundItem.CanPickUp(this))
        {
            SendActiveMessage("You can't pick that up right now");

            return;
        }

        var item = groundItem.Item;

        if (TryGiveItem(item, destinationSlot))
        {
            Logger.LogDebug("{Player} picked up {Item}", this, groundItem);
            MapInstance.RemoveObject(groundItem);
            item.Script.OnPickup(this);

            foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(groundItem).ToList())
                reactor.OnItemPickedUpFrom(this, groundItem);
        }
    }

    public void PickupMoney(Money money)
    {
        if (!money.CanPickUp(this))
        {
            SendActiveMessage("You can't pick that up right now");

            return;
        }

        if (TryGiveGold(money.Amount))
        {
            Logger.LogDebug("{Player} picked up {Gold}", this, money);

            MapInstance.RemoveObject(money);
        }
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

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(this).ToList())
            reactor.OnWalkedOn(this);
    }

    public void SendActiveMessage(string message) => SendServerMessage(ServerMessageType.ActiveMessage, message);

    public void SendOrangeBarMessage(string message) => SendServerMessage(ServerMessageType.OrangeBar1, message);

    public void SendPersistentMessage(string message) => SendServerMessage(ServerMessageType.PersistentMessage, message);

    public void SendServerMessage(ServerMessageType serverMessageType, string message) =>
        Client.SendServerMessage(serverMessageType, message);

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDisplayAisling(this);

    public bool TryDrop(
        IPoint point,
        byte slot,
        [MaybeNullWhen(false)]
        out GroundItem[] groundItems,
        int? amount = null
    )
    {
        groundItems = null;

        if (MapInstance.IsWall(point))
            return false;

        if (!this.WithinRange(point, WorldOptions.Instance.DropRange))
            return false;

        var item = Inventory[slot];

        if ((item == null) || item.Template.AccountBound)
            return false;

        if (amount.HasValue)
        {
            if (!Inventory.HasCount(item.DisplayName, amount.Value))
                return false;

            if (Inventory.RemoveQuantity(item.Slot, amount.Value, out var items))
                return TryDrop(point, items.FixStacks(ItemCloner), out groundItems);
        } else
        {
            if (Inventory.TryGetRemove(slot, out var droppedItem))
                return TryDrop(point, out groundItems, droppedItem);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool TryDropGold(IPoint point, int amount, [MaybeNullWhen(false)] out Money money)
    {
        money = null;

        if (!TryTakeGold(amount))
            return false;

        money = new Money(amount, MapInstance, point);
        MapInstance.AddObject(money, point);

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(money).ToList())
            reactor.OnGoldDroppedOn(this, money);

        return true;
    }

    public bool TryGiveGold(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Cannot give negative gold.");

        var @new = Gold + amount;

        if (@new > WorldOptions.Instance.MaxGoldHeld)
        {
            SendOrangeBarMessage("You have too much gold.");

            return false;
        }

        Gold = @new;

        Client.SendAttributes(StatUpdateType.ExpGold);

        return true;
    }

    public bool TryGiveItem(Item item, byte? slot = null)
    {
        if (!CanCarry(item))
        {
            SendOrangeBarMessage("You can't carry that");

            return false;
        }

        //cancarry will allow adding stackable items even if we are at max weight, if the inventory contains enough incomplete stacks to store them
        //if we're at max weight, the item is stackable, has weight, and the object is being added to a specific slot, and that slot is empty
        //this will overweight the character...
        /*
        bool WillOverweight(Item localItem, byte localSlot)
        {
            var slotItem = Inventory[localSlot];

            if (slotItem != null)
                return false;

            return localItem.Template.Stackable
                   && (localItem.Template.Weight > 0)
                   && (UserStatSheet.CurrentWeight >= UserStatSheet.MaxWeight);
        }*/

        if (slot.HasValue)
            return Inventory.TryAdd(slot.Value, item);

        return Inventory.TryAddToNextSlot(item);
    }

    public bool TryGiveItems(params Item[] items)
    {
        if (!CanCarry(items))
        {
            SendOrangeBarMessage("You can't carry that");

            return false;
        }

        foreach (var item in items.FixStacks(ItemCloner))
            Inventory.TryAddToNextSlot(item);

        return true;
    }

    private bool TryStartExchange(Aisling source, [MaybeNullWhen(false)] out Exchange exchange)
    {
        exchange = ExchangeFactory.CreateExchange(source, this);

        if (!ActiveObject.SetIfNull(exchange))
        {
            source.SendActiveMessage($"{Name} is busy right now");
            SendActiveMessage($"{source.Name} is trying to exchange with you");

            exchange = null;

            return false;
        }

        if (!source.ActiveObject.SetIfNull(exchange))
        {
            ActiveObject.TryRemove(exchange);
            source.SendActiveMessage("You're already busy");

            exchange = null;

            return false;
        }

        exchange.Activate();

        return true;
    }

    public bool TryTakeGold(int amount)
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Cannot take negative gold.");

        if (amount == 0)
            return true;

        var @new = Gold - amount;

        if (@new < 0)
        {
            SendOrangeBarMessage($"You do not have enough gold, you need a total of {amount}");

            return false;
        }

        Gold = @new;
        Client.SendAttributes(StatUpdateType.ExpGold);

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

    public override bool TryUseSkill(Skill skill)
    {
        if (!CanUse(skill, out var context))
            return false;

        if (!skill.Template.IsAssail)
        {
            if (!ActionThrottle.TryIncrement())
                return false;

            if (!SkillThrottle.TryIncrement())
                return false;
        }

        skill.Use(context);

        return true;
    }

    public bool TryUseSkill(byte slot)
    {
        if (!SkillBook.TryGetObject(slot, out var skill))
            return false;

        return TryUseSkill(skill);
    }

    /// <inheritdoc />
    public override bool TryUseSpell(Spell spell, uint? targetId = null, string? prompt = null)
    {
        Creature? target;

        if (!targetId.HasValue)
        {
            if (spell.Template.SpellType == SpellType.Targeted)
                return false;

            target = this;
        } else if (!MapInstance.TryGetObject(targetId.Value, out target))
            return false;

        if (!CanUse(
                spell,
                target!,
                prompt,
                out var context))
            return false;

        if (!ActionThrottle.TryIncrement())
            return false;

        if (!SpellThrottle.TryIncrement())
            return false;

        spell.Use(context);

        return true;
    }

    public bool TryUseSpell(byte slot, uint? targetId = null, string? prompt = null)
    {
        if (!SpellBook.TryGetObject(slot, out var spell))
            return false;

        return TryUseSpell(spell, targetId, prompt);
    }

    public void UnEquip(EquipmentSlot slot)
    {
        if (Inventory.IsFull)
            return;

        if (!Equipment.TryGetRemove((byte)slot, out var item))
            return;

        Inventory.TryAddToNextSlot(item);
        LastEquipOrUnEquip = DateTime.UtcNow;
    }

    public override void Update(TimeSpan delta)
    {
        Equipment.Update(delta);
        Inventory.Update(delta);
        SkillBook.Update(delta);
        SpellBook.Update(delta);
        ActionThrottle.Update(delta);
        SpellThrottle.Update(delta);
        SkillThrottle.Update(delta);
        WalkCounter.Update(delta);
        ChantTimer.Update(delta);
        TimedEvents.Update(delta);
        SaveTimer.Update(delta);

        base.Update(delta);
    }

    public override void Walk(Direction direction)
    {
        if (!Script.CanMove() || ((direction != Direction) && !Script.CanTurn()) || !ShouldWalk)
        {
            Refresh(true);

            return;
        }

        Direction = direction;
        var startPoint = Point.From(this);
        var endPoint = PointExtensions.DirectionalOffset(this, direction);

        //admins can walk through creatures and walls
        if (!IsAdmin && !MapInstance.IsWalkable(endPoint, Type))
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
        foreach (var visible in objsBeforeWalk.OtherVisibles.Except(objsAfterWalk.OtherVisibles))
            if (visible is Creature creature)
                Helpers.HandleApproach(creature, this);

        //send all doors that arent nearby
        //this will result in sending doors multiple times but we have to do this because
        //the client ignores doors greater than 12 spaces away
        //if a client viewport is offset due to walking issues, it could ignore doors we send specifically at a 12 space distance
        var doors = objsAfterWalk.Doors.Where(door => door.DistanceFrom(this) >= 10);
        //objsAfterWalk.Doors.Except(objsBeforeWalk.Doors, PointEqualityComparer.Instance)
        //             .Cast<Door>();

        //send any doors that came into view
        Client.SendDoors(doors);

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

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(this).ToList())
            reactor.OnWalkedOn(this);
    }

    /// <inheritdoc />
    public override void WarpTo(IPoint destinationPoint)
    {
        Hide();

        var creaturesBefore = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                         .ToList();

        SetLocation(destinationPoint);

        var creaturesAfter = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                        .ToList();

        foreach (var creature in creaturesBefore.Except(creaturesAfter))
            Helpers.HandleDeparture(creature, this);

        foreach (var creature in creaturesAfter.Except(creaturesBefore))
            Helpers.HandleApproach(creature, this);

        Refresh(true);
    }
}