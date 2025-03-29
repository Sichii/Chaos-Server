#region
using System.Runtime.CompilerServices;
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.CreatureScripts.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.NaturalRegeneration;
using Chaos.Services.Servers.Options;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Models.World.Abstractions;

public abstract class Creature : NamedEntity, IAffected, IScripted<ICreatureScript>
{
    public Direction Direction { get; set; }
    public IEffectsBar Effects { get; protected set; }
    public int GamePoints { get; set; }
    public int Gold { get; set; }
    public virtual bool IsDead { get; set; }
    public Trackers Trackers { get; set; }

    public VisionType Vision { get; protected set; }
    public Dictionary<VisibleEntity, DateTime> ApproachTime { get; }
    public abstract int AssailIntervalMs { get; }
    public abstract ILogger Logger { get; }
    public IIntervalTimer RegenTimer { get; }

    /// <inheritdoc />
    public abstract ICreatureScript Script { get; }

    /// <inheritdoc />
    public abstract ISet<string> ScriptKeys { get; }

    public abstract StatSheet StatSheet { get; }
    public abstract CreatureType Type { get; }
    public int EffectiveAssailIntervalMs => StatSheet.CalculateEffectiveAssailInterval(AssailIntervalMs);
    public virtual bool IsAlive => StatSheet.CurrentHp > 0;
    public virtual bool IsBlind => Vision is not VisionType.Normal;

    protected Creature(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point)
        : base(
            name,
            sprite,
            mapInstance,
            point)
    {
        Direction = Direction.Down;
        Effects = new EffectsBar(this);
        RegenTimer = new RegenTimer(this, DefaultNaturalRegenerationScript.Create());
        ApproachTime = new Dictionary<VisibleEntity, DateTime>();
        Trackers = new Trackers();
    }

    public override void Update(TimeSpan delta)
    {
        Effects.Update(delta);
        RegenTimer.Update(delta);
        Script.Update(delta);
        Trackers.Update(delta);
    }

    /// <inheritdoc />
    public override void Animate(Animation animation, uint? sourceId = null)
    {
        var targetedAnimation = animation.GetTargetedAnimation(Id, sourceId);

        foreach (var obj in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                       .ThatCanObserve(this))
            obj.Client.SendAnimation(targetedAnimation);
    }

    public virtual void AnimateBody(BodyAnimation bodyAnimation, ushort speed = 25, byte? sound = null)
    {
        if (bodyAnimation is BodyAnimation.None)
            return;

        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)

