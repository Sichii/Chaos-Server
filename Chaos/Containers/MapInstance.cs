using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Chaos.Clients.Interfaces;
using Chaos.Core.Collections;
using Chaos.Core.Definitions;
using Chaos.Core.Extensions;
using Chaos.Core.Geometry;
using Chaos.Core.Utilities;
using Chaos.DataObjects;
using Chaos.Extensions;
using Chaos.Options;
using Chaos.Templates;
using Chaos.Templates.Interfaces;
using Chaos.WorldObjects;
using Chaos.WorldObjects.Abstractions;

namespace Chaos.Containers;

public class MapInstance : ITemplated<short, MapTemplate>
{
    public MapFlags Flags { get; set; }
    public string InstanceId { get; init; }
    public short MapId { get; set; }
    public sbyte Music { get; set; }
    public string Name { get; set; }
    public WorldOptions WorldOptions { get; set; } = null!;
    public Warp[][] WarpGroups { get; set; } = Array.Empty<Warp[]>();
    public MapTemplate Template { get; set; } = null!;
    private readonly TypeCheckingDictionary<uint, MapObject> Objects;
    //private readonly ConcurrentDictionary<Exchange>
    private readonly object Sync;
    
    public AutoReleasingMonitor AutoMonitor { get; }

    public MapInstance(string name, string instanceId)
    {
        Name = name;
        InstanceId = instanceId;
        Objects = new TypeCheckingDictionary<uint, MapObject>();
        Sync = new object();
        AutoMonitor = new AutoReleasingMonitor(Sync);
    }

