#region
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Collections.Common;
using Chaos.Collections.Synchronized;
using Chaos.Collections.Time;
using Chaos.Common.Abstractions;
using Chaos.Common.Synchronization;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Observers;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Models.World;

public sealed class Aisling : Creature, IScripted<IAislingScript>, IDialogSourceEntity, ICommandSubject, IChannelSubscriber
{
    private readonly IFactory<Exchange> ExchangeFactory;
    private readonly ICloningService<Item> ItemCloner;
    public Bank Bank { get; private set; }
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public SynchronizedHashSet<ChannelSettings> ChannelSettings { get; init; }
    public IChaosWorldClient Client { get; set; }
    public IEquipment Equipment { get; private set; }
    public int FaceSprite { get; set; }
    public Gender Gender { get; set; }
    public Group? Group { get; set; }
    public GroupBox? GroupBox { get; set; }
    public Guild? Guild { get; set; }
    public string? GuildRank { get; set; }
    public DisplayColor HairColor { get; set; }
    public int HairStyle { get; set; }
    public IgnoreList IgnoreList { get; init; }
    public IInventory Inventory { get; private set; }
    public bool IsAdmin { get; set; }
    public LanternSize LanternSize { get; private set; }
    public Collections.Legend Legend { get; private set; }
    public MailBox MailBox { get; set; } = null!;
    public Nation Nation { get; set; }
    public UserOptions Options { get; init; }
    public byte[] Portrait { get; set; }
    public string ProfileText { get; set; }
    public RestPosition RestPosition { get; set; }
    public IKnowledgeBook<Skill> SkillBook { get; private set; }
    public IKnowledgeBook<Spell> SpellBook { get; private set; }
    public TitleList Titles { get; init; }

    public new AislingTrackers Trackers
    {
        get => (AislingTrackers)base.Trackers;
        private set => base.Trackers = value;
    }

    public UserState UserState { get; set; }
    public UserStatSheet UserStatSheet { get; init; }
    public ResettingCounter ActionThrottle { get; }
    public IInterlockedObject<Dialog> ActiveDialog { get; }
    public IInterlockedObject<object> ActiveObject { get; }

    /// <inheritdoc />
    public override int AssailIntervalMs { get; }

    public ChantTimer ChantTimer { get; }
    public Stack<Dialog> DialogHistory { get; }
    public ResettingCounter ItemThrottle { get; }
    public override ILogger<Aisling> Logger { get; }
    public IIntervalTimer SaveTimer { get; }

    /// <inheritdoc />
    public override IAislingScript Script { get; }

    /// <inheritdoc />
    public override ISet<string> ScriptKeys { get; }

    public bool ShouldWalk
    {
        get
        {
            if (Trackers.LastRefresh.HasValue
                && WorldOptions.Instance.ProhibitF5Walk
                && (DateTime.UtcNow.Subtract(Trackers.LastRefresh.Value)
                            .TotalMilliseconds
                    < 150))
                return false;

            var lastEquipOrUnEquip = Trackers.LastEquipOrUnequip;

            if (lastEquipOrUnEquip.HasValue
                && WorldOptions.Instance.ProhibitItemSwitchWalk
                && (DateTime.UtcNow.Subtract(lastEquipOrUnEquip.Value)
                            .TotalMilliseconds
                    < 150))
                return false;

            if ((Sprite == 0) && WorldOptions.Instance.ProhibitSpeedWalk && !WalkCounter.TryIncrement())
            {
                Logger.WithTopics(
                          [
                              Topics.Entities.Aisling,
                              Topics.Qualifiers.Cheating,
                              Topics.Actions.Walk
                          ])
                      .WithProperty(this)
                      .LogWarning("Aisling {@AislingName} is probably speed walking", Name);

                return false;
            }

            return true;
        }
    }

    public ResettingCounter SkillThrottle { get; }
    public ResettingCounter SpellThrottle { get; }
    public ResettingCounter TurnThrottle { get; }
    public ResettingCounter WalkCounter { get; }

    /// <inheritdoc />
    DisplayColor IDialogSourceEntity.Color => DisplayColor.Default;

    /// <inheritdoc />
    EntityType IDialogSourceEntity.EntityType => EntityType.Aisling;

    public bool ShouldRefresh
        => !Trackers.LastRefresh.HasValue
           || (DateTime.UtcNow.Subtract(Trackers.LastRefresh.Value)
                       .TotalMilliseconds
               > WorldOptions.Instance.RefreshIntervalMs);

