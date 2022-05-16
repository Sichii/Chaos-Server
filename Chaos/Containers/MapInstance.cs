using System.Text.Json.Serialization;
using Chaos.Clients.Interfaces;
using Chaos.Core.Collections;
using Chaos.Core.Interfaces;
using Chaos.Core.Synchronization;
using Chaos.Core.Utilities;
using Chaos.Extensions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Options;
using Chaos.Templates;

namespace Chaos.Containers;

public class MapInstance : IDeltaUpdatable
{
    private readonly TypeCheckingDictionary<uint, MapObject> Objects;
    //private readonly ConcurrentDictionary<Exchange>
    public AutoReleasingMonitor Sync { get; }
    public MapFlags Flags { get; set; }
    public string InstanceId { get; init; }
    public short MapId { get; set; }
    public sbyte Music { get; set; }
    public string Name { get; set; }
    public MapTemplate Template { get; set; } = null!;
    public Warp[][] WarpGroups { get; set; } = Array.Empty<Warp[]>();
    public WorldOptions WorldOptions { get; set; } = null!;

    [JsonConstructor]
    public MapInstance(string name, string instanceId)
    {
        Name = name;
        InstanceId = instanceId;
        Objects = new TypeCheckingDictionary<uint, MapObject>();
        Sync = new AutoReleasingMonitor();
    }

    public void ActivateReactors(Creature creature)
    {
        using var @lock = Sync.Enter();

        var reactors = ObjectsAtPoint<ReactorTile>(creature.Point);

        foreach (var reactor in reactors)
            reactor.Activate(creature);
    }

    public void AddObject(VisibleObject visibleObject, Point point)
    {
        var temp = Sync.TryEnter(5);
        
        //TODO: was considering reentrancy a potential problem (YOU ARE HERE)
        //a user failed to enter because we're currently inside it's map instance lock and trying to enter a different map
        //this means we're entering 2 locks at once, which could cause a deadlock if the reverse order happens for another user
        //the solution here is to exit the old map's lock and try to reacquire the current map's lock
        if ((temp == null) && visibleObject is User && visibleObject.MapInstance.Sync.IsEntered)
        {
            visibleObject.MapInstance.Sync.Exit();
            temp = Sync.TryEnter(10);
        }

        temp ??= AutoReleasingMonitor.NoOpDisposable;

        using var @lock = temp;

        visibleObject.Point = point;
        visibleObject.MapInstance = this;
        Objects.Add(visibleObject.Id, visibleObject);

        if (visibleObject is User user)
        {
            var nearbyObjects = ObjectsWithinRange<VisibleObject>(point);

            var nearbyUsers = new List<User>();
            var nearbyOthers = new List<VisibleObject>();
            var nearbyDoors = new List<Door>();

            foreach (var obj in nearbyObjects)
                switch (obj)
                {
                    case User userObj:
                        nearbyUsers.Add(userObj);

                        break;
                    case Creature creatureObj:
                        nearbyOthers.Add(creatureObj);

                        break;
                    case GroundItem groundItemObj:
                        nearbyOthers.Add(groundItemObj);

                        break;
                    case Door doorObj:
                        nearbyDoors.Add(doorObj);

                        break;
                }

            user.Client.SendMapChangePending();
            user.Client.SendMapInfo();
            user.Client.SendLocation();

            foreach (var nearbyUser in nearbyUsers)
            {
                if (nearbyUser.Equals(user))
                    continue;

                if (user.IsVisibleTo(nearbyUser))
                    nearbyUser.Client.SendDisplayUser(user);

                if (nearbyUser.IsVisibleTo(user))
                    user.Client.SendDisplayUser(nearbyUser);
            }

            user.Client.SendVisibleObjects(nearbyOthers.ToArray());
            user.Client.SendDoors(nearbyDoors.ToArray());
            user.Client.SendMapChangeComplete();
            user.Client.SendMapLoadComplete();
            user.Client.SendDisplayUser(user);
        } else
            foreach (var nearbyUser in ObjectsThatSee<User>(visibleObject))
                nearbyUser.Client.SendVisibleObjects(visibleObject);
    }

