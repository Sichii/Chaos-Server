using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.CreatureScripts.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.NaturalRegeneration;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities;

namespace Chaos.Models.World.Abstractions;

public abstract class Creature : NamedEntity, IAffected, IScripted<ICreatureScript>
{
    public Direction Direction { get; set; }
    public IEffectsBar Effects { get; protected set; }
    public int GamePoints { get; set; }
    public int Gold { get; set; }
    public virtual bool IsDead { get; set; }
    public Status Status { get; set; }
    public Trackers Trackers { get; set; }
    public Dictionary<uint, DateTime> ApproachTime { get; }
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
    public virtual bool IsBlind => Script.IsBlind();

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
        ApproachTime = new Dictionary<uint, DateTime>();
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

    public virtual bool CanObserve(VisibleEntity entity)
    {
        //can always see yourself
        if (entity.Equals(this))
            return true;

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
        string? prompt,
        [MaybeNullWhen(false)] out SpellContext spellContext)
    {
        spellContext = null;

        if (!Script.CanUseSpell(spell))
            return false;

        if (!spell.CanUse())
            return false;

        spellContext = new SpellContext(this, target, prompt);

        return spell.Script.CanUse(spellContext);
    }

    public virtual void Chant(string message) => ShowPublicMessage(PublicMessageType.Chant, message);

    public Stack<IPoint> FindPath(IPoint target, ICollection<IPoint>? unwalkablePoints = null)
    {
        var nearbyDoors = MapInstance.GetEntitiesWithinRange<Door>(this)
                                     .Where(door => door.Closed);

        var nearbyCreatures = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                         .ThatThisCollidesWith(this);

        var nearbyUnwalkablePoints = nearbyDoors.Concat<IPoint>(nearbyCreatures)
                                                .Concat(unwalkablePoints ?? Array.Empty<IPoint>())
                                                .ToList();

        return MapInstance.Pathfinder.FindPath(
            MapInstance.InstanceId,
            this,
            target,
            Type == CreatureType.WalkThrough,
            nearbyUnwalkablePoints,
            12);
    }

    public virtual bool IsFriendlyTo(Creature other) => Script.IsFriendlyTo(other);

    public virtual bool IsHostileTo(Creature other) => Script.IsHostileTo(other);

    public virtual void OnApproached(Creature creature)
    {
        ApproachTime.TryAdd(creature.Id, DateTime.UtcNow);

        Script.OnApproached(creature);
    }

    public override void OnClicked(Aisling source)
    {
        if (!ShouldRegisterClick(source.Id))
            return;

        LastClicked[source.Id] = DateTime.UtcNow;
        Script.OnClicked(source);
    }

    public virtual void OnDeparture(Creature creature)
    {
        ApproachTime.Remove(creature.Id);

        Script.OnDeparture(creature);
    }

