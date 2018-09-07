// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Map
    {
        internal readonly object Sync = new object();
        private Dictionary<int, WorldObject> Objects;
        private List<Effect> Effects;

        internal Dictionary<Point, Tile> Tiles { get; }
        internal byte[] Data { get; private set; }
        internal ushort CheckSum { get; private set; }

        public ConcurrentDictionary<Point, Door> Doors { get; set; }
        public Dictionary<Point, Warp> Warps { get; set; }
        public Dictionary<Point, WorldMap> WorldMaps { get; set; }
        [JsonProperty]
        public ushort Id { get; }
        public byte SizeX { get; set; }
        public byte SizeY { get; set; }
        public MapFlags Flags { get; set; }
        public string Name { get; set; }
        public sbyte Music { get; set; }

        /// <summary>
        /// Master constructor for an object representing an in-game map.
        /// </summary>
        public Map(ushort id, byte sizeX, byte sizeY, MapFlags flags, string name, sbyte music)
        {
            Id = id;
            SizeX = sizeX;
            SizeY = sizeY;
            Flags = flags;
            Name = name;
            Tiles = new Dictionary<Point, Tile>();
            Warps = new Dictionary<Point, Warp>();
            WorldMaps = new Dictionary<Point, WorldMap>();
            Objects = new Dictionary<int, WorldObject>();
            Doors = new ConcurrentDictionary<Point, Door>();
            Effects = new List<Effect>();
        }

        /// <summary>
        /// Json constructor for an object representing an in-game map. Only the Id is serialized. The map is then fetched from a pre-populated list from the world.
        /// </summary>
        /// <param name="id"></param>
        [JsonConstructor]
        internal Map(ushort id)
        {
            Id = id;
        }

        /// <summary>
        /// Loads the tile data from file for the map.
        /// </summary>
        /// <param name="path"></param>
        internal void LoadData(string path)
        {
            if (File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);
                Tiles.Clear();
                Data = data;

                int index = 0;
                for (ushort y = 0; y < SizeY; y++)
                    for (ushort x = 0; x < SizeX; x++)
                        Tiles[new Point(x, y)] = new Tile((short)(data[index++] | data[index++] << 8), (short)(data[index++] | data[index++] << 8), (short)(data[index++] | data[index++] << 8));
            }

            CheckSum = Crypto.Generate16(Data);
        }

        /// <summary>
        /// Synchronously adds a single object to a map. Sends and sets all relevant data.
        /// </summary>
        /// <param name="vObject">Any visible object.</param>
        /// <param name="point">The point you want to add it to.</param>
        internal void AddObject(VisibleObject vObject, Point point)
        {
            if (vObject == null) return;

            lock (Sync)
            {
                //change location of the object and add it to the map
                vObject.Map = this;
                vObject.Point = point;
                Objects.Add(vObject.Id, vObject);

                List<VisibleObject> itemMonsterToSend = new List<VisibleObject>();
                List<User> usersToSend = new List<User>();

                //get all objects that would be visible to this object and sort them
                foreach (VisibleObject obj in vObject.Map.ObjectsVisibleFrom(vObject))
                    if (obj is User)
                        usersToSend.Add(obj as User);
                    else
                        itemMonsterToSend.Add(obj);

                //if this object is a user
                if (vObject is User)
                {
                    User user = vObject as User;

                    user.Client.Enqueue(ServerPackets.MapChangePending());     //send pending map change
                    user.Client.Enqueue(ServerPackets.MapInfo(user.Map));      //send map info
                    user.Client.Enqueue(ServerPackets.Location(user.Point));   //send location

                    foreach (User u2s in usersToSend)
                    {
                        user.Client.Enqueue(ServerPackets.DisplayUser(u2s));   //send it all the users
                        u2s.Client.Enqueue(ServerPackets.DisplayUser(user));   //send all the users this user as well
                    }

                    user.Client.Enqueue(ServerPackets.DisplayItemMonster(itemMonsterToSend.ToArray()));    //send it all the items, monsters, and merchants
                    user.Client.Enqueue(ServerPackets.Door(user.Map.DoorsVisibleFrom(user).ToArray()));     //send the user all nearby doors
                    user.Client.Enqueue(ServerPackets.MapChangeComplete());    //send it mapchangecomplete
                    user.Client.Enqueue(ServerPackets.MapLoadComplete());      //send it maploadcomplete
                    user.Client.Enqueue(ServerPackets.DisplayUser(user));      //send it itself

                    user.AnimationHistory.Clear();
                }
                else //if this object isnt a user
                    foreach (User u2s in usersToSend)
                        u2s.Client.Enqueue(ServerPackets.DisplayItemMonster(vObject)); //send all the visible users this object
            }
        }

        /// <summary>
        /// Synchronously adds many objects to the map. NON-USERS ONLY!
        /// </summary>
        /// <param name="vObjects">Any non-user visibleobject</param>
        /// <param name="point">The point you want to add it to.</param>
        internal void AddObjects(IEnumerable<VisibleObject> vObjects, Point point)
        {
            if (vObjects.Count() == 0) return;

            lock (Sync)
            {
                //change location of each object and add each item to the map
                foreach (VisibleObject vObj in vObjects)
                {
                    vObj.Map = this;
                    vObj.Point = point;
                    Objects.Add(vObj.Id, vObj);
                }

                //send all the visible users these objects
                foreach (User user in ObjectsVisibleFrom(vObjects.First()).OfType<User>())
                    user.Client.Enqueue(ServerPackets.DisplayItemMonster(vObjects.ToArray()));
            }
        }

        /// <summary>
        /// Synchronously removes a single object from the map.
        /// </summary>
        /// <param name="vObject">Any visible object you want removed.</param>
        /// <param name="worldMap">Whether or not they are stepping into a worldMap.</param>
        internal void RemoveObject(VisibleObject vObject, bool worldMap = false)
        {
            if (vObject == null) return;

            lock (Sync)
            {
                Objects.Remove(vObject.Id);
                foreach (User user in ObjectsVisibleFrom(vObject).OfType<User>())
                    user.Client.Enqueue(ServerPackets.RemoveObject(vObject));

                if (!worldMap)
                    vObject.Map = null;
            }
        }

        /// <summary>
        /// Synchronously adds an effect to the map.
        /// </summary>
        /// <param name="effect">The effect to add.</param>
        internal void AddEffect(Effect effect)
        {
            lock (Sync)
                Effects.Add(effect);
        }

        /// <summary>
        /// Synchronously removes a single effect from the map.
        /// </summary>
        /// <param name="effect">The effect to remove.</param>
        internal void RemoveEffect(Effect effect)
        {
            lock (Sync)
                Effects.Remove(effect);
        }

        /// <summary>
        /// Synchronously retrieves all objects the given object can see.
        /// </summary>
        /// <param name="vObject">Object to base from.</param>
        /// <param name="include">Whether or not to include the base object.</param>
        /// <param name="distance">Optional distance from the object to retrieve from.</param>
        internal IEnumerable<VisibleObject> ObjectsVisibleFrom(VisibleObject vObject, bool include = false, byte distance = 13)
        {
            lock (Sync)
                return Objects.Values.OfType<VisibleObject>().Where(obj => obj.Point.Distance(vObject.Point) <= distance && (include ? true : vObject != obj));
        }

        /// <summary>
        /// Synchronously retrieves all objects visible from a given point.
        /// </summary>
        /// <param name="point">The point of origin.</param>
        /// <param name="include">Whether or not to include the origin point.</param>
        /// <param name="distance">Optional distance from the point to retreive from.</param>
        /// <returns></returns>
        internal IEnumerable<VisibleObject> ObjectsVisibleFrom(Point point, bool include = false, byte distance = 13)
        {
            lock (Sync)
                return Objects.Values.OfType<VisibleObject>().Where(obj => obj.Point.Distance(point) <= distance && (include ? true : obj.Point != point));
        }

        /// <summary>
        /// Synchronously retrieves all doors visible from the user.
        /// </summary>
        /// <param name="user">The user to base from.</param>
        internal IEnumerable<Door> DoorsVisibleFrom(User user)
        {
            lock (Sync)
                return Doors.Values.Where(door => user.WithinRange(door.Point));
        }

        /// <summary>
        /// Synchronously retrieves all effects visible from the creature.
        /// </summary>
        /// <param name="creature">The creature to base the search from.</param>
        internal IEnumerable<Effect> EffectsVisibleFrom(Creature creature)
        {
            lock (creature.Map.Sync)
            {
                foreach (Effect eff in Effects.Where(e => e.Animation.TargetPoint.Distance(creature.Point) < 13))
                    yield return eff;
            }
        }

        /// <summary>
        /// Attempts to synchronously retreive a user by searching through the objects for the given name.
        /// </summary>
        /// <param name="name">The name of the user to search for.</param>
        /// <param name="user">Reference to the user to set.</param>
        internal bool TryGetUser(string name, out User user)
        {
            user = null;

            lock (Sync)
                user = Objects.Values.FirstOrDefault(obj => obj.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)) as User;

            return user != null;
        }

        /// <summary>
        /// Attempts to synchronously retreive an object by searching through the objects for the given id.
        /// </summary>
        /// <param name="id">The id of the object to search for.</param>
        /// <param name="obj">Reference to the object to set.</param>
        internal bool TryGetObject<T>(int id, out T obj) where T : class
        {
            WorldObject wObj;
            obj = null;

            lock (Sync)
                if (Objects.TryGetValue(id, out wObj))
                    obj = wObj as T;

            return obj != null;
        }

        /// <summary>
        /// Attempts to synchronously retreive an object by searching through the maps for the given point.
        /// </summary>
        /// <param name="p">The point of the object to search for.</param>
        /// <param name="obj">Reference to the object to set.</param>
        internal bool TryGetObject<T>(Point p, out T obj) where T : class
        {
            obj = null;

            lock (Sync)
            {
                Type tType = typeof(T);

                if (typeof(VisibleObject).IsAssignableFrom(tType))
                    obj = Objects.Values.FirstOrDefault(tObj => (tObj as VisibleObject)?.Point == p) as T;
                else if (typeof(Door).IsAssignableFrom(tType))
                    obj = Doors.ContainsKey(p) ? Doors[p] as T : null;
                else if (typeof(Warp).IsAssignableFrom(tType))
                    obj = Warps.ContainsKey(p) ? Warps[p] as T : null;
                else if (typeof(WorldMap).IsAssignableFrom(tType))
                    obj = WorldMaps.ContainsKey(p) ? WorldMaps[p] as T : null;
                else if (typeof(Effect).IsAssignableFrom(tType))
                    obj = Effects.FirstOrDefault(e => e.Animation.TargetPoint == p) as T;
            }

            return obj != null;
        }

        /// <summary>
        /// Attempts to synchronously retreive all objects of a given type on a given map.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="map">The map to return from.</param>
        internal IEnumerable<T> GetAllObjects<T>()
        {
            lock (Sync)
            {
                Type tType = typeof(T);

                if (typeof(VisibleObject).IsAssignableFrom(tType))
                    return Objects.Values.OfType<T>();
                else if (typeof(Door).IsAssignableFrom(tType))
                    return Doors.Values.OfType<T>();
                else if (typeof(Warp).IsAssignableFrom(tType))
                    return Warps.Values.OfType<T>();
                else if (typeof(WorldMap).IsAssignableFrom(tType))
                    return WorldMaps.Values.OfType<T>();
                else if (typeof(Effect).IsAssignableFrom(tType))
                    return Effects.OfType<T>();
            }

            return new List<T>();
        }

        /// <summary>
        /// Checks if the map has a certain flag.
        /// </summary>
        internal bool HasFlag(MapFlags flag) => Flags.HasFlag(flag);

        /// <summary>
        /// Checks if a point is within the bounds of the map.
        /// </summary>
        internal bool WithinMap(Point p) => p.X >= 0 && p.Y >= 0 && p.X < SizeX && p.Y < SizeY;

        /// <summary>
        /// Checks if a point is within the bounds of the map, or is a wall.
        /// </summary>
        internal bool IsWall(Point p) => !WithinMap(p) || Tiles[p].IsWall;

        /// <summary>
        /// Checks if a given point is within the bounds of the map, is a wall, or has a monster, door, or other object already on it.
        /// </summary>
        internal bool IsWalkable(Point p)
        {
            lock (Sync)
                return !IsWall(p) && (Doors.Keys.Contains(p) ? Doors[p].Opened : true) && !Objects.Values.OfType<Creature>().Any(creature => creature.Type != CreatureType.WalkThrough && creature.Point == p);
        }
    }
}