    public void AddObjects<T>(ICollection<T> visibleObjects) where T: VisibleObject
    {
        using var @lock = Sync.Enter();

        foreach (var visibleObj in visibleObjects)
        {
            visibleObj.MapInstance = this;
            Objects.Add(visibleObj.Id, visibleObj);
        }

        foreach (var user in Objects.Values<User>())
        {
            var objectsInRange = visibleObjects
                                 .Where(obj => obj.WithinRange(user) && obj.IsVisibleTo(user))
                                 .ToArray<VisibleObject>();

            if (objectsInRange.Any())
                user.Client.SendVisibleObjects(objectsInRange);
        }
    }

    public void Click(uint id, User source)
    {
        using var @lock = Sync.Enter();

        if (TryGetObject<VisibleObject>(id, out var obj))
            if (obj.WithinRange(source) && obj.IsVisibleTo(source))
                obj.OnClicked(source);
    }

    public void Click(Point point, User source)
    {
        using var @lock = Sync.Enter();

        var obj = ObjectsAtPoint<VisibleObject>(point)
                  .ThatAreVisibleTo(source)
                  .FirstOrDefault();

        obj?.OnClicked(source);
    }

    public void ClientWalk(User user, Direction direction)
    {
        using var @lock = Sync.Enter();

        user.Direction = direction;

        var startPoint = user.Point;
        var newPoint = startPoint.Offset(direction);

        if (!user.IsAdmin && !IsWalkable(newPoint))
        {
            Refresh(user, true);

            return;
        }

        var visibleBefore = ObjectsWithinRange<VisibleObject>(user).ToList();
        user.Point = newPoint;
        var visibleAfter = ObjectsWithinRange<VisibleObject>(user).ToList();
        var otherObjectsToSend = new List<VisibleObject>();
        var doorsToSend = new List<Door>();

        user.Client.SendConfirmClientWalk(startPoint, direction);

        //for all objects we can no longer see
        //remove them from our screen, remove use from their if theyre a user
        foreach (var obj in visibleBefore.Except(visibleAfter))
            if (obj is GroundItem or Creature)
            {
                user.Client.SendRemoveObject(obj.Id);

                if (obj is User otherUser)
                    otherUser.Client.SendRemoveObject(user.Id);
            }

        //for all objects we couldnt see before
        //display them to us, display us to them if theyre a user
        foreach (var obj in visibleAfter.Except(visibleBefore))
            if (obj is User otherUser)
            {
                if (user.IsVisibleTo(otherUser))
                    otherUser.Client.SendDisplayUser(user);

                if (otherUser.IsVisibleTo(user))
                    user.Client.SendDisplayUser(otherUser);
            } else if (obj is GroundItem or Creature)
            {
                if (obj.IsVisibleTo(user))
                    otherObjectsToSend.Add(obj);
            } else if (obj is Door door)
                doorsToSend.Add(door);

        //send all non-users at the same time
        if (otherObjectsToSend.Any())
            user.Client.SendVisibleObjects(otherObjectsToSend.ToArray());

        if (doorsToSend.Any())
            user.Client.SendDoors(doorsToSend.ToArray());

        foreach (var otherUser in visibleBefore.Intersect(visibleAfter).OfType<User>().ThatCanSee(user))
            if (!otherUser.Equals(user))
                otherUser.Client.SendCreatureWalk(user.Id, startPoint, direction);

        ActivateReactors(user);
    }

    public void CreatureWalk(Creature creature, Direction direction)
    {
        using var @lock = Sync.Enter();

        creature.Direction = direction;
        var startPoint = creature.Point;
        var newPoint = startPoint.Offset(direction);

        if (!IsWalkable(newPoint, creature.Type == CreatureType.WalkThrough))
            return;

        var visibleBefore = ObjectsThatSee<User>(creature).ToList();
        creature.Point = newPoint;
        var visibleAfter = ObjectsThatSee<User>(creature).ToList();

        foreach (var user in visibleBefore.Except(visibleAfter))
            user.Client.SendRemoveObject(creature.Id);

        foreach (var user in visibleAfter.Except(visibleBefore))
            user.Client.SendVisibleObjects(creature);

        foreach (var user in visibleBefore.Intersect(visibleAfter))
            user.Client.SendCreatureWalk(creature.Id, startPoint, direction);
    }

