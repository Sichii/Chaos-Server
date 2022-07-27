using System.Text.Json.Serialization;
using Chaos.Core.Collections;
using Chaos.Core.Synchronization;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Geometry.Definitions;
using Chaos.Geometry.Interfaces;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Services.Hosted.Options;
using Chaos.Templates;
using Chaos.Time.Interfaces;

namespace Chaos.Containers;

public class MapInstance : IDeltaUpdatable
{
    private readonly TypeCheckingDictionary<uint, MapEntity> Objects;
    //private readonly ConcurrentDictionary<Exchange>
    public AutoReleasingMonitor Sync { get; }
    public MapFlags Flags { get; set; }
    public string InstanceId { get; init; }
    public short MapId { get; set; }
    public sbyte Music { get; set; }
    public string Name { get; set; }
    public MapTemplate Template { get; set; } = null!;
    public Warp[][] WarpGroups { get; set; } = Array.Empty<Warp[]>();

    [JsonConstructor]
    public MapInstance(string name, string instanceId)
    {
        Name = name;
        InstanceId = instanceId;
        Objects = new TypeCheckingDictionary<uint, MapEntity>();
        Sync = new AutoReleasingMonitor();
    }

    public void ActivateReactors(Creature creature)
    {
        using var @lock = Sync.Enter();

        var reactors = ObjectsAtPoint<ReactorTile>(creature);

        foreach (var reactor in reactors)
            reactor.Activate(creature);
    }

    public void AddObject(VisibleEntity visibleEntity, IPoint point)
    {
        using var @lock = Sync.Enter();

        visibleEntity.X = point.X;
        visibleEntity.Y = point.Y;
        visibleEntity.MapInstance = this;
        Objects.Add(visibleEntity.Id, visibleEntity);

        if (visibleEntity is Aisling aisling)
        {
            (var aislings, var doors, var otherVisibles) = ObjectsWithinRange<VisibleEntity>(point).SortBySendType();

            aisling.Client.SendMapChangePending();
            aisling.Client.SendMapInfo();
            aisling.Client.SendLocation();

            foreach (var nearbyUser in aislings)
            {
                if (nearbyUser.Equals(aisling))
                    continue;

                if (aisling.IsVisibleTo(nearbyUser))
                    nearbyUser.Client.SendDisplayAisling(aisling);

                if (nearbyUser.IsVisibleTo(aisling))
                    aisling.Client.SendDisplayAisling(nearbyUser);
            }

            aisling.Client.SendVisibleObjects(otherVisibles);
            aisling.Client.SendDoors(doors);
            aisling.Client.SendMapChangeComplete();
            aisling.Client.SendMapLoadComplete();
            aisling.Client.SendDisplayAisling(aisling);
        } else
            foreach (var nearbyUser in ObjectsThatSee<Aisling>(visibleEntity))
                nearbyUser.Client.SendVisibleObjects(visibleEntity);
    }

    public void AddObjects<T>(ICollection<T> visibleObjects) where T: VisibleEntity
    {
        using var @lock = Sync.Enter();

        foreach (var visibleObj in visibleObjects)
        {
            visibleObj.MapInstance = this;
            Objects.Add(visibleObj.Id, visibleObj);
        }

        foreach (var aisling in Objects.Values<Aisling>())
        {
            var objectsInRange = visibleObjects
                                 .Where(obj => obj.WithinRange(aisling) && obj.IsVisibleTo(aisling))
                                 .ToArray<VisibleEntity>();

            if (objectsInRange.Any())
                aisling.Client.SendVisibleObjects(objectsInRange);
        }
    }
    
    public void Click(uint id, Aisling source)
    {
        using var @lock = Sync.Enter();

        if (TryGetObject<VisibleEntity>(id, out var obj))
            if (obj.WithinRange(source) && obj.IsVisibleTo(source))
                obj.OnClicked(source);
    }

