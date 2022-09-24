using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Effects.Abstractions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Networking.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.Panel.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World.Abstractions;

public abstract class Creature : NamedEntity, IEffected
{
    public Direction Direction { get; set; }
    public IEffectsBar Effects { get; init; }
    public int GamePoints { get; set; }
    public int Gold { get; set; }
    public virtual bool IsDead { get; set; }
    public Status Status { get; set; }
    protected ConcurrentDictionary<uint, DateTime> LastClicked { get; init; }
    public abstract int AssailIntervalMs { get; }
    public virtual bool IsAlive => StatSheet.CurrentHp > 0;
    public abstract StatSheet StatSheet { get; }
    public abstract CreatureType Type { get; }
    protected abstract ILogger Logger { get; }

    protected Creature(
        string name,
        ushort sprite,
        MapInstance mapInstance,
        IPoint point
    )
        : base(
            name,
            (ushort)(sprite == 0 ? 0 : sprite + NETWORKING_CONSTANTS.CREATURE_SPRITE_OFFSET),
            mapInstance,
            point)
    {
        Direction = Direction.Down;
        Effects = new EffectsBar(this);
        LastClicked = new ConcurrentDictionary<uint, DateTime>();
    }

    /// <inheritdoc />
    public override void Animate(Animation animation, uint? sourceId = null)
    {
        var targetedAnimation = animation.GetTargetedAnimation(Id, sourceId);

        foreach (var obj in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                       .ThatCanSee(this))
            obj.Client.SendAnimation(targetedAnimation);
    }

