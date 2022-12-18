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
using Chaos.Formulae;
using Chaos.Formulae.LevelUp;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Observers;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;
using PointExtensions = Chaos.Extensions.Geometry.PointExtensions;

namespace Chaos.Objects.World;

public sealed class Aisling : Creature
{
    private readonly IExchangeFactory ExchangeFactory;
    private readonly ICloningService<Item> ItemCloner;
    public Bank Bank { get; private set; }
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public IWorldClient Client { get; set; }
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
                Logger.LogWarning("{Player} is probably speed walking", this);

                return false;
            }

            return true;
        }
    }

    public override StatSheet StatSheet => UserStatSheet;
    public override CreatureType Type => CreatureType.Aisling;
    public ResettingCounter WalkCounter { get; }
    protected override ILogger<Aisling> Logger { get; }
    private IIntervalTimer RegenTimer { get; }

    public Aisling(
        string name,
        MapInstance mapInstance,
        IPoint point,
        IExchangeFactory exchangeFactory,
        ILogger<Aisling> logger,
        ICloningService<Item> itemCloner
    )
        : this(name, mapInstance, point)
    {
        ExchangeFactory = exchangeFactory;
        Logger = logger;
        ItemCloner = itemCloner;
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
        ActionThrottle = new ResettingCounter(WorldOptions.Instance.MaxActionsPerSecond, new IntervalTimer(TimeSpan.FromSeconds(1)));
        WalkCounter = new ResettingCounter(10, new IntervalTimer(TimeSpan.FromSeconds(3)));
        AssailIntervalMs = WorldOptions.Instance.AislingAssailIntervalMs;
        Flags = new FlagCollection();
        RegenTimer = new RegenTimer(this, RegenFormulae.Default);
        TimedEvents = new TimedEventCollection();

        //this object is purely intended to be created and immediately serialized
        //these pieces should never come into play
        Client = null!;
        Logger = null!;
        ExchangeFactory = null!;
        ItemCloner = null!;
    }

    /// <inheritdoc />
    public override void ApplyDamage(Creature source, int amount, byte? hitSound = 1)
    {
        StatSheet.SubtractHp(amount);
        Client.SendAttributes(StatUpdateType.Vitality);
        ShowHealth(hitSound);
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

                //so we calculate that value and subtract it from the count we're using to calculate how much this item will weigh
                weightlessAllowance = maxCount - totalCount;
            }

            //separate each stack into it's most condensed possible form
            var maxStacks = set.Item.Template.MaxStacks;
            //the number of stacks we will actually need to add to the inventory
            var countActual = Math.Max(0, set.Count - weightlessAllowance);
            var estimatedStacks = (int)Math.Ceiling(countActual / (decimal)maxStacks);
            weightSum += set.Item.Template.Weight * estimatedStacks;
            slotSum += estimatedStacks;
        }

        return (UserStatSheet.CurrentWeight + weightSum <= UserStatSheet.MaxWeight) && (Inventory.AvailableSlots >= slotSum);
    }

    public bool CanCarry(params (Item Item, int Count)[] hypotheticalItems) => CanCarry(hypotheticalItems.AsEnumerable());

    /// <inheritdoc />
    public override bool CanUse(Skill skill, out SkillContext skillContext) =>
        base.CanUse(skill, out skillContext!) && ActionThrottle.TryIncrement();

    /// <inheritdoc />
    public override bool CanUse(
        Spell spell,
        Creature target,
        string? prompt,
        out SpellContext spellContext
    ) =>
        base.CanUse(
            spell,
            target,
            prompt,
            out spellContext!)
        && ActionThrottle.TryIncrement();

    public bool CanUse(Item item) => item.CanUse() && item.Script.CanUse(this);

    public void Drop(IPoint point, byte slot, int? amount = null)
    {
        if (MapInstance.IsWall(point))
            return;

        if (!this.WithinRange(point, WorldOptions.Instance.DropRange))
            return;

        var item = Inventory[slot];

        if ((item == null) || item.Template.AccountBound)
            return;

        if (amount.HasValue)
        {
            if (!Inventory.HasCount(item.DisplayName, amount.Value))
                return;

            if (Inventory.RemoveQuantity(item.Slot, amount.Value, out var items))
                Drop(point, items.FixStacks(ItemCloner));
        } else
        {
            if (Inventory.TryGetRemove(slot, out var droppedItem))
                Drop(point, droppedItem);
        }
    }

    /// <inheritdoc />
    public override void DropGold(IPoint point, int amount)
    {
        if (!TryTakeGold(amount))
            return;

        var money = new Money(amount, MapInstance, point);
        MapInstance.AddObject(money, point);

        foreach (var reactor in MapInstance.GetEntitiesAtPoint<ReactorTile>(money))
            reactor.OnGoldDroppedOn(this, money);
    }

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

    public void GiveExp(long amount)
    {
        if (amount + UserStatSheet.TotalExp > uint.MaxValue)
            amount = uint.MaxValue - UserStatSheet.TotalExp;

        //if you're at max level, you don't gain exp
        if (UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
            return;

        SendActiveMessage($"You have gained {amount} experience!");

        while (amount > 0)
        {
            if (UserStatSheet.Level >= WorldOptions.Instance.MaxLevel)
                break;

            var expToGive = Math.Min(amount, UserStatSheet.ToNextLevel);
            UserStatSheet.AddTotalExp(expToGive);
            UserStatSheet.AddTNL(-expToGive);

            amount -= expToGive;

            if (UserStatSheet.ToNextLevel <= 0)
                LevelUp();
        }

        Client.SendAttributes(StatUpdateType.Full);
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

        if (TryGiveItem(item, destinationSlot))
        {
            Logger.LogDebug("{Player} picked up {Item}", this, item);

            MapInstance.RemoveObject(groundItem);
            item.Script.OnPickup(this);
        }
    }

    public void PickupMoney(Money money)
    {
        if (TryGiveGold(money.Amount))
        {
            Logger.LogDebug("{Player} picked up {Amount} gold", this, money.Amount);

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

        foreach (var reactor in MapInstance.GetEntitiesAtPoint<ReactorTile>(Point.From(this)))
            reactor.OnWalkedOn(this);
    }

    public void SendActiveMessage(string message) => SendServerMessage(ServerMessageType.ActiveMessage, message);

    public void SendOrangeBarMessage(string message) => SendServerMessage(ServerMessageType.OrangeBar1, message);

    public void SendPersistentMessage(string message) => SendServerMessage(ServerMessageType.PersistentMessage, message);

    public void SendServerMessage(ServerMessageType serverMessageType, string message) =>
        Client.SendServerMessage(serverMessageType, message);

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDisplayAisling(this);

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

    public override void Update(TimeSpan delta)
    {
        Equipment.Update(delta);
        Inventory.Update(delta);
        SkillBook.Update(delta);
        SpellBook.Update(delta);
        ActionThrottle.Update(delta);
        WalkCounter.Update(delta);
        ChantTimer.Update(delta);
        RegenTimer.Update(delta);

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

        foreach (var reactor in MapInstance.GetEntitiesAtPoint<ReactorTile>(Point.From(this)))
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