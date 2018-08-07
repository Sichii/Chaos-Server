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
using System.Linq;
using System;
using System.IO;

namespace Chaos
{
    internal class World
    {
        internal Server Server { get; }
        internal ConcurrentDictionary<ushort, Map> Maps { get; set; }
        internal ConcurrentDictionary<uint, WorldMap> WorldMaps { get; set; }
        internal ConcurrentDictionary<string, Guild> Guilds { get; set; }
        internal ConcurrentDictionary<int, Group> Groups { get; set; }
        internal ConcurrentDictionary<int, Exchange> Exchanges { get; set; }

        internal World(Server server)
        {
            Server = server;
            Maps = new ConcurrentDictionary<ushort, Map>();
            WorldMaps = new ConcurrentDictionary<uint, WorldMap>();
            Guilds = new ConcurrentDictionary<string, Guild>(StringComparer.CurrentCultureIgnoreCase);
            Groups = new ConcurrentDictionary<int, Group>();
            Exchanges = new ConcurrentDictionary<int, Exchange>();
        }

        internal void Load()
        {
            Server.WriteLog("Creating world objects...");

            Guild team = new Guild();
            Guilds.TryAdd(team.Name, team);

            #region Load Maps
            using (BinaryReader reader = new BinaryReader(new MemoryStream(Server.DataBase.MapData)))
            {
                reader.ReadInt32();

                //load worldmaps
                ushort worldMapCount = reader.ReadUInt16();
                for (int wMap = 0; wMap < worldMapCount; ++wMap)
                {
                    string field = reader.ReadString();
                    byte nodeCount = reader.ReadByte();
                    WorldMapNode[] nodes = new WorldMapNode[nodeCount];
                    for (int i = 0; i < nodeCount; i++)
                    {
                        ushort x = reader.ReadUInt16();
                        ushort y = reader.ReadUInt16();
                        string name = reader.ReadString();
                        ushort mapId = reader.ReadUInt16();
                        byte dX = reader.ReadByte();
                        byte dY = reader.ReadByte();
                        nodes[i] = new WorldMapNode(new Point(x, y), name, mapId, new Point(dX, dY));
                    }

                    WorldMap worldMap = new WorldMap(field, nodes);

                    uint checkSum = worldMap.GetCheckSum();
                    WorldMaps[checkSum] = worldMap;
                }

                //load maps
                ushort mapCount = reader.ReadUInt16();
                for (int map = 0; map < mapCount; map++)
                {
                    //load map information
                    ushort mapId = reader.ReadUInt16();
                    byte sizeX = reader.ReadByte();
                    byte sizeY = reader.ReadByte();
                    string name = reader.ReadString();
                    MapFlags flags = (MapFlags)reader.ReadUInt32();
                    sbyte music = reader.ReadSByte();
                    Map newMap = new Map(mapId, sizeX, sizeY, flags, name, music);

                    //load doors
                    byte doorCount = reader.ReadByte();
                    for (byte b = 0; b < doorCount; b++)
                    {
                        ushort x = reader.ReadUInt16();
                        ushort y = reader.ReadUInt16();
                        bool opensRight = reader.ReadBoolean();
                        newMap.Doors[new Point(x, y)] = new Door(mapId, x, y, false, opensRight);
                    }

                    //load warps
                    ushort warpCount = reader.ReadUInt16();
                    for (int i = 0; i < warpCount; i++)
                    {
                        byte sourceX = reader.ReadByte();
                        byte sourceY = reader.ReadByte();
                        ushort targetMapId = reader.ReadUInt16();
                        byte targetX = reader.ReadByte();
                        byte targetY = reader.ReadByte();
                        Warp warp = new Warp(sourceX, sourceY, targetX, targetY, mapId, targetMapId);
                        newMap.Warps[new Point(sourceX, sourceY)] = warp;
                    }

                    //load worldmaps for this map
                    byte wMapCount = reader.ReadByte();
                    for (int i = 0; i < wMapCount; i++)
                    {
                        byte x = reader.ReadByte();
                        byte y = reader.ReadByte();
                        uint CheckSum = reader.ReadUInt32();
                        if (WorldMaps.ContainsKey(CheckSum))
                            newMap.WorldMaps[new Point(x, y)] = WorldMaps[CheckSum];
                    }

                    //add the map to the map list
                    if (Maps.TryAdd(mapId, newMap))
                        newMap.LoadData($@"{Paths.MapFiles}lod{newMap.Id}.map");
                }
            }
            #endregion
        }