    public bool IsWalkable(Point point, bool toWalkthroughCreature = false)
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
                           return (creature.Type != CreatureType.WalkThrough) || (creature.Point != point);

                       return (creature.Type == CreatureType.WalkThrough) || (creature.Point != point);
                   });
    }

    public bool IsWall(Point point) => Template.IsWall(point);

    public bool IsWithinMap(Point point) => Template.IsWithinMap(point);

    public IEnumerable<T> ObjectsAtPoint<T>(Point point) where T: MapObject
    {
        var objects = Objects.Values<T>()
                             .Where(obj => obj.Point == point)
                             .OrderByDescending(obj => obj.Creation);

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }

    public IEnumerable<T> ObjectsThatSee<T>(VisibleObject visibleObject, int distance = 13) where T: MapObject
    {
        var objects = Objects.Values<T>()
                             .Where(obj => visibleObject.WithinRange(obj, distance));

        if (objects is IEnumerable<Creature> creatures)
            objects = creatures.ThatCanSee(visibleObject).Cast<T>();

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }

    public IEnumerable<T> ObjectsVisibleTo<T>(MapObject mapObject, int distance = 13) where T: VisibleObject
    {
        var objects = Objects.Values<T>()
                             .Where(obj => mapObject.WithinRange(obj, distance));

        if (mapObject is User user)
            objects = objects.ThatAreVisibleTo(user);

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }

    public IEnumerable<T> ObjectsWithinRange<T>(MapObject mapObject, int distance = 13) where T: MapObject
    {
        var objects = Objects.Values<T>()
                             .Where(obj => mapObject.WithinRange(obj, distance));

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }

    public IEnumerable<T> ObjectsWithinRange<T>(Point point, int distance = 13) where T: MapObject
    {
        var objects = Objects.Values<T>()
                             .Where(obj => obj.WithinRange(point, distance));

        if (Sync.IsEntered)
            return objects;

        using var @lock = Sync.Enter();

        return objects.ToList();
    }

    public void Refresh(User user, bool forceRefresh = false)
    {
        using var @lock = Sync.Enter();

        var now = DateTime.UtcNow;

        if (!forceRefresh && (now.Subtract(user.LastRefresh).TotalMilliseconds < WorldOptions.RefreshIntervalMs))
            return;

        var nearbyObjects = ObjectsWithinRange<VisibleObject>(user);

        var nearbyUsers = new List<User>();
        var nearbyOthers = new List<VisibleObject>();
        var nearbyDoors = new List<Door>();

        foreach (var obj in nearbyObjects)
            switch (obj)
            {
                case User userObj:
                    nearbyUsers.Add(userObj);

                    break;
                case Creature creatureObj:
                    nearbyOthers.Add(creatureObj);

                    break;
                case GroundItem groundItemObj:
                    nearbyOthers.Add(groundItemObj);

                    break;
                case Door doorObj:
                    nearbyDoors.Add(doorObj);

                    break;
            }

        user.LastRefresh = now;
        user.Client.SendMapInfo();
        user.Client.SendLocation();
        user.Client.SendAttributes(StatUpdateType.Full);

        foreach (var nearbyUser in nearbyUsers)
        {
            if (nearbyUser.Equals(user))
                continue;

            if (user.IsVisibleTo(nearbyUser))
                nearbyUser.Client.SendDisplayUser(user);

            if (nearbyUser.IsVisibleTo(user))
                user.Client.SendDisplayUser(nearbyUser);
        }

        user.Client.SendVisibleObjects(nearbyOthers.ToArray());
        user.Client.SendDoors(nearbyDoors.ToArray());
        user.Client.SendMapLoadComplete();
        user.Client.SendDisplayUser(user);
        user.Client.SendRefreshResponse();

        ActivateReactors(user);
    }

    public void RemoveObject(MapObject mapObject)
    {
        using var @lock = Sync.Enter();

        Objects.Remove(mapObject.Id);

        if (mapObject is VisibleObject visibleObject)
            foreach (var user in ObjectsThatSee<User>(visibleObject))
                user.Client.SendRemoveObject(visibleObject.Id);
    }

    public void Show(Action<IWorldClient> action, Point source)
    {
        using var @lock = Sync.Enter();

        foreach (var user in ObjectsWithinRange<User>(source))
            action(user.Client);
    }

    public void Show(Action<IWorldClient> action, VisibleObject source)
    {
        using var @lock = Sync.Enter();

        foreach (var user in ObjectsThatSee<User>(source))
            action(user.Client);
    }

    public void ShowAnimation(Animation animation)
    {
        using var @lock = Sync.Enter();

        if (animation.TargetId.HasValue)
        {
            if (TryGetObject<Creature>(animation.TargetId.Value, out var target))
                foreach (var user in ObjectsThatSee<User>(target))
                    user.Client.SendAnimation(animation);
        } else if (animation.TargetPoint.HasValue)
            foreach (var user in ObjectsWithinRange<User>(animation.TargetPoint.Value))
                user.Client.SendAnimation(animation);
    }

    public void ShowBodyAnimation(BodyAnimation bodyAnimation, Creature source, byte? sound = null)
    {
        using var @lock = Sync.Enter();

        foreach (var user in ObjectsThatSee<User>(source))
            user.Client.SendBodyAnimation(
                source.Id,
                bodyAnimation,
                100,
                sound);
    }

    public void ShowOthers(Action<IWorldClient> action, VisibleObject source)
    {
        using var @lock = Sync.Enter();

        foreach (var user in ObjectsThatSee<User>(source))
            if (!user.Equals(source))
                action(user.Client);
    }

    public void ShowPublicMessage(PublicMessageType publicMessageType, string message, Creature source)
    {
        using var @lock = Sync.Enter();

        foreach (var user in ObjectsThatSee<User>(source))
            user.Client.SendPublicMessage(source.Id, publicMessageType, message);
    }

    public void ShowTurn(Direction direction, Creature source)
    {
        using var @lock = Sync.Enter();

        foreach (var user in ObjectsThatSee<User>(source))
            user.Client.SendCreatureTurn(source.Id, direction);
    }

    public void ShowUser(User source)
    {
        using var @lock = Sync.Enter();

        foreach (var user in ObjectsThatSee<User>(source))
            user.Client.SendDisplayUser(source);
    }

    public void SimpleAdd(MapObject mapObject)
    {
        using var @lock = Sync.Enter();
        Objects.Add(mapObject.Id, mapObject);
    }

    public void ToggleDoor(Point point)
    {
        using var @lock = Sync.Enter();

        var allDoors = Objects
                       .Values<Door>()
                       .ToDictionary(door => door.Point);

        IEnumerable<Door> GetSurroundingDoors(Point doorPoint)
        {
            foreach (var cardinalPoint in doorPoint.GetCardinalPoints())
                if (allDoors.TryGetValue(cardinalPoint, out var adjacentDoor))
                    yield return adjacentDoor;
        }

        if (allDoors.TryGetValue(point, out var targetDoor))
        {
            var allTouchingDoors = new HashSet<Door> { targetDoor };
            var pendingDiscovery = new Stack<Door>();
            pendingDiscovery.Push(targetDoor);

            //floodfill to find all touching doors
            while (pendingDiscovery.Any())
            {
                var popped = pendingDiscovery.Pop();

                foreach (var innerDoor in GetSurroundingDoors(popped.Point))
                    if (allTouchingDoors.Add(innerDoor))
                        pendingDiscovery.Push(innerDoor);
            }

            foreach (var user in Objects.Values<User>())
            {
                var doorsInRange = allTouchingDoors
                                   .Where(door => door.WithinRange(user))
                                   .ToArray();

                if (doorsInRange.Any())
                    user.Client.SendDoors(doorsInRange);
            }
        }
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