    public override StatSheet StatSheet => UserStatSheet;
    public override CreatureType Type => CreatureType.Aisling;

    public Aisling(
        string name,
        MapInstance mapInstance,
        IPoint point,
        IFactory<Exchange> exchangeFactory,
        IScriptProvider scriptProvider,
        ILogger<Aisling> logger,
        ICloningService<Item> itemCloner)
        : this(name, mapInstance, point)
    {
        ExchangeFactory = exchangeFactory;
        Logger = logger;
        ItemCloner = itemCloner;
        SaveTimer = new IntervalTimer(TimeSpan.FromMinutes(WorldOptions.Instance.SaveIntervalMins), false);

        ScriptKeys = new HashSet<string>
        {
            ScriptBase.GetScriptKey(typeof(DefaultAislingScript))
        };
        Script = scriptProvider.CreateScript<IAislingScript, Aisling>(ScriptKeys, this);
    }

    //default user
    public Aisling(
        string name,
        Gender gender,
        int hairStyle,
        DisplayColor hairColor,
        MapInstance mapInstance,
        IPoint point)
        : this(name, mapInstance, point)
    {
        Name = name;
        Gender = gender;
        BodyColor = BodyColor.White;
        BodySprite = Gender == Gender.Male ? BodySprite.Male : BodySprite.Female;
        HairStyle = hairStyle;
        HairColor = hairColor;
        UserStatSheet = UserStatSheet.NewCharacter;

        Titles = [string.Empty];

        ChannelSettings.AddRange(WorldOptions.Instance.DefaultChannels.Select(x => new ChannelSettings(x.ChannelName)));
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
        Titles = [];
        Options = new UserOptions();
        IgnoreList = [];
        Legend = new Collections.Legend();
        Bank = new Bank();
        Equipment = new Equipment();
        Inventory = new Inventory();
        SkillBook = new SkillBook();
        SpellBook = new SpellBook();
        Effects = new EffectsBar(this);
        ActiveObject = new InterlockedObject<object>();
        ActiveDialog = new InterlockedObject<Dialog>();
        ChantTimer = new ChantTimer(WorldOptions.Instance.MaxChantTimeBurdenMs);
        Portrait = [];
        ProfileText = string.Empty;
        ActionThrottle = new ResettingCounter(WorldOptions.Instance.MaxActionsPerSecond);
        SpellThrottle = new ResettingCounter(WorldOptions.Instance.MaxSpellsPerSecond);
        SkillThrottle = new ResettingCounter(WorldOptions.Instance.MaxSkillsPerSecond);
        ItemThrottle = new ResettingCounter(WorldOptions.Instance.MaxItemsPerSecond);
        WalkCounter = new ResettingCounter(4, 2);
        TurnThrottle = new ResettingCounter(3);
        AssailIntervalMs = WorldOptions.Instance.AislingAssailIntervalMs;
        ChannelSettings = [];
        DialogHistory = new Stack<Dialog>();

        Trackers = new AislingTrackers
        {
            Flags = new FlagCollection(),
            Enums = new EnumCollection(),
            TimedEvents = new TimedEventCollection(),
            Counters = new CounterCollection()
        };

        ScriptKeys = new HashSet<string>();

        //this object is purely intended to be created and immediately serialized
        //these pieces should never come into play
        Client = null!;
        Logger = null!;
        ExchangeFactory = null!;
        ItemCloner = null!;
        Script = null!;
        SaveTimer = null!;
    }

    /// <inheritdoc />
    void IDialogSourceEntity.Activate(Aisling source) => Script.OnClicked(source);

    /// <inheritdoc />
    public bool IsIgnoring(string name) => IgnoreList.Contains(name);

    /// <inheritdoc />
    public void SendMessage(string message) => SendActiveMessage(message);

    public void BeginObserving()
    {
        //add observers
        var inventoryObserver = new InventoryObserver(this);
        var spellBookObserver = new SpellBookObserver(this);
        var skillBookObserver = new SkillBookObserver(this);
        var equipmentObserver = new EquipmentObserver(this);

        BeginObservingPanel(Inventory, inventoryObserver);
        BeginObservingPanel(SpellBook, spellBookObserver);
        BeginObservingPanel(SkillBook, skillBookObserver);
        BeginObservingPanel(Equipment, equipmentObserver);

        foreach (var effect in Effects)
        {
            effect.Subject = this;
            effect.OnReApplied();
        }

        Effects.ResetDisplay();
    }