        internal void Populate()
        {
            #region Load Merchants
            foreach (Merchant merchant in Game.Merchants)
                AddObjectToMap(merchant, merchant.Location);
            #endregion
        }

        /// <summary>
        /// Adds a single object to a map. Sends and sets all relevant data.
        /// </summary>
        /// <param name="vObject">Any visible object.</param>
        /// <param name="location">The map and point you want to add it to.</param>
        internal void AddObjectToMap(VisibleObject vObject, Location location)
        {
            if (vObject == null) return;

            lock(Maps[location.MapId].Sync)
            {
                //change location of the object and add it to the map
                vObject.Map = Maps[location.MapId];
                vObject.Point = location.Point;
                Maps[location.MapId].Objects.AddOrUpdate(vObject.Id, vObject as WorldObject, (key, oldValue) => vObject);

                List<VisibleObject> itemMonsterToSend = new List<VisibleObject>();
                List<User> usersToSend = new List<User>();

                //get all objects that would be visible to this object and sort them
                foreach(VisibleObject obj in ObjectsVisibleFrom(vObject))
                    if(obj is User)
                        usersToSend.Add(obj as User);
                    else
                        itemMonsterToSend.Add(obj);

                //if this object is a user
                if(vObject is User)
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
                    user.Client.Enqueue(ServerPackets.Door(DoorsVisibleFrom(user).ToArray()));     //send the user all nearby doors
                    user.Client.Enqueue(ServerPackets.MapChangeComplete());    //send it mapchangecomplete
                    user.Client.Enqueue(ServerPackets.MapLoadComplete());      //send it maploadcomplete
                    user.Client.Enqueue(ServerPackets.DisplayUser(user));      //send it itself
                }
                else //if this object isnt a user
                    foreach (User u2s in usersToSend)
                        u2s.Client.Enqueue(ServerPackets.DisplayItemMonster(vObject)); //send all the visible users this object
            }
        }

        /// <summary>
        /// Adds many objects to the map. NON-USERS ONLY!
        /// </summary>
        /// <param name="vObjects">Any non-user visibleobject</param>
        /// <param name="location">The map and point you want to add it to.</param>
        internal void AddObjectsToMap(IEnumerable<VisibleObject> vObjects, Location location)
        {
            if (vObjects.Count() == 0) return;

            lock(Maps[location.MapId].Sync)
            {
                //change location of each object and add each item to the map
                foreach(VisibleObject vObj in vObjects)
                {
                    vObj.Map = Maps[location.MapId];
                    vObj.Point = location.Point;
                    Maps[location.MapId].Objects.AddOrUpdate(vObj.Id, vObj as WorldObject, (key, oldValue) => vObj);
                }

                //send all the visible users these objects
                foreach (User user in ObjectsVisibleFrom(vObjects.First()).OfType<User>())
                    user.Client.Enqueue(ServerPackets.DisplayItemMonster(vObjects.ToArray()));
            }
        }

        /// <summary>
        /// Removes a single object from the map.
        /// </summary>
        /// <param name="vObject">Any visible object you want removed.</param>
        /// <param name="worldMap">Whether or not they are stepping into a worldMap.</param>
        internal void RemoveObjectFromMap(VisibleObject vObject, bool worldMap = false)
        {
            if (vObject == null) return;

            lock(vObject.Map.Sync)
            {
                WorldObject w;
                if (vObject.Map.Objects.TryRemove(vObject.Id, out w))
                {
                    foreach (User user in ObjectsVisibleFrom(vObject).OfType<User>())
                        user.Client.Enqueue(ServerPackets.RemoveObject(vObject));

                    if (!worldMap)
                        vObject.Map = null;
                }
            }
        }