    public virtual void OnGoldDroppedOn(Aisling source, int amount)
    {
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

    public void Pathfind(IPoint target, ICollection<IPoint>? unwalkablePoints = null)
    {
        //if we're right next to the point, dont bother pathfinding
        if (this.DistanceFrom(target) == 1)
            return;

        var path = FindPath(target, unwalkablePoints);

        //if there is no path, return
        if (path.Count == 0)
            return;

        var nextPoint = path.Pop();
        var direction = nextPoint.DirectionalRelationTo(this);

        Walk(direction);
    }

    public virtual void Say(string message) => ShowPublicMessage(PublicMessageType.Normal, message);

    /// <inheritdoc />
    public override void SetVisibility(VisibilityType newVisibilityType)
    {
        if (Visibility == newVisibilityType)
            return;

        var creaturesInRange = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                          .ToList();

        var canObserveBefore = new List<Creature>();

        foreach (var creature in creaturesInRange)
            if (creature.CanObserve(this))
                canObserveBefore.Add(creature);

        Hide();

        Visibility = newVisibilityType;

        Display();

        var canObserveAfter = new List<Creature>();

        foreach (var creature in creaturesInRange)
            if (creature.CanObserve(this))
                canObserveAfter.Add(creature);

        foreach (var creature in canObserveBefore.Except(canObserveAfter))
            creature.OnDeparture(this);

        foreach (var creature in canObserveAfter.Except(canObserveBefore))
            creature.OnApproached(this);
    }

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
                creaturesWithinRange = MapInstance.GetEntitiesWithinRange<Creature>(this);
                sendMessage = $"{Name}: {message}";

                break;
            case PublicMessageType.Shout:
                creaturesWithinRange = MapInstance.GetEntities<Creature>();
                sendMessage = $"{Name}! {message}";

                break;
            case PublicMessageType.Chant:
                creaturesWithinRange = MapInstance.GetEntities<Creature>();

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(publicMessageType), publicMessageType, null);
        }

        if (this is Aisling && (sendMessage.Length > CONSTANTS.MAX_PUBLIC_MESSAGE_LENGTH))
            sendMessage = sendMessage[..CONSTANTS.MAX_PUBLIC_MESSAGE_LENGTH];

        creaturesWithinRange = creaturesWithinRange.ToList();

        //separated to merchant replies show up below the text theyre responding to
        foreach (var aisling in creaturesWithinRange.OfType<Aisling>())
            if (!aisling.IgnoreList.Contains(Name))
                aisling.Client.SendDisplayPublicMessage(Id, publicMessageType, sendMessage);

        Trackers.LastTalk = DateTime.UtcNow;

        foreach (var creature in creaturesWithinRange)
            creature.Script.OnPublicMessage(this, message);
    }

    public void TraverseMap(
        MapInstance destinationMap,
        IPoint destinationPoint,
        bool ignoreSharding = false,
        bool fromWolrdMap = false,
        Func<Task>? onTraverse = null)
        =>

            //run a task that will await entrancy into the destination map
            //once synchronized, the creature will be added to the map
            Task.Run(
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

    public virtual bool TryDrop(IPoint point, IEnumerable<Item> items, [MaybeNullWhen(false)] out GroundItem[] groundItems)
    {
        groundItems = items.Select(i => new GroundItem(i, MapInstance, point))
                           .ToArray();

        if (!groundItems.Any())
            return false;

        MapInstance.AddObjects(groundItems);

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

    public virtual bool TryDrop(IPoint point, [MaybeNullWhen(false)] out GroundItem[] groundItems, params Item[] items)
        => TryDrop(point, items.AsEnumerable(), out groundItems);

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

    public virtual bool TryUseSpell(Spell spell, uint? targetId = null, string? prompt = null)
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

        spell.Use(context);
        Trackers.LastSpellUse = DateTime.UtcNow;
        Trackers.LastUsedSpell = spell;

        return true;
    }

    public virtual void Turn(Direction direction)
    {
        if (!Script.CanTurn())
            return;

        Direction = direction;

        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanObserve(this))
            aisling.Client.SendCreatureTurn(Id, direction);

        Trackers.LastTurn = DateTime.UtcNow;
    }

    public virtual void Walk(Direction direction)
    {
        if (!Script.CanMove())
            return;

        Direction = direction;
        var startPosition = Location.From(this);
        var startPoint = Point.From(this);
        var endPoint = ((IPoint)this).DirectionalOffset(direction);

        if (!MapInstance.IsWalkable(endPoint, Type))
            return;

        var visibleBefore = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                       .ToList();

        SetLocation(endPoint);
        Trackers.LastWalk = DateTime.UtcNow;
        Trackers.LastPosition = startPosition;

        var visibleAfter = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                      .ToList();

        foreach (var creature in visibleBefore.Except(visibleAfter))
        {
            if (creature is Aisling aisling)
                HideFrom(aisling);

            Helpers.HandleDeparture(creature, this);
        }

        foreach (var creature in visibleAfter.Except(visibleBefore))
        {
            if (creature is Aisling aisling)
                ShowTo(aisling);

            Helpers.HandleApproach(creature, this);
        }

        foreach (var aisling in visibleAfter.Intersect(visibleBefore)
                                            .OfType<Aisling>())
            aisling.Client.SendCreatureWalk(Id, startPoint, Direction);

        foreach (var reactor in MapInstance.GetDistinctReactorsAtPoint(this)
                                           .ToList())
            reactor.OnWalkedOn(this);
    }

    public virtual void Wander(ICollection<IPoint>? unwalkablePoints = null)
    {
        var nearbyDoors = MapInstance.GetEntitiesWithinRange<Door>(this, 1)
                                     .Where(door => door.Closed);

        var nearbyCreatures = MapInstance.GetEntitiesWithinRange<Creature>(this, 1)
                                         .ThatThisCollidesWith(this);

        var nearbyUnwalkablePoints = nearbyDoors.Concat<IPoint>(nearbyCreatures)
                                                .Concat(unwalkablePoints ?? Array.Empty<IPoint>())
                                                .ToList();

        var direction = MapInstance.Pathfinder.FindRandomDirection(
            MapInstance.InstanceId,
            this,
            Type == CreatureType.WalkThrough,
            nearbyUnwalkablePoints);

        if (direction == Direction.Invalid)
            return;

        Walk(direction);
    }

    /// <inheritdoc />
    public override void WarpTo(IPoint destinationPoint)
    {
        Hide();

        var creaturesBefore = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                         .ToList();

        var startPosition = Location.From(this);
        SetLocation(destinationPoint);
        Trackers.LastPosition = startPosition;

        var creaturesAfter = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                        .ToList();

        foreach (var creature in creaturesBefore.Except(creaturesAfter))
            Helpers.HandleDeparture(creature, this);

        foreach (var creature in creaturesAfter.Except(creaturesBefore))
            Helpers.HandleApproach(creature, this);

        Display();
    }

    public virtual bool WillCollideWith(Creature other) => Type.WillCollideWith(other);

    public virtual bool WithinLevelRange(Creature other)
        => LevelRangeFormulae.Default.WithinLevelRange(StatSheet.Level, other.StatSheet.Level);
}