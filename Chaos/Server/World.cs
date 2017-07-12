using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Chaos.Objects;
using System;
using System.IO;

namespace Chaos
{
    internal class World
    {
        internal Server Server { get; }
        internal ConcurrentDictionary<ushort, Map> Maps { get; set; }
        internal ConcurrentDictionary<uint, WorldMap> WorldMaps { get; set; }
        internal ConcurrentDictionary<int, Group> Groups { get; set; }
        internal ConcurrentDictionary<string, Guild> Guilds { get; set; }
        internal ConcurrentDictionary<int, Exchange> Exchanges { get; set; }
        

        internal World(Server server)
        {
            Server = server;
            Maps = new ConcurrentDictionary<ushort, Map>();
            WorldMaps = new ConcurrentDictionary<uint, WorldMap>();
            Groups = new ConcurrentDictionary<int, Group>();
            Guilds = new ConcurrentDictionary<string, Guild>(StringComparer.CurrentCultureIgnoreCase);
            Exchanges = new ConcurrentDictionary<int, Exchange>();
        }

        internal void Load()
        {
            //load maps
            using (BinaryReader reader = new BinaryReader(File.OpenRead(Paths.MapKey)))
            {
                uint mapCount = reader.ReadUInt32();
                for(int i = 0; i < mapCount; i++)
                {
                    Map map = new Map(reader.ReadUInt16(), reader.ReadByte(), reader.ReadByte(), reader.ReadString(), reader.ReadByte(), (MapFlags)reader.ReadUInt32());
                    map.SetData($@"{Paths.MapFiles}{map.Id}.map");
                    Maps[map.Id] = map;
                }
            }

            //load doors
            //load warps
            //load merchants
            //other shit?
        }

        /// <summary>
        /// Adds a single object to a map. Sends and sets all relevant data.
        /// </summary>
        /// <param name="vObject">Any visible object.</param>
        /// <param name="location">The map and point you want to add it to.</param>
        internal void AddObjectToMap(VisibleObject vObject, Location location)
        {
            if (vObject == null) return;

            lock(Maps[location.MapId].Objects)
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
                    
                    user.Client.Enqueue(Server.Packets.MapChangePending());     //pending map change
                    user.Client.Enqueue(Server.Packets.MapInfo(user.Map));      //send map info
                    user.Client.Enqueue(Server.Packets.Location(user.Point));   //send location

                    foreach (User u2s in usersToSend)
                    {   
                        user.Client.Enqueue(Server.Packets.DisplayUser(u2s));   //send it all the users
                        u2s.Client.Enqueue(Server.Packets.DisplayUser(user));   //send all the users this user as well
                    }

                    user.Client.Enqueue(Server.Packets.DisplayItemMonster(itemMonsterToSend.ToArray()));    //send it all the items, monsters, and merchants
                    user.Client.Enqueue(Server.Packets.Door(DoorsVisibleFrom(user).ToArray()));     //send the user all nearby doors
                    user.Client.Enqueue(Server.Packets.MapChangeComplete());    //send it mapchangecomplete
                    user.Client.Enqueue(Server.Packets.MapLoadComplete());      //send it maploadcomplete
                    user.Client.Enqueue(Server.Packets.DisplayUser(user));      //send it itself
                }
                else //if this object isnt a user
                    foreach (User u2s in usersToSend)
                        u2s.Client.Enqueue(Server.Packets.DisplayItemMonster(vObject)); //send all the visible users this object
            }
        }

        /// <summary>
        /// Adds many objects to the map. NON-USERS ONLY!
        /// </summary>
        /// <param name="vObjects">Any non-user visibleobject</param>
        /// <param name="location">The map and point you want to add it to.</param>
        internal void AddObjectsToMap(List<VisibleObject> vObjects, Location location)
        {
            if (vObjects.Count == 0) return;

            lock(Maps[location.MapId].Objects)
            {
                //change location of each object and add each item to the map
                foreach(VisibleObject vObj in vObjects)
                {
                    vObj.Map = Maps[location.MapId];
                    vObj.Point = location.Point;
                    Maps[location.MapId].Objects.AddOrUpdate(vObj.Id, vObj as WorldObject, (key, oldValue) => vObj);
                }

                //send all the visible users these objects
                foreach (User user in ObjectsVisibleFrom(vObjects[0]).OfType<User>())
                    user.Client.Enqueue(Server.Packets.DisplayItemMonster(vObjects.ToArray()));
            }
        }

        /// <summary>
        /// Removes a single object from the map.
        /// </summary>
        /// <param name="vObject">Any visible object you want removed.</param>
        internal void RemoveObjectFromMap(VisibleObject vObject)
        {
            if (vObject == null) return;

            lock(vObject.Map.Objects)
            {
                foreach(User user in ObjectsVisibleFrom(vObject))
                    user.Client.Enqueue(Server.Packets.RemoveObject(vObject));
            }
        }

        internal List<VisibleObject> ObjectsVisibleFrom(VisibleObject vObject)
        {
            lock (vObject.Map.Objects)
            {
                return Maps[vObject.Location.MapId].Objects.OfType<VisibleObject>().Where(obj => obj.WithinRange(vObject.Location.Point) && vObject != obj).ToList();
            }
        }

        internal List<Door> DoorsVisibleFrom(User user) => user.Map.Doors.Values.Where(door => user.WithinRange(door.Point)).ToList();

        internal void Refresh(User user)
        {
            lock(user.Map.Objects)
            {

            }
        }
    }
}