    public void AddObject(VisibleObject visibleObject, Point point)
    {
        lock (Sync)
        {
            visibleObject.Point = point;
            visibleObject.MapInstance = this;
            Objects.Add(visibleObject.Id, visibleObject);

            if (visibleObject is User user)
            {
                var nearbyObjects = ObjectsWithinRange<VisibleObject>(point);

                var nearbyUsers = new List<User>();
                var nearbyOthers = new List<VisibleObject>();
                var nearbyDoors = new List<Door>();
                
                foreach(var obj in nearbyObjects)
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
    }

    public void SimpleAdd(MapObject mapObject)
    {
        lock (Sync)
            Objects.Add(mapObject.Id, mapObject);
    }

    public void AddObjects<T>(ICollection<T> visibleObjects) where T: VisibleObject
    {
        lock (Sync)
        {
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
    }

    public void RemoveObject(MapObject mapObject)
    {
        lock (Sync)
        {
            Objects.Remove(mapObject.Id);
            
            if(mapObject is VisibleObject visibleObject)
                foreach (var user in ObjectsThatSee<User>(visibleObject))
                    user.Client.SendRemoveObject(visibleObject.Id);
        }
    }

    public bool TryGetObject<T>(uint id, [MaybeNullWhen(false)] out T obj)
    {
        lock (Sync)
            return Objects.TryGetValue(id, out obj);
    }

    public IEnumerable<T> ObjectsWithinRange<T>(MapObject mapObject, int distance = 13) where T: MapObject
    {
        var objects = Objects.Values<T>()
            .Where(obj => mapObject.WithinRange(obj, distance));

        if (Monitor.IsEntered(Sync))
            return objects;

        lock (Sync)
            return objects.ToList();
    }

    public IEnumerable<T> ObjectsWithinRange<T>(Point point, int distance = 13) where T: MapObject
    {
        var objects = Objects.Values<T>()
            .Where(obj => obj.WithinRange(point, distance));

        if (Monitor.IsEntered(Sync))
            return objects;

        lock (Sync)
            return objects.ToList();
    }

    public IEnumerable<T> ObjectsVisibleTo<T>(MapObject mapObject, int distance = 13) where T: VisibleObject
    {
        var objects = Objects.Values<T>()
            .Where(obj => mapObject.WithinRange(obj, distance));

        if (mapObject is User user)
            objects = objects.ThatAreVisibleTo(user);

        if (Monitor.IsEntered(Sync))
            return objects;

        lock (Sync)
            return objects.ToList();
    }

    public IEnumerable<T> ObjectsAtPoint<T>(Point point) where T: MapObject
    {
        var objects = Objects.Values<T>()
            .Where(obj => obj.Point == point)
            .OrderByDescending(obj => obj.Creation);

        if (Monitor.IsEntered(Sync))
            return objects;

        lock (Sync)
            return objects.ToList();
    }

    public IEnumerable<T> ObjectsThatSee<T>(VisibleObject visibleObject, int distance = 13) where T: MapObject
    {
        var objects = Objects.Values<T>()
            .Where(obj => visibleObject.WithinRange(obj, distance));

        if (objects is IEnumerable<Creature> creatures)
            objects = creatures.ThatCanSee(visibleObject).Cast<T>();

        if (Monitor.IsEntered(Sync))
            return objects;

        lock (Sync)
            return objects.ToList();
    }

    public void Refresh(User user, bool forceRefresh = false)
    {
        lock (Sync)
        {
            var now = DateTime.UtcNow;

            if (!forceRefresh && (now.Subtract(user.LastRefresh).TotalMilliseconds < WorldOptions.RefreshIntervalMs))
                return;
            
            var nearbyObjects = ObjectsWithinRange<VisibleObject>(user);

            var nearbyUsers = new List<User>();
            var nearbyOthers = new List<VisibleObject>();
            var nearbyDoors = new List<Door>();
                
            foreach(var obj in nearbyObjects)
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
    }
    
    public void ClientWalk(User user, Direction direction)
    {
        lock (Sync)
        {
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
            foreach(var obj in visibleBefore.Except(visibleAfter))
                if (obj is GroundItem or Creature)
                {
                    user.Client.SendRemoveObject(obj.Id);

                    if (obj is User otherUser)
                        otherUser.Client.SendRemoveObject(user.Id);
                }
            
            //for all objects we couldnt see before
            //display them to us, display us to them if theyre a user
            foreach(var obj in visibleAfter.Except(visibleBefore))
                if (obj is User otherUser)
                {
                    if(user.IsVisibleTo(otherUser))
                        otherUser.Client.SendDisplayUser(user);
                    
                    if(otherUser.IsVisibleTo(user))
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
    }

    public void ShowTurn(Direction direction, Creature source)
    {
        lock (Sync)
            foreach (var user in ObjectsThatSee<User>(source))
                user.Client.SendCreatureTurn(source.Id, direction);
    }

    public void ShowUser(User source)
    {
        lock(Sync)
            foreach(var user in ObjectsThatSee<User>(source))
                user.Client.SendDisplayUser(source);
    }
    
    public void CreatureWalk(Creature creature, Direction direction)
    {
        lock (Sync)
        {
            creature.Direction = direction;
            var startPoint = creature.Point;
            var newPoint = startPoint.Offset(direction);
            
            //TODO: do we want walkthrough mobs to walk over walls?

            if (!IsWalkable(newPoint))
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
    }
    
    public bool IsWall(Point point) => Template.IsWall(point);
    
    public bool IsWalkable(Point point)
    {
        if (IsWall(point))
            return false;
        
        lock (Sync)
            return Objects
                       .Values<Creature>()
                       .All(creature => (creature.Point != point) || (creature.Type == CreatureType.WalkThrough));
    }

    public void ActivateReactors(Creature creature)
    {
        lock (Sync)
        {
            var reactors = ObjectsAtPoint<ReactorTile>(creature.Point);

            foreach (var reactor in reactors)
                reactor.Activate(creature);
        }
    }

    public void Click(uint id, User source)
    {
        lock (Sync)
            if(TryGetObject<VisibleObject>(id, out var obj))
                if (obj.WithinRange(source) && obj.IsVisibleTo(source))
                    obj.OnClicked(source);
    }

    public void Click(Point point, User source)
    {
        lock (Sync)
        {
            var obj = ObjectsAtPoint<VisibleObject>(point)
                .ThatAreVisibleTo(source)
                .FirstOrDefault();

            obj?.OnClicked(source);
        }
    }

    public void ShowPublicMessage(PublicMessageType publicMessageType, string message, Creature source)
    {
        lock (Sync)
            foreach (var user in ObjectsThatSee<User>(source))
                user.Client.SendPublicMessage(source.Id, publicMessageType, message);
    }

    public void ShowBodyAnimation(BodyAnimation bodyAnimation, Creature source, byte? sound = null)
    {
        lock(Sync)
            foreach (var user in ObjectsThatSee<User>(source))
                user.Client.SendBodyAnimation(source.Id, bodyAnimation, 100, sound);
    }
    
    public void Show(Action<IWorldClient> action, Point source)
    {
        lock (Sync)
            foreach (var user in ObjectsWithinRange<User>(source))
                action(user.Client);
    }

    public void Show(Action<IWorldClient> action, VisibleObject source)
    {
        lock(Sync)
            foreach (var user in ObjectsThatSee<User>(source))
                action(user.Client);
    }

    public void ShowOthers(Action<IWorldClient> action, VisibleObject source)
    {
        lock(Sync)
            foreach (var user in ObjectsThatSee<User>(source))
                if (!user.Equals(source))
                    action(user.Client);
    }
    
    public void ToggleDoor(Point point)
    {
        lock (Sync)
        {
            var allDoors = Objects
                .Values<Door>()
                .ToDictionary(door => door.Point);

            IEnumerable<Door> GetSurroundingDoors(Point doorPoint)
            {
                foreach(var cardinalPoint in doorPoint.GetCardinalPoints())
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
    }
}