    public virtual void AnimateBody(BodyAnimation bodyAnimation, ushort speed = 25, byte? sound = null)
    {
        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanSee(this))
            aisling.Client.SendBodyAnimation(
                Id,
                bodyAnimation,
                speed,
                sound);
    }

    public abstract void ApplyDamage(
        Creature source,
        int amount,
        byte? hitSound = 1
    );

    public virtual bool CanUse(PanelObjectBase panelObject) => panelObject.CanUse();

    public void Chant(string message) => ShowPublicMessage(PublicMessageType.Chant, message);

    public virtual void Drop(IPoint point, IEnumerable<Item> items)
    {
        var groundItems = items.Select(i => new GroundItem(i, MapInstance, point))
                               .ToList();

        if (!groundItems.Any())
            return;

        MapInstance.AddObjects(groundItems);

        foreach (var groundItem in groundItems)
            groundItem.Item.Script.OnDropped(this, MapInstance);
    }

    public void Drop(IPoint point, params Item[] items) => Drop(point, items.AsEnumerable());

    public virtual void DropGold(IPoint point, int amount)
    {
        if (amount <= 0)
            return;

        var money = new Money(amount, MapInstance, point);

        MapInstance.AddObject(money, point);
    }

    public abstract void OnGoldDroppedOn(Aisling source, int amount);

    public abstract void OnItemDroppedOn(Aisling source, byte slot, byte count);

    public void Pathfind(IPoint target)
    {
        var nearbyCreatures = MapInstance.GetEntitiesWithinRange<Creature>(this)
                                         .ThatCollideWith(this)
                                         .ToList<IPoint>();

        var direction = MapInstance.Pathfinder.Pathfind(
            MapInstance.InstanceId,
            this,
            target,
            Type == CreatureType.WalkThrough,
            nearbyCreatures);

        if (direction == Direction.Invalid)
            return;

        Walk(direction);
    }

    public void Say(string message) => ShowPublicMessage(PublicMessageType.Normal, message);

    public bool ShouldRegisterClick(uint fromId) =>
        !LastClicked.TryGetValue(fromId, out var lastClick) || (DateTime.UtcNow.Subtract(lastClick).TotalMilliseconds > 500);

    public void Shout(string message) => ShowPublicMessage(PublicMessageType.Shout, message);

    public void ShowPublicMessage(PublicMessageType publicMessageType, string message)
    {
        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanSee(this))
            aisling.Client.SendPublicMessage(Id, publicMessageType, message);
    }

    public void TraverseMap(MapInstance destinationMap, IPoint destinationPoint)
    {
        var currentMap = MapInstance;

        if (!currentMap.RemoveObject(this))
            return;

        //if this thread has a lock taken out on the current map
        //we have to release it, because adding to another map takes out that map's lock
        //deadlocks can occur if there is contention on both locks simultaneously
        var wasEntered = false;

        if (currentMap.Sync.IsEntered)
        {
            wasEntered = true;
            currentMap.Sync.Exit();
        }

        destinationMap.AddObject(this, destinationPoint);

        //there is no guarantee we can get the lock back
        if (wasEntered)
            currentMap.Sync.TryEnter(5);
    }

    public virtual bool TryUseSkill(Skill skill)
    {
        if (!CanUse(skill))
            return false;

        skill.Use(this);

        return true;
    }

    public virtual bool TryUseSpell(Spell spell, uint? targetId = null, string? prompt = null)
    {
        if (!CanUse(spell))
            return false;

        Creature target;

        if (!targetId.HasValue)
            target = this;
        else if (!MapInstance.TryGetObject(targetId.Value, out target!))
            return false;

        var context = new ActivationContext(target, this, prompt);

        spell.Use(context);

        return true;
    }

    public virtual void Turn(Direction direction)
    {
        Direction = direction;

        foreach (var aisling in MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                           .ThatCanSee(this))
            aisling.Client.SendCreatureTurn(Id, direction);
    }

    public override void Update(TimeSpan delta) => Effects.Update(delta);

    public virtual void Walk(Direction direction)
    {
        Direction = direction;
        var startPoint = Point.From(this);
        var endPoint = ((IPoint)this).DirectionalOffset(direction);

        if (!MapInstance.IsWalkable(endPoint, Type == CreatureType.WalkThrough))
            return;

        var visibleBefore = MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                       .ThatCanSee(this)
                                       .ToList();

        SetLocation(endPoint);

        var visibleAfter = MapInstance.GetEntitiesWithinRange<Aisling>(this)
                                      .ThatCanSee(this)
                                      .ToList();

        foreach (var aisling in visibleBefore.Except(visibleAfter))
            HideFrom(aisling);

        foreach (var aisling in visibleAfter.Except(visibleBefore))
            ShowTo(aisling);

        foreach (var aisling in visibleAfter.Intersect(visibleBefore))
            aisling.Client.SendCreatureWalk(Id, startPoint, Direction);

        MapInstance.ActivateReactors(this, ReactorTileType.Walk);
    }

    public void Wander()
    {
        var nearbyCreatures = MapInstance.GetEntitiesWithinRange<Creature>(this, 1)
                                         .ThatCollideWith(this)
                                         .ToList<IPoint>();

        var direction = MapInstance.Pathfinder.Wander(
            MapInstance.InstanceId,
            this,
            Type == CreatureType.WalkThrough,
            nearbyCreatures);

        if (direction == Direction.Invalid)
            return;

        Walk(direction);
    }

    public virtual bool WillCollideWith(Creature creature)
    {
        //merchants will collide with everything
        if ((creature.Type == CreatureType.Merchant) || (Type == CreatureType.Merchant))
            return true;

        //walkthrough creatures only collide with eachother (and merchants)
        if ((Type == CreatureType.WalkThrough) && (creature.Type == CreatureType.WalkThrough))
            return true;

        //normal monsters don't collide with walkthrough creatures
        if ((Type == CreatureType.Normal) && (creature.Type != CreatureType.WalkThrough))
            return true;

        return false;
    }

    public virtual bool WithinLevelRange(Creature other) =>
        LevelRangeFormulae.Default.WithinLevelRange(StatSheet.Level, other.StatSheet.Level);
}