    private void BeginObservingPanel<T>(IPanel<T> panel, Observers.Abstractions.IObserver<T> observer)
    {
        panel.AddObserver(observer);

        var objs = panel.ToList();
        panel.Clear();

        foreach (var obj in objs)
        {
            panel.ForceAdd(obj);
            observer.OnAdded(obj);

            switch (obj)
            {
                case Spell { Elapsed: not null } spell:
                    Client.SendCooldown(spell);

                    break;
                case Skill { Elapsed: not null, Template.IsAssail: false } skill:
                    Client.SendCooldown(skill);

                    break;
            }
        }
    }

    public bool CanCarry(params IEnumerable<Item> items) => CanCarry(items.Select(item => (item, item.Count)));

    public bool CanCarry(params IEnumerable<(Item Item, int Count)> hypotheticalItems)
    {
        var weightSum = 0;
        var slotSum = 0;

        //group all separated stacks together by summing their counts
        foreach (var set in hypotheticalItems.GroupBy(
                     set => set.Item.DisplayName,
                     (_, grp) =>
                     {
                         var col = grp.ToList();

                         return (col.First()
                                    .Item, Count: col.Sum(i => i.Count));
                     }))
        {
            var weightlessAllowance = 0;

            //for stackable items, we can fill the existing stacks in our inventory without adding any weight
            if (set.Item.Template.Stackable)
            {
                //the number of existing stacks in our inventory
                var numUniqueStacks = Inventory.Count(i => i.DisplayName.EqualsI(set.Item.DisplayName));

                //the total count in all stacks of this item
                var totalCount = Inventory.CountOf(set.Item.DisplayName);

                //the maximum number of items we can have in all stacks of this item
                var maxCount = set.Item.Template.MaxStacks * numUniqueStacks;

                //if we have any stacks of this item, we can fill them up without adding any weight
                //if we don't have any stacks of this item, we can fill one stack without adding any weight
                //this prevents someone from having multiple stacks of an item, but does not interfere with existing multiple stacks
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

        return ((UserStatSheet.CurrentWeight + weightSum) <= UserStatSheet.MaxWeight) && (Inventory.AvailableSlots >= slotSum);
    }

    /// <inheritdoc />
    public override bool CanObserve(VisibleEntity entity, bool fullCheck = false)
    {
        /*if (IsAdmin)
            return true;*/

        //can always see yourself
        if (entity.Equals(this))
            return true;

        if (!fullCheck)
            return ApproachTime.ContainsKey(entity);

        if ((entity.ManhattanDistanceFrom(this) > 1) && !MapInstance.IsInSharedLanternVision(entity))
            return false;

        if (Vision == VisionType.TrueBlind)
            return false;

        return base.CanObserve(entity, fullCheck);
    }

    public override bool CanSee(VisibleEntity entity)
    {
        if (IsAdmin)
            return true;

        return base.CanSee(entity);
    }

    /// <inheritdoc />
    public override bool CanUse(Skill skill, [MaybeNullWhen(false)] out ActivationContext skillContext)
    {
        skillContext = null;

        if (!skill.Template.IsAssail && (!ActionThrottle.CanIncrement || !SkillThrottle.CanIncrement))
            return false;

        return base.CanUse(skill, out skillContext!);
    }

    /// <inheritdoc />
    public override bool CanUse(
        Spell spell,
        Creature target,
        string? prompt,
        [MaybeNullWhen(false)] out SpellContext spellContext)
    {
        spellContext = null;

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

    public bool CanUse(Item item)
        => Script.CanUseItem(item) && ActionThrottle.CanIncrement && ItemThrottle.CanIncrement && item.CanUse() && item.Script.CanUse(this);

    public void Equip(EquipmentType type, Item item)
    {
        var slot = item.Slot;

        //try equip,
        if (Equipment.TryEquip(type, item, out var returnedItem))
        {
            Inventory.Remove(slot);

            if (returnedItem != null)
            {
                Trackers.LastUnequip = DateTime.UtcNow;
                Inventory.TryAddToNextSlot(returnedItem);
            }

            Trackers.LastEquip = DateTime.UtcNow;
        }
    }

    public int GetLanternRadius()
        => LanternSize switch
        {
            LanternSize.Small => 3,
            LanternSize.Large => 5,
            _                 => throw new ArgumentOutOfRangeException()
        };

    public void GiveItemOrSendToBank(Item item)
    {
        var items = item.FixStacks(ItemCloner);

        foreach (var single in items)
            if (!CanCarry(single) || !Inventory.TryAddToNextSlot(single))
            {
                Bank.Deposit(single);

                SendOrangeBarMessage($"{single.DisplayName} was sent to your bank as overflow");
            }
    }

    /// <summary>
    ///     Determines whether or not an aisling's class counts as being a certain class
    /// </summary>
    /// <param name="class">
    ///     The other baseClass
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the aisling's class contains <paramref name="class" />, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool HasClass(BaseClass @class)
        => @class is BaseClass.Peasant
           || UserStatSheet.BaseClass switch
           {
               //Diacht "is" all classes
               BaseClass.Diacht => true,
               _                => UserStatSheet.BaseClass == @class
           };

    public bool Illuminates(VisibleEntity entity)
    {
        if (!MapInstance.Flags.HasFlag(MapFlags.Darkness))
            return true;

        if (LanternSize is LanternSize.None)
            return false;

        var radius = GetLanternRadius();
        var distance = Math.Ceiling(entity.EuclideanDistanceFrom(this));

        return distance <= radius;
    }

    public void Initialize(
        string name,
        Bank bank,
        Equipment equipment,
        Inventory inventory,
        SkillBook skillBook,
        SpellBook spellBook,
        Collections.Legend legend,
        EffectsBar effects,
        AislingTrackers aislingTrackers)
    {
        Name = name;
        Bank = bank;
        Equipment = equipment;
        Inventory = inventory;
        SkillBook = skillBook;
        SpellBook = spellBook;
        Legend = legend;
        Effects = effects;
        Trackers = aislingTrackers;
    }

    public override void OnClicked(Aisling source)
    {
        if (!ShouldRegisterClick(source.Id))
            return;

        if (source.Equals(this))
        {
            source.Client.SendSelfProfile();

            LastClicked[source.Id] = DateTime.UtcNow;
            Script.OnClicked(source);
        } else
        {
            source.Client.SendOtherProfile(this);

            LastClicked[source.Id] = DateTime.UtcNow;
            Script.OnClicked(source);
        }
    }

    public override void OnGoldDroppedOn(Aisling source, int amount)
    {
        if (!TryStartExchange(source, out var exchange))
            return;

        exchange.SetGold(source, amount);
    }

    public override void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        if (source.Inventory.TryGetObject(slot, out var inventoryItem))
            if (!Script.CanDropItemOn(source, inventoryItem))
            {
                source.SendActiveMessage("You can't trade that item");

                return;
            }

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

        if (!forceRefresh && !ShouldRefresh)
            return;

        Trackers.LastRefresh = now;
        Client.SendMapInfo();
        Client.SendLocation();
        Client.SendAttributes(StatUpdateType.Full);

        UpdateViewPort(null, true);

        Client.SendMapLoadComplete();
        Client.SendDisplayAisling(this);
        Client.SendRefreshResponse();
        Client.SendLightLevel(MapInstance.CurrentLightLevel);

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(this)
                                           .ToList())
            reactor.OnWalkedOn(this);
    }

    public void SendActiveMessage(string message) => SendServerMessage(ServerMessageType.ActiveMessage, message);

    public void SendOrangeBarMessage(string message) => SendServerMessage(ServerMessageType.OrangeBar1, message);

    public void SendPersistentMessage(string message) => SendServerMessage(ServerMessageType.PersistentMessage, message);

    public void SendServerMessage(ServerMessageType serverMessageType, string message)
    {
        if ((message.Length < CONSTANTS.MAX_MESSAGE_LINE_LENGTH)
            || serverMessageType is ServerMessageType.WoodenBoard
                                    or ServerMessageType.ScrollWindow
                                    or ServerMessageType.NonScrollWindow
                                    or ServerMessageType.UserOptions)
            Client.SendServerMessage(serverMessageType, message);
        else
            foreach (var msg in message.Chunk(CONSTANTS.MAX_MESSAGE_LINE_LENGTH))
                Client.SendServerMessage(serverMessageType, new string(msg));
    }

    public void SetLanternSize(LanternSize lanternSize)
    {
        LanternSize = lanternSize;

        MapInstance.UpdateNearbyViewPorts(this);
        Display();
    }

    public void SetSprite(ushort sprite)
    {
        Sprite = sprite;

        Refresh(true);
        Display();
    }

    public override void SetVision(VisionType visionType)
    {
        if (visionType == Vision)
            return;

        var currentVisionType = Vision;

        Vision = visionType;

        //if we were TrueBlinded or are now TrueBlinded, we need to refresh
        if (visionType is VisionType.TrueBlind || currentVisionType is VisionType.TrueBlind)
            Client.Aisling.Refresh(true);
        else
            Client.SendAttributes(StatUpdateType.Secondary);
    }

    /// <inheritdoc />
    public override void ShowPublicMessage(PublicMessageType publicMessageType, string message)
    {
        if (!Script.CanTalk())
            return;

        Logger.WithTopics(
                  [
                      Topics.Entities.Aisling,
                      Topics.Entities.Message,
                      Topics.Actions.Send
                  ])
              .WithProperty(this)
              .LogInformation(
                  "Aisling {@AislingName} sent {@Type} message {@Message}",
                  Name,
                  publicMessageType,
                  message);

        base.ShowPublicMessage(publicMessageType, message);
    }

    public override void ShowTo(Aisling aisling) => aisling.Client.SendDisplayAisling(this);

    public bool TryDrop(
        IPoint point,
        byte slot,
        [MaybeNullWhen(false)]

        // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Global
        out GroundItem[] groundItems,
        int? amount = null)
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
                if (TryDrop(point, items.FixStacks(ItemCloner), out groundItems))
                {
                    if (item.Template.NoTrade)
                        foreach (var groundItem in groundItems)
                            groundItem.LockToAislings(int.MaxValue, this);

                    return true;
                }
        } else
        {
            if (Inventory.TryGetRemove(slot, out var droppedItem))
                if (TryDrop(point, out groundItems, droppedItem))
                {
                    if (item.Template.NoTrade)
                        foreach (var groundItem in groundItems)
                            groundItem.LockToAislings(int.MaxValue, this);

                    return true;
                }
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
        MapInstance.AddEntity(money, point);

        Logger.WithTopics(
                  [
                      Topics.Entities.Aisling,
                      Topics.Entities.Gold,
                      Topics.Actions.Drop
                  ])
              .WithProperty(this)
              .WithProperty(money)
              .LogInformation(
                  "Aisling {@AislingName} dropped {Amount} gold at {@Location}",
                  Name,
                  money.Amount,
                  ILocation.ToString(money));

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(money)
                                           .ToList())
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

