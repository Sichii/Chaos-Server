using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Effects.Abstractions;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;
using Chaos.Objects.Panel;
using Chaos.Services.Hosted.Options;
using Chaos.Utilities;
using Microsoft.Extensions.Logging;

namespace Chaos.Objects.World.Abstractions;

public abstract class Creature : NamedEntity, IEffected
{
    public Direction Direction { get; set; }
    public IEffectsBar Effects { get; init; }
    public int GamePoints { get; set; }
    public int Gold { get; set; }
    public Status Status { get; set; }
    protected ConcurrentDictionary<uint, DateTime> LastClicked { get; init; }
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
            sprite,
            mapInstance,
            point)
    {
        Direction = Direction.Down;
        Effects = new EffectsBar(this);
        LastClicked = new ConcurrentDictionary<uint, DateTime>();
    }

    protected virtual void ApplyAcModifier(ref float damage)
    {
        if (StatSheet.Ac == 0)
            return;

        var ac = Math.Clamp(StatSheet.Ac, WorldOptions.Instance.MinimumMonsterAc, WorldOptions.Instance.MaximumMonsterAc);
        var mod = 1 + ac / 100.0f;
        damage *= mod;
    }

    public virtual void ApplyDamage(
        Creature source,
        int amount,
        byte hitSound = 1,
        bool ignoreAc = false
    )
    {
        var damage = (float)amount;

        if (!ignoreAc)
            ApplyAcModifier(ref damage);
    }

    public void Chant(string message) => ShowPublicMessage(PublicMessageType.Chant, message);

    public abstract void OnGoldDroppedOn(Aisling source, int amount);

    public abstract void OnItemDroppedOn(Aisling source, byte slot, byte count);

    public void Say(string message) => ShowPublicMessage(PublicMessageType.Normal, message);

    public bool ShouldRegisterClick(uint fromId) =>
        !LastClicked.TryGetValue(fromId, out var lastClick) || (DateTime.UtcNow.Subtract(lastClick).TotalMilliseconds > 500);

    public void Shout(string message) => ShowPublicMessage(PublicMessageType.Shout, message);

    public void ShowPublicMessage(PublicMessageType publicMessageType, string message)
    {
        foreach (var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            aisling.Client.SendPublicMessage(Id, publicMessageType, message);
    }

    public void TraverseMap(MapInstance destinationMap, IPoint destinationPoint)
    {
        var currentMap = MapInstance;
        currentMap.RemoveObject(this);

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

    public virtual void Turn(Direction direction)
    {
        Direction = direction;

        foreach (var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            aisling.Client.SendCreatureTurn(Id, direction);
    }

    public virtual void Update(TimeSpan delta) => Effects.Update(delta);

    public virtual bool WillCollideWith(Creature creature)
    {
        //merchanges will collide with everything
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
    
    public virtual void Walk(Direction direction)
    {
        Direction = direction;
        var startPoint = Point.From(this);
        var endPoint = PointExtensions.DirectionalOffset(this, direction);

        if (!MapInstance.IsWalkable(endPoint, Type == CreatureType.WalkThrough))
            return;

        var visibleBefore = MapInstance.ObjectsThatSee<Aisling>(this)
                                       .ToList();

        SetLocation(endPoint);

        var visibleAfter = MapInstance.ObjectsThatSee<Aisling>(this)
                                      .ToList();

        foreach (var aisling in visibleBefore.Except(visibleAfter))
            HideFrom(aisling);

        foreach (var aisling in visibleAfter.Except(visibleBefore))
            ShowTo(aisling);

        foreach (var aisling in visibleAfter.Intersect(visibleBefore))
            aisling.Client.SendCreatureWalk(Id, startPoint, Direction);

        MapInstance.ActivateReactors(this, ReactorTileType.Walk);
    }
    
    public void Pathfind(IPoint target)
    {
        var nearbyCreatures = MapInstance.ObjectsWithinRange<Creature>(this)
                                         .Where(WillCollideWith)
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

    public void Wander()
    {
        var nearbyCreatures = MapInstance.ObjectsWithinRange<Creature>(this)
                                         .Where(WillCollideWith)
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

    public virtual void Drop(IPoint point, IEnumerable<Item> items)
    {
        var groundItems = items.Select(i => new GroundItem(i, MapInstance, point))
                               .ToList();

        if (!groundItems.Any())
            return;
        
        MapInstance.AddObjects(groundItems);
        
        foreach(var groundItem in groundItems)
            groundItem.Item.Script.OnDropped(this, MapInstance);
    }

    public virtual void DropGold(IPoint point, int amount)
    {
        if (amount <= 0)
            return;
        
        var money = new Money(amount, MapInstance, point);
        
        MapInstance.AddObject(money, point);
    }

    public virtual bool WithinLevelRange(Creature other)
    {
        //these 2 statements are graphed as opposing intersecting lines
        //conal-positive (level range grows as level goes up)
        //the idea here is to be able to calculate level ranges for 2 people
        //on the edge of eachother's level range, but theyre both still
        //in eachother's range
        //if a flat range value was calculated based on level
        //a low level person would be in a high level person's larger range,
        //but the high level person would not be in the lower level person's range
        var upperBound = LevelRange.GetUpperBound(StatSheet.Level);
        var lowerBound = LevelRange.GetLowerBound(StatSheet.Level);

        if((other.StatSheet.Level <= upperBound) && (other.StatSheet.Level >= lowerBound))
            return true;

        return false;
    }
    
    public void Drop(IPoint point, params Item[] items) => Drop(point, items.AsEnumerable());
}