    public void Click(IPoint point, Aisling source)
    {
        using var @lock = Sync.Enter();

        var obj = ObjectsAtPoint<VisibleEntity>(point)
                  .ThatAreVisibleTo(source)
                  .FirstOrDefault();

        obj?.OnClicked(source);
    }
    
    public bool IsWalkable(IPoint point, bool toWalkthroughCreature = false)
    {
        if (toWalkthroughCreature ? !IsWithinMap(point) : IsWall(point))
            return false;

        using var @lock = Sync.Enter();

        return Objects
               .Values<Creature>()
               .All(
                   creature =>
                   {
                       if (toWalkthroughCreature)
                           return (creature.Type != CreatureType.WalkThrough) || (Point.From(creature) != point);

                       return (creature.Type == CreatureType.WalkThrough) || (Point.From(creature) != point);
                   });
    }

    public bool IsWall(IPoint point) => Template.IsWall(point);

    public bool IsWithinMap(IPoint point) => Template.IsWithinMap(point);

    public IEnumerable<T> ObjectsAtPoint<T>(IPoint point) where T: MapEntity
    {
        var objects = Objects.Values<T>()
                             .Where(obj => Point.From(obj) == point)
                             .OrderByDescending(obj => obj.Creation);

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }

    public IEnumerable<T> ObjectsThatSee<T>(VisibleEntity visibleEntity, int distance = 13) where T: MapEntity
    {
        var objects = Objects.Values<T>()
                             .Where(obj => visibleEntity.WithinRange(obj, distance));

        if (objects is IEnumerable<Creature> creatures)
            objects = creatures.ThatCanSee(visibleEntity).Cast<T>();

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }

    public IEnumerable<T> ObjectsVisibleTo<T>(MapEntity mapEntity, int distance = 13) where T: VisibleEntity
    {
        var objects = Objects.Values<T>()
                             .Where(obj => mapEntity.WithinRange(obj, distance));

        if (mapEntity is Aisling aisling)
            objects = objects.ThatAreVisibleTo(aisling);

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }
    
    public IEnumerable<T> ObjectsWithinRange<T>(IPoint point, int distance = 13) where T: MapEntity
    {
        var objects = Objects.Values<T>()
                             .Where(obj => obj.WithinRange(point, distance));

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }
    

    
    public void RemoveObject(MapEntity mapEntity)
    {
        using var @lock = Sync.Enter();

        Objects.Remove(mapEntity.Id);

        if (mapEntity is VisibleEntity visibleObject)
            foreach (var aisling in ObjectsThatSee<Aisling>(visibleObject))
                aisling.Client.SendRemoveObject(visibleObject.Id);
    }

    public void ShowAnimation(Animation animation)
    {
        using var @lock = Sync.Enter();

        if (animation.TargetId.HasValue)
        {
            if (TryGetObject<Creature>(animation.TargetId.Value, out var target))
                foreach (var aisling in ObjectsThatSee<Aisling>(target))
                    aisling.Client.SendAnimation(animation);
        } else if (animation.TargetPoint != default)
            foreach (var aisling in ObjectsWithinRange<Aisling>(animation.TargetPoint))
                aisling.Client.SendAnimation(animation);
    }

    public void ShowBodyAnimation(BodyAnimation bodyAnimation, Creature source, byte? sound = null)
    {
        using var @lock = Sync.Enter();

        foreach (var aisling in ObjectsThatSee<Aisling>(source))
            aisling.Client.SendBodyAnimation(
                source.Id,
                bodyAnimation,
                100,
                sound);
    }
    
    public void SimpleAdd(MapEntity mapEntity)
    {
        using var @lock = Sync.Enter();
        Objects.Add(mapEntity.Id, mapEntity);
    }
    
    public bool TryGetObject<T>(uint id, [MaybeNullWhen(false)] out T obj)
    {
        using var @lock = Sync.Enter();

        return Objects.TryGetValue(id, out obj);
    }

    public void Update(TimeSpan delta)
    {
        using var @lock = Sync.Enter();

        foreach (var updateable in Objects.Values<IDeltaUpdatable>())
            updateable.Update(delta);
    }
}