using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Chaos.Objects;

namespace Chaos
{
    internal class World
    {
        internal ConcurrentDictionary<Socket, Client> Clients { get; set; }
        internal ConcurrentDictionary<ushort, Map> Maps { get; set; }
        internal ConcurrentDictionary<uint, WorldMap> WorldMaps { get; set; }
        internal ConcurrentDictionary<uint, WorldObject> Objects { get; set; }

        internal World()
        {
            Clients = new ConcurrentDictionary<Socket, Client>();
            Maps = new ConcurrentDictionary<ushort, Map>();
            WorldMaps = new ConcurrentDictionary<uint, WorldMap>();
            Objects = new ConcurrentDictionary<uint, WorldObject>();
        }

        internal void AddCreatureToMap(Creature c)
        {
            User user = c as User;
            //if it's a user
            if(user != null)
            {
                //get all visible non user objects in range of where this user is being put in at
                List<VisibleObject> itemMonster = Maps[user.Map.Id].Objects.OfType<VisibleObject>().Where(obj => obj.WithinRange(user) && !(obj is User)).ToList();
                List<User> users = Maps[user.Map.Id].Objects.OfType<User>().Where(obj => obj.WithinRange(user)).ToList();

                //send this user display packet for every item or monster in range
                user.Client.Enqueue(ServerPackets.DisplayItemMonster(itemMonster.Where(obj => !(obj is User)).ToArray()));

                //send this user display packet for each user in range, and vice verse
                foreach(User u in users)
                {
                    //displayuser packets to this client
                    //displayuser packet to each client
                }
            }
        }
    }
}