        /// <summary>
        /// Gets all objects the given object can see
        /// </summary>
        /// <param name="vObject">Object to base from.</param>
        /// <returns></returns>
        internal IEnumerable<VisibleObject> ObjectsVisibleFrom(VisibleObject vObject, bool include = false, byte distance = 13)
        {
            lock (vObject.Map.Sync)
                return vObject.Map.Objects.Values.OfType<VisibleObject>().Where(obj => obj.Point.Distance(vObject.Point) <= distance && (include ? true : vObject != obj));
        }

        internal IEnumerable<VisibleObject> ObjectsVisibleFrom(Point point, Map map, bool include = false, byte distance = 13)
        {
            lock (map.Sync)
                return map.Objects.Values.OfType<VisibleObject>().Where(obj => obj.Point.Distance(point) <= distance && (include ? true : obj.Point != point));
        }

        /// <summary>
        /// Gets all doors visible from the user.
        /// </summary>
        /// <param name="user">The user to base from.</param>
        /// <returns></returns>
        internal IEnumerable<Door> DoorsVisibleFrom(User user)
        {
            lock (user.Map.Sync)
                return user.Map.Doors.Values.Where(door => user.WithinRange(door.Point));
        }

        /// <summary>
        /// Attempts to retreive a user by searching through the maps for the given name.
        /// </summary>
        /// <param name="name">The name of the user to search for.</param>
        /// <param name="user">Reference to the user to set.</param>
        /// <param name="mapToTry">Map to try retreiving from.</param>
        internal bool TryGetUser(string name, out User user, Map mapToTry)
        {
            user = null;

            if (mapToTry != null)
                lock (mapToTry.Sync)
                    user = mapToTry.Objects.Values.FirstOrDefault(obj => obj.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)) as User;
            else
                foreach (Map map in Maps.Values)
                    if (TryGetUser(name, out user, map))
                        return true;

            return user != null;
        }

        /// <summary>
        /// Attempts to retreive an object by searching through the maps for the given id.
        /// </summary>
        /// <param name="id">The id of the object to search for.</param>
        /// <param name="obj">Reference to the object to set.</param>
        /// <param name="mapToTry">Map to try retreiving from.</param>
        internal bool TryGetObject<T>(int id, out T obj, Map mapToTry = null) where T : class
        {
            WorldObject wObj;
            obj = null;

            if (mapToTry != null)
            {
                lock (mapToTry.Sync)
                    if (mapToTry.Objects.TryGetValue(id, out wObj))
                        obj = wObj as T;
            }
            else
                foreach (Map map in Maps.Values)
                    if (TryGetObject(id, out obj, map))
                        return true;

            return obj != null;
        }

        /// <summary>
        /// Attempts to retreive an object by searching through the maps for the given id.
        /// </summary>
        /// <param name="p">The point of the object to search for.</param>
        /// <param name="obj">Reference to the object to set.</param>
        /// <param name="mapToTry">Map to try retreiving from.</param>
        internal bool TryGetObject<T>(Point p, out T obj, Map mapToTry = null) where T : class
        {
            obj = null;

            if (mapToTry != null)
                lock (mapToTry.Sync)
                {
                    Type tType = typeof(T);

                    if (typeof(VisibleObject).IsAssignableFrom(tType))
                        obj = mapToTry.Objects.Values.OfType<T>().LastOrDefault(tObj => (tObj as VisibleObject)?.Point == p);
                    else if (typeof(Door).IsAssignableFrom(tType))
                        obj = mapToTry.Doors.ContainsKey(p) ? mapToTry.Doors[p] as T : null;
                    else if (typeof(Warp).IsAssignableFrom(tType))
                        obj = mapToTry.Warps.ContainsKey(p) ? mapToTry.Warps[p] as T : null;
                    else if (typeof(WorldMap).IsAssignableFrom(tType))
                        obj = mapToTry.WorldMaps.ContainsKey(p) ? mapToTry.WorldMaps[p] as T : null;
                }
            else
                foreach (Map map in Maps.Values)
                    if (TryGetObject(p, out obj, map))
                        return true;

            return obj != null;
        }

        /// <summary>
        /// Cleans up active world info of the user.
        /// </summary>
        /// <param name="user">The user to clean up</param>
        internal void ScrubUser(User user)
        {
            user.Group?.TryRemove(user.Id);
            user.Exchange?.Cancel(user);
            user.Save();
            RemoveObjectFromMap(user);

            //remove from other things
        }
    }
}