    public bool TryGiveItem(ref Item item, byte? slot = null)
    {
        if (!CanCarry(item))
        {
            SendOrangeBarMessage("You can't carry that");

            return false;
        }

        if (slot.HasValue)
        {
            if (Inventory.TryAdd(slot.Value, item))
            {
                if (item.Template.Stackable)
                    item = Inventory[item.DisplayName]!;

                return true;
            }

            return false;
        }

        if (Inventory.TryAddToNextSlot(item))
        {
            if (item.Template.Stackable)
                item = Inventory[item.DisplayName]!;

            return true;
        }

        return false;
    }

    public bool TryGiveItems(params ICollection<Item> items)
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

    public bool TryPickupItem(GroundItem groundItem, byte destinationSlot)
    {
        if (!groundItem.CanPickUp(this))
        {
            SendActiveMessage("You can't pick that up right now");

            return false;
        }

        var item = groundItem.Item;
        var originalItem = item;
        var originalCount = originalItem.Count;

        if (TryGiveItem(ref item, destinationSlot))
        {
            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Actions.Pickup
                      ])
                  .WithProperty(this)
                  .WithProperty(groundItem)
                  .LogInformation(
                      "Aisling {@AislingName} picked up {Amount} {@ItemName}",
                      Name,
                      originalCount,
                      groundItem.Name);