                                           //intentional... if you're observable but still invisible
                                           //we dont want the body animation to show
                                           .ThatCanSee(this))
            aisling.Client.SendBodyAnimation(
                Id,
                bodyAnimation,
                speed,
                sound);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual bool CanObserve(VisibleEntity entity, bool fullCheck = false)
    {
        //can always see yourself
        if (entity.Equals(this))
            return true;

        if (!fullCheck)
            return ApproachTime.ContainsKey(entity);

        if (Vision == VisionType.TrueBlind)
            return false;

        switch (entity.Visibility)
        {
            case VisibilityType.Normal:
            case VisibilityType.Hidden:
                return true;
            case VisibilityType.TrueHidden when this is Aisling aisling:
                if (aisling.Group is not null && aisling.Group.Any(member => member.Equals(entity)))
                    return true;

                goto case VisibilityType.TrueHidden;
            case VisibilityType.TrueHidden:
                return Script.CanSee(entity);
            case VisibilityType.GmHidden when this is Aisling aisling:
                return aisling.IsAdmin;
            case VisibilityType.GmHidden:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public virtual bool CanSee(VisibleEntity entity)
    {
        if (entity.Equals(this))
            return true;

        return Script.CanSee(entity);
    }

    public virtual bool CanUse(Skill skill, [MaybeNullWhen(false)] out ActivationContext skillContext)
    {
        skillContext = null;

        if (!Script.CanUseSkill(skill))
            return false;

        if (!skill.CanUse())
            return false;

        skillContext = new ActivationContext(this, this);

        return skill.Script.CanUse(skillContext);
    }

    public virtual bool CanUse(
        Spell spell,
        Creature target,
        string? promptResponse,
        [MaybeNullWhen(false)] out SpellContext spellContext)
    {
        spellContext = null;

        if (!Script.CanUseSpell(spell))
            return false;

        if (!spell.CanUse())
            return false;

        spellContext = new SpellContext(this, target, promptResponse);

        return spell.Script.CanUse(spellContext);
    }

    public virtual void Chant(string message) => ShowPublicMessage(PublicMessageType.Chant, message);

    public Stack<IPoint> FindPath(IPoint target, IPathOptions? pathOptions = null)
    {
        pathOptions ??= PathOptions.Default with
        {
            IgnoreWalls = Type == CreatureType.WalkThrough
        };

        var nearbyDoors = MapInstance.GetEntitiesWithinRange<Door>(this)
                                     .Where(door => door.Closed);

        var nearbyCreatures = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                         .ThatThisCollidesWith(this);

        var nearbyUnwalkablePoints = nearbyDoors.Concat<IPoint>(nearbyCreatures)
                                                .Concat(pathOptions.BlockedPoints)
                                                .ToHashSet();

        pathOptions.BlockedPoints = nearbyUnwalkablePoints;

        return MapInstance.Pathfinder.FindPath(
            MapInstance.InstanceId,
            this,
            target,
            pathOptions);
    }

    private void HandleAdminVerbalCommand(Aisling source, string message)
    {
        if (message.EqualsI("show ids"))
        {
            foreach (var creature in MapInstance.GetEntitiesWithinRange<Creature>(this))
                if (!creature.Equals(this))
                    source.Client.SendDisplayPublicMessage(creature.Id, PublicMessageType.Normal, creature.Name);

            foreach (var groundItem in MapInstance.GetEntitiesWithinRange<GroundItem>(this))
                source.Client.SendDisplayPublicMessage(groundItem.Id, PublicMessageType.Normal, groundItem.Name);
        } else if (message.EqualsI("show keys"))
        {
            foreach (var creature in MapInstance.GetEntitiesWithinRange<Creature>(this))
                if (!creature.Equals(this))
                    switch (creature)
                    {
                        case Merchant merchant:
                            source.Client.SendDisplayPublicMessage(creature.Id, PublicMessageType.Normal, merchant.Template.TemplateKey);

                            break;
                        case Monster monster:
                            source.Client.SendDisplayPublicMessage(creature.Id, PublicMessageType.Normal, monster.Template.TemplateKey);

                            break;
                    }

            foreach (var groundItem in MapInstance.GetEntitiesWithinRange<GroundItem>(this))
                source.Client.SendDisplayPublicMessage(groundItem.Id, PublicMessageType.Normal, groundItem.Item.Template.TemplateKey);
        }
    }

    public virtual void HandleMapDeparture()
    {
        foreach (var entity in ApproachTime.Keys)
            OnDeparture(entity, true);
    }

    public virtual bool IsFriendlyTo(Creature other) => Script.IsFriendlyTo(other);

    public virtual bool IsHostileTo(Creature other) => Script.IsHostileTo(other);

    public virtual void OnApproached(VisibleEntity entity, bool refresh = false)
    {
        if (Equals(entity))
            return;

        ApproachTime.TryAdd(entity, DateTime.UtcNow);

        if (!refresh && entity is Creature c)
            Script.OnApproached(c);
    }

    public override void OnClicked(Aisling source)
    {
        if (!ShouldRegisterClick(source.Id))
            return;

        LastClicked[source.Id] = DateTime.UtcNow;
        Script.OnClicked(source);
    }

    public virtual void OnDeparture(VisibleEntity entity, bool refresh = false)
    {
        if (Equals(entity))
            return;

        ApproachTime.Remove(entity);

        if (!refresh && entity is Creature c)
            Script.OnDeparture(c);
    }

    public virtual void OnGoldDroppedOn(Aisling source, int amount)
    {
        if (!this.WithinRange(source, WorldOptions.Instance.TradeRange))
            return;

        if (!Script.CanDropMoneyOn(source, amount))
        {
            source.SendActiveMessage("You can't do that right now");

            return;
        }

        if (source.TryTakeGold(amount))
        {
            Gold += amount;
            source.Client.SendAttributes(StatUpdateType.ExpGold);
            Script.OnGoldDroppedOn(source, amount);

            Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Gold, Topics.Actions.Drop)
                  .WithProperty(source)
                  .LogInformation(
                      "Aisling {@AislingName} dropped {Amount} gold on creature {@CreatureName}",
                      source.Name,
                      amount,
                      Name);
        }
    }

    public virtual void OnItemDroppedOn(Aisling source, byte slot, byte count)
    {
        if (count == 0)
            count = 1;

        if (!this.WithinRange(source, WorldOptions.Instance.TradeRange))
            return;

        if (!source.Inventory.TryGetObject(slot, out var inventoryItem) || (inventoryItem.Count < count))
            return;

        if (!Script.CanDropItemOn(source, inventoryItem) || !inventoryItem.Script.CanBeDroppedOn(source, this))
        {
            source.SendActiveMessage("You can't trade that item");

            return;
        }

        if (source.Inventory.RemoveQuantity(slot, count, out var items))
            foreach (var item in items)
            {
                Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Item, Topics.Actions.Drop)
                      .WithProperty(source)
                      .WithProperty(item)
                      .WithProperty(this)
                      .LogInformation(
                          "Aisling {@AislingName} dropped item {@ItemName} on creature {@CreatureName}",
                          source.Name,
                          item.DisplayName,
                          Name);

                if (this is Monster monster)
                    monster.Items.Add(item);

                Script.OnItemDroppedOn(source, item);
            }
    }

    public void Pathfind(IPoint target, int distance = 1, IPathOptions? pathOptions = null)
    {
        pathOptions ??= PathOptions.Default with
        {
            IgnoreWalls = Type == CreatureType.WalkThrough
        };

        //if we're within distance, no need to pathfind
        if (this.ManhattanDistanceFrom(target) <= distance)
            return;

        var path = FindPath(target, pathOptions);

        var nextPoint = path.Pop();
        var direction = nextPoint.DirectionalRelationTo(this);

        Walk(direction, pathOptions.IgnoreBlockingReactors, pathOptions.IgnoreWalls);
    }

    public virtual void Say(string message) => ShowPublicMessage(PublicMessageType.Normal, message);

    /// <inheritdoc />
    public override void SetVisibility(VisibilityType newVisibilityType)
    {
        if (Visibility == newVisibilityType)
            return;

        Visibility = newVisibilityType;

        foreach (var creature in MapInstance.GetEntitiesWithinRange<Creature>(this))
            if (!creature.Equals(this))
                creature.UpdateViewPort([this]);

        Display();
    }

    public virtual void SetVision(VisionType visionType) => Vision = visionType;

    public virtual void Shout(string message) => ShowPublicMessage(PublicMessageType.Shout, message);

    public virtual void ShowHealth(byte? sound = null)
    {
        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)

                                           //intentional... if you're observable but still invisible, health bar shouldnt show
                                           .ThatCanSee(this))
            aisling.Client.SendHealthBar(this, sound);
    }

    public virtual void ShowPublicMessage(PublicMessageType publicMessageType, string message)
    {
        if (!Script.CanTalk())
            return;

        IEnumerable<Creature>? creaturesWithinRange;
        var sendMessage = message;

        switch (publicMessageType)
        {
            case PublicMessageType.Normal:
                creaturesWithinRange = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                                  .ThatCanObserve(this);
                sendMessage = $"{Name}: {message}";

                break;
            case PublicMessageType.Shout:
                creaturesWithinRange = MapInstance.GetEntities<Creature>();
                sendMessage = $"{Name}! {message}";

                break;
            case PublicMessageType.Chant:
                creaturesWithinRange = MapInstance.GetEntities<Creature>()
                                                  .ThatCanObserve(this);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(publicMessageType), publicMessageType, null);
        }

        if (this is Aisling && (sendMessage.Length > CONSTANTS.MAX_MESSAGE_LINE_LENGTH))
            sendMessage = sendMessage[..CONSTANTS.MAX_MESSAGE_LINE_LENGTH];

        creaturesWithinRange = creaturesWithinRange.ToList();

        //separated so merchant replies show up below the text theyre responding to
        foreach (var aisling in creaturesWithinRange.OfType<Aisling>())
            if (!aisling.IgnoreList.Contains(Name))
                aisling.Client.SendDisplayPublicMessage(Id, publicMessageType, sendMessage);

        Trackers.LastTalk = DateTime.UtcNow;

        foreach (var creature in creaturesWithinRange)
            creature.Script.OnPublicMessage(this, message);
    }

    /// <summary>
    ///     Attempts to move this entity from its current map to the destination map
    /// </summary>
    /// <remarks>
    ///     This method does not use the MapInstance.BeginInvoke api due to not knowing who the caller is. The caller could be
    ///     from another map, which would require entrancy into this entity's map synchronization, but there is no way of
    ///     knowing where the caller is from. In order for this to be safe, it has to make no assumptions and deal with every
    ///     requirement itself.
    /// </remarks>
    public virtual void TraverseMap(
        MapInstance destinationMap,
        IPoint destinationPoint,
        bool ignoreSharding = false,
        bool fromWolrdMap = false,
        Func<Task>? onTraverse = null)
        => Task.Run<Task>(
            async () =>
            {
                var currentMap = MapInstance;

                var aisling = this as Aisling;

                if (aisling is not null)
                    await aisling.Client.ReceiveSync.WaitAsync();

                try
                {
                    await using var sync = await ComplexSynchronizationHelper.WaitAsync(
                        TimeSpan.FromMilliseconds(500),
                        TimeSpan.FromMilliseconds(3),
                        currentMap.Sync,
                        destinationMap.Sync);

                    if (!fromWolrdMap && !currentMap.RemoveEntity(this))
                        return;

                    if (currentMap.InstanceId != destinationMap.InstanceId)
                        Trackers.LastMapInstanceId = currentMap.InstanceId;

                    if (aisling is not null && ignoreSharding)
                        destinationMap.AddAislingDirect(aisling, destinationPoint);
                    else
                        destinationMap.AddEntity(this, destinationPoint);

                    if (onTraverse is not null)
                        await onTraverse();
                } catch (Exception e)
                {
                    Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.Creature, Topics.Actions.Traverse)
                          .WithProperty(this)
                          .WithProperty(currentMap)
                          .WithProperty(destinationMap)
                          .LogError(
                              e,
                              "Exception thrown while creature {@CreatureName} attempted to traverse from map {@FromMapInstanceId} to map {@ToMapInstanceId}",
                              Name,
                              currentMap.InstanceId,
                              destinationMap.InstanceId);
                } finally
                {
                    aisling?.Client.ReceiveSync.Release();
                }
            });

    /// <summary>
    ///     Attempts to move this entity from its current map to the destination map
    /// </summary>
    /// <remarks>
    ///     This method does not use the MapInstance.BeginInvoke api due to not knowing who the caller is. The caller could be
    ///     from another map, which would require entrancy into this entity's map synchronization, but there is no way of
    ///     knowing where the caller is from. In order for this to be safe, it has to make no assumptions and deal with every
    ///     requirement itself.
    /// </remarks>
    public virtual async Task TraverseMapAsync(
        MapInstance destinationMap,
        IPoint destinationPoint,
        bool ignoreSharding = false,
        bool fromWolrdMap = false,
        Func<Task>? onTraverse = null)
    {
        var currentMap = MapInstance;

        var aisling = this as Aisling;

        if (aisling is not null)
            await aisling.Client.ReceiveSync.WaitAsync();

        try
        {
            await using var sync = await ComplexSynchronizationHelper.WaitAsync(
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromMilliseconds(3),
                currentMap.Sync,
                destinationMap.Sync);

            if (!fromWolrdMap && !currentMap.RemoveEntity(this))
                return;

            if (currentMap.InstanceId != destinationMap.InstanceId)
                Trackers.LastMapInstanceId = currentMap.InstanceId;

            if (aisling is not null && ignoreSharding)
                destinationMap.AddAislingDirect(aisling, destinationPoint);
            else
                destinationMap.AddEntity(this, destinationPoint);

            if (onTraverse is not null)
                await onTraverse();
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.MapInstance, Topics.Entities.Creature, Topics.Actions.Traverse)
                  .WithProperty(this)
                  .WithProperty(currentMap)
                  .WithProperty(destinationMap)
                  .LogError(
                      e,
                      "Exception thrown while creature {@CreatureName} attempted to traverse from map {@FromMapInstanceId} to map {@ToMapInstanceId}",
                      Name,
                      currentMap.InstanceId,
                      destinationMap.InstanceId);
        } finally
        {
            aisling?.Client.ReceiveSync.Release();
        }
    }

    public virtual bool TryDrop(IPoint point, IEnumerable<Item> items, [MaybeNullWhen(false)] out GroundItem[] groundItems)
    {
        groundItems = items.Select(i => new GroundItem(i, MapInstance, point))
                           .ToArray();

        if (groundItems.Length == 0)
            return false;

        MapInstance.AddEntities(groundItems);

        var reactors = MapInstance.GetDistinctReactorsAtPoint(point)
                                  .ToList();

        foreach (var groundItem in groundItems)
        {
            Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Item, Topics.Actions.Drop)
                  .WithProperty(this)
                  .WithProperty(groundItem)
                  .LogInformation(
                      "{@CreatureType} {@CreatureName} dropped item {@ItemName} at {@Location}",
                      GetType()
                          .Name,
                      Name,
                      groundItem.Name,
                      ILocation.ToString(groundItem));

            groundItem.Item.Script.OnDropped(this, MapInstance);

            foreach (var reactor in reactors)
                reactor.OnItemDroppedOn(this, groundItem);
        }

        return true;
    }

    public virtual bool TryDrop(IPoint point, [MaybeNullWhen(false)] out GroundItem[] groundItems, params IEnumerable<Item> items)
        => TryDrop(point, items, out groundItems);

    public virtual bool TryDropGold(IPoint point, int amount, [MaybeNullWhen(false)] out Money money)
    {
        money = null;

        if ((amount <= 0) || (amount > Gold))
            return false;

        Gold -= amount;

        money = new Money(amount, MapInstance, point);

        MapInstance.AddEntity(money, point);

        Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Gold, Topics.Actions.Drop)
              .WithProperty(this)
              .WithProperty(money)
              .LogInformation(
                  "{@CreatureType} {@CreatureName} dropped {Amount} gold at {@Location}",
                  GetType()
                      .Name,
                  Name,
                  money.Amount,
                  ILocation.ToString(money));

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(point)
                                           .ToList())
            reactor.OnGoldDroppedOn(this, money);

        return true;
    }

    public virtual bool TryUseSkill(Skill skill)
    {
        if (!CanUse(skill, out var context))
            return false;

        skill.Use(context);
        Trackers.LastSkillUse = DateTime.UtcNow;
        Trackers.LastUsedSkill = skill;

        return true;
    }

    public virtual bool TryUseSpell(Spell spell, uint? targetId = null, string? promptResponse = null)
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
                promptResponse,
                out var context))
            return false;

        spell.Use(context);
        Trackers.LastSpellUse = DateTime.UtcNow;
        Trackers.LastUsedSpell = spell;

        return true;
    }

    public virtual void Turn(Direction direction, bool forced = false)
    {
        if (!forced && !Script.CanTurn())
            return;

        Direction = direction;

        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanObserve(this))
            aisling.Client.SendCreatureTurn(Id, direction);

        Trackers.LastTurn = DateTime.UtcNow;
    }

    public virtual void UpdateViewPort(HashSet<VisibleEntity>? partialUpdateEntities = null, bool refresh = false)
    {
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

        foreach (var entity in previouslyObservable)
            if (!currentlyObservable.Contains(entity))
                OnDeparture(entity, refresh);

        foreach (var entity in currentlyObservable)
            if (!previouslyObservable.Contains(entity))
                OnApproached(entity, refresh);

        /*foreach (var entity in previouslyObservable.Except(currentlyObservable))
            OnDeparture(entity, refresh);

        foreach (var entity in currentlyObservable.Except(previouslyObservable))
            OnApproached(entity, refresh);*/
    }

    public virtual void Walk(Direction direction, bool? ignoreBlockingReactors = null, bool? ignoreWalls = null)
    {
        ignoreBlockingReactors ??= Type == CreatureType.Aisling;
        ignoreWalls ??= Type == CreatureType.WalkThrough;
        var metaType = ignoreWalls.Value ? CreatureType.WalkThrough : CreatureType.Normal;

        if (!Script.CanMove())
            return;

        Direction = direction;
        var startPosition = Location.From(this);
        var startPoint = Point.From(this);
        var endPoint = this.DirectionalOffset(direction);

        if (!MapInstance.IsWalkable(endPoint, metaType, ignoreBlockingReactors))
            return;

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

        //non-aislings only cause partial viewport updates because they do not have shared vision requirements (due to lanterns)
        foreach (var creature in creaturesToUpdate)
            creature.UpdateViewPort([this]);

        var aislingsThatWatchedUsWalk = creaturesToUpdate.OfType<Aisling>()
                                                         .ThatCanObserve(this)
                                                         .ThatAreWithinRange(startPoint)
                                                         .ThatAreWithinRange(endPoint);

        foreach (var aisling in aislingsThatWatchedUsWalk)
            aisling.Client.SendCreatureWalk(Id, startPoint, Direction);

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(this)
                                           .ToList())
            reactor.OnWalkedOn(this);
    }

    public virtual void Wander(IPathOptions? pathOptions = null)
    {
        pathOptions ??= PathOptions.Default with
        {
            IgnoreWalls = Type == CreatureType.WalkThrough
        };

        var nearbyDoors = MapInstance.GetEntitiesWithinRange<Door>(this, 1)
                                     .Where(door => door.Closed);

        var nearbyCreatures = MapInstance.GetEntitiesWithinRange<Creature>(this, 1)
                                         .ThatThisCollidesWith(this);

        var nearbyUnwalkablePoints = nearbyDoors.Concat<IPoint>(nearbyCreatures)
                                                .Concat(pathOptions.BlockedPoints)
                                                .ToHashSet();

        pathOptions.BlockedPoints = nearbyUnwalkablePoints;

        var direction = MapInstance.Pathfinder.FindRandomDirection(MapInstance.InstanceId, this, pathOptions);

        if (direction == Direction.Invalid)
            return;

        Walk(direction, pathOptions.IgnoreBlockingReactors, pathOptions.IgnoreWalls);
    }

    /// <inheritdoc />
    public override void WarpTo(IPoint destinationPoint)
    {
        var startPosition = Location.From(this);

        base.WarpTo(destinationPoint);
        Trackers.LastPosition = startPosition;

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(this)
                                           .ToList())
            reactor.OnWalkedOn(this);
    }

    public virtual bool WillCollideWith(Creature other) => Type.WillCollideWith(other);

    public virtual bool WithinLevelRange(Creature other)
        => LevelRangeFormulae.Default.WithinLevelRange(StatSheet.Level, other.StatSheet.Level);
}