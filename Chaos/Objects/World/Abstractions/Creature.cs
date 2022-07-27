using Chaos.Containers;
using Chaos.Data;
using Chaos.Effects.Interfaces;
using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;
using Microsoft.Extensions.Logging;
using PointExtensions = Chaos.Geometry.Extensions.PointExtensions;

namespace Chaos.Objects.World.Abstractions;

public abstract class Creature : NamedEntity, IEffected
{
    public Direction Direction { get; set; }
    public IEffectsBar Effects { get; init; }
    public int GamePoints { get; set; }
    public int Gold { get; set; }
    protected ConcurrentDictionary<uint, DateTime> LastClicked { get; init; }
    public Status Status { get; set; }
    public virtual bool IsAlive => StatSheet.CurrentHp > 0;
    public abstract StatSheet StatSheet { get; }
    public abstract CreatureType Type { get; }
    protected abstract ILogger Logger { get; }
    
    public bool ShouldRegisterClick(uint fromId) =>
        !LastClicked.TryGetValue(fromId, out var lastClick) || (DateTime.UtcNow.Subtract(lastClick).TotalMilliseconds > 500);

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

    public virtual void Update(TimeSpan delta) => Effects.Update(delta);

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

    public void Say(string message) => ShowPublicMessage(PublicMessageType.Normal, message);
    public void Shout(string message) => ShowPublicMessage(PublicMessageType.Shout, message);
    public void Chant(string message) => ShowPublicMessage(PublicMessageType.Chant, message);
    
    public void ShowPublicMessage(PublicMessageType publicMessageType, string message)
    {
        foreach (var aisling in MapInstance.ObjectsThatSee<Aisling>(this))
            aisling.Client.SendPublicMessage(Id, publicMessageType, message);
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

        MapInstance.ActivateReactors(this);
    }

    public abstract void OnGoldDroppedOn(int amount, Aisling source);

    public abstract void OnItemDroppedOn(byte slot, byte count, Aisling source);
}