            MapInstance.RemoveEntity(groundItem);
            item.Script.OnPickup(this, originalItem, originalCount);

            foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(groundItem)
                                               .ToList())
                reactor.OnItemPickedUpFrom(this, groundItem, originalCount);

            return true;
        }

        return false;
    }

    public bool TryPickupMoney(Money money)
    {
        if (!money.CanPickUp(this))
        {
            SendActiveMessage("You can't pick that up right now");

            return false;
        }

        if (TryGiveGold(money.Amount))
        {
            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Gold,
                          Topics.Actions.Pickup
                      ])
                  .WithProperty(this)
                  .WithProperty(money)
                  .LogInformation("Aisling {@AislingName} picked up {Amount} gold", Name, money.Amount);

            MapInstance.RemoveEntity(money);

            foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(money)
                                               .ToList())
                reactor.OnGoldPickedUpFrom(this, money);
        }

        return false;
    }

    private bool TryStartExchange(Aisling source, [MaybeNullWhen(false)] out Exchange exchange)
    {
        exchange = ExchangeFactory.Create(source, this);

        if (!source.Options.AllowExchange)
        {
            source.SendActiveMessage("You have disabled exchanging");
            exchange = null;

            return false;
        }

        if (IsIgnoring(source.Name))
        {
            source.SendActiveMessage($"{Name} has disabled exchanging");

            Logger.WithTopics(
                      [
                          Topics.Entities.Aisling,
                          Topics.Entities.Exchange,
                          Topics.Qualifiers.Harassment
                      ])
                  .WithProperty(this)
                  .WithProperty(source)
                  .LogWarning(
                      "{@AislingName} is trying to exchange with {@TargetName}, but is ignored (potential harassment)",
                      source.Name,
                      Name);

            exchange = null;

            return false;
        }

        if (!Options.AllowExchange)
        {
            source.SendActiveMessage($"{Name} has disabled exchanging");
            SendActiveMessage($"{source.Name} is trying to exchange with you, but it is disabled");

            exchange = null;

            return false;
        }

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

        if (!ActionThrottle.TryIncrement())
            return false;

        if (!ItemThrottle.TryIncrement())
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
        Trackers.LastSkillUse = DateTime.UtcNow;
        Trackers.LastUsedSkill = skill;

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
        } else if (!MapInstance.TryGetEntity(targetId.Value, out target))
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
        Trackers.LastSpellUse = DateTime.UtcNow;
        Trackers.LastUsedSpell = spell;

        return true;
    }

    public bool TryUseSpell(byte slot, uint? targetId = null, string? prompt = null)
    {
        if (!SpellBook.TryGetObject(slot, out var spell))
            return false;

        return TryUseSpell(spell, targetId, prompt);
    }

    /// <inheritdoc />
    public override void Turn(Direction direction)
    {
        if (!Script.CanTurn() || !TurnThrottle.TryIncrement())
            return;

        Direction = direction;

        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanObserve(this))
            aisling.Client.SendCreatureTurn(Id, direction);

        Trackers.LastTurn = DateTime.UtcNow;
    }

    public void UnEquip(EquipmentSlot slot)
    {
        if (Inventory.IsFull)
            return;

        if (!Equipment.TryGetRemove((byte)slot, out var item))
            return;

        Inventory.TryAddToNextSlot(item);
        Trackers.LastUnequip = DateTime.UtcNow;
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
        ItemThrottle.Update(delta);
        TurnThrottle.Update(delta);
        WalkCounter.Update(delta);
        ChantTimer.Update(delta);
        SaveTimer.Update(delta);

        base.Update(delta);
    }

    /// <inheritdoc />
    public override void UpdateViewPort(HashSet<VisibleEntity>? partialUpdateEntities = null, bool refresh = false)
    {
        Dictionary<VisibleEntity, DateTime>? stashedApproachTime = null;

        if (refresh)
        {
            stashedApproachTime = ApproachTime.ToDictionary();
            ApproachTime.Clear();
        }

        //if entitiestoCheck is not null, only do a partial viewport update
        var previouslyObservable = partialUpdateEntities is null
            ? ApproachTime.Keys.ToHashSet()
            : partialUpdateEntities.Where(entity => ApproachTime.ContainsKey(entity))
                                   .ToHashSet();

        var currentlyObservable = partialUpdateEntities is null
            ? MapInstance.GetEntitiesWithinRange<VisibleEntity>(this)
                         .ThatAreObservedBy(this, true)
                         .ToHashSet()
            : partialUpdateEntities.ThatAreWithinRange(this)
                                   .Where(e => MapInstance.TryGetEntity<WorldEntity>(e.Id, out _)) //make sure they are still on the map
                                   .ThatAreObservedBy(this, true)
                                   .ToHashSet();

        var entitiesToSend = new List<VisibleEntity>();
        var doorsToSend = new HashSet<Door>();

        foreach (var entity in previouslyObservable)
            if (!currentlyObservable.Contains(entity))
            {
                if (entity.Equals(this))
                    continue;

                entity.HideFrom(this);
                OnDeparture(entity, refresh);
            }

        foreach (var entity in currentlyObservable)
            if (!previouslyObservable.Contains(entity))
            {
                if (entity.Equals(this))
                    continue;

                switch (entity)
                {
                    case Aisling:
                        entity.ShowTo(this);

                        break;
                    case Door door:
                        doorsToSend.AddRange(door.GetCluster());

                        break;
                    default:
                        entitiesToSend.Add(entity);

                        break;
                }

                OnApproached(entity, refresh);
            }

        Client.SendVisibleEntities(entitiesToSend);
        Client.SendDoors(doorsToSend);

        if (refresh && stashedApproachTime is not null)
            foreach (var kvp in stashedApproachTime)
                if (ApproachTime.ContainsKey(kvp.Key))
                    ApproachTime[kvp.Key] = kvp.Value;
    }

    public override void Walk(Direction direction, bool? ignoreBlockingReactors = null)
    {
        ignoreBlockingReactors ??= true;

        if (!Script.CanMove() || ((direction != Direction) && !Script.CanTurn()) || !ShouldWalk)
        {
            Refresh(true);

            return;
        }

        Direction = direction;
        var startPosition = Location.From(this);
        var startPoint = Point.From(this);
        var endPoint = this.DirectionalOffset(direction);

        //if admin, just check if we're within the map
        if (IsAdmin)
        {
            if (!MapInstance.IsWithinMap(endPoint))
            {
                Refresh(true);

                return;
            }
        }

        //otherwise, check if the point is walkable
        else if (!MapInstance.IsWalkable(endPoint, Type, ignoreBlockingReactors))
        {
            Refresh(true);

            return;
        }

        SetLocation(endPoint);
        Trackers.LastWalk = DateTime.UtcNow;
        Trackers.LastPosition = startPosition;

        var creaturesToUpdate = MapInstance.GetEntitiesWithinRange<Creature>(startPoint, 16)
                                           .ThatAreWithinRange(
                                               points:
                                               [
                                                   startPoint,
                                                   endPoint
                                               ])
                                           .ToList();

        foreach (var creature in creaturesToUpdate)
            if (creature is Aisling)
                creature.UpdateViewPort();
            else
                creature.UpdateViewPort([this]);

        var aislingsThatWatchedUsWalk = creaturesToUpdate.ThatAreWithinRange(startPoint)
                                                         .ThatAreWithinRange(endPoint)
                                                         .ThatCanObserve(this)
                                                         .OfType<Aisling>();

        foreach (var aisling in aislingsThatWatchedUsWalk)
            if (!aisling.Equals(this))
                aisling.Client.SendCreatureWalk(Id, startPoint, direction);

        Client.SendClientWalkResponse(startPoint, direction);

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(this)
                                           .ToList())
            reactor.OnWalkedOn(this);

        var startOnWater = MapInstance.Template.Tiles[startPosition.X, startPosition.Y].IsWater;
        var endOnWater = MapInstance.Template.Tiles[endPoint.X, endPoint.Y].IsWater;

        //if we transition between water / nonwater tiles
        //send attributes to update the water walking status
        if (startOnWater != endOnWater)
            Client.SendAttributes(StatUpdateType.Full);
    }

    /// <inheritdoc />
    public override void WarpTo(IPoint destinationPoint)
    {
        var startPoint = Location.From(this);
        SetLocation(destinationPoint);
        Trackers.LastPosition = startPoint;

        var creaturesToUpdate = MapInstance.GetEntitiesWithinRange<Creature>(startPoint)
                                           .Union(MapInstance.GetEntitiesWithinRange<Creature>(destinationPoint))
                                           .ToList();

        foreach (var creature in creaturesToUpdate)
            if (creature is Aisling)
                creature.UpdateViewPort();
            else
                creature.UpdateViewPort([this]);

        var aislingsThatWatchedUsWarp = creaturesToUpdate.ThatAreWithinRange(startPoint)
                                                         .ThatAreWithinRange(destinationPoint)
                                                         .ThatCanObserve(this)
                                                         .OfType<Aisling>();

        foreach (var aisling in aislingsThatWatchedUsWarp)
        {
            HideFrom(aisling);
            ShowTo(aisling);
        }

        //refresh will activate reactors, don't double up
        Refresh(true);
    }
}