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
using System;
using System.IO;
using System.Threading.Tasks;

namespace Chaos
{
    /// <summary>
    /// Represents the game world. Contains server-constructs of game-world objects.
    /// </summary>
    internal sealed class World
    {
        private readonly Server Server;

        internal Dictionary<ushort, Map> Maps { get; }
        internal Dictionary<uint, WorldMap> WorldMaps { get; }
        internal ConcurrentDictionary<string, Guild> Guilds { get; }
        internal ConcurrentDictionary<int, Group> Groups { get; }
        internal ConcurrentDictionary<int, Exchange> Exchanges { get; }

        internal World(Server server, out Task Initializer)
        {
            Server = server;
            Maps = new Dictionary<ushort, Map>();
            WorldMaps = new Dictionary<uint, WorldMap>();
            Guilds = new ConcurrentDictionary<string, Guild>(StringComparer.CurrentCultureIgnoreCase);
            Groups = new ConcurrentDictionary<int, Group>();
            Exchanges = new ConcurrentDictionary<int, Exchange>();

            Initializer = Task.Run(Initialize);
        }

        /// <summary>
        /// Populates the world with maps, worldmaps, warps, and more.
        /// </summary>
        internal void Initialize()
        {
            Server.WriteLogAsync("Initializing world...");

            #region Load Maps
            using (var reader = new BinaryReader(new MemoryStream(Server.DataBase.MapData)))
            {
                reader.ReadInt32();

                //load worldmaps
                ushort worldMapCount = reader.ReadUInt16();
                for (int wMap = 0; wMap < worldMapCount; ++wMap)
                {
                    string field = reader.ReadString();
                    byte nodeCount = reader.ReadByte();
                    var nodes = new WorldMapNode[nodeCount];
                    for (int i = 0; i < nodeCount; i++)
                    {
                        ushort x = reader.ReadUInt16();
                        ushort y = reader.ReadUInt16();
                        string name = reader.ReadString();
                        ushort mapId = reader.ReadUInt16();
                        byte dX = reader.ReadByte();
                        byte dY = reader.ReadByte();
                        nodes[i] = new WorldMapNode((x, y), name, mapId, (dX, dY));
                    }

                    var worldMap = new WorldMap(field, nodes);

                    uint checkSum = worldMap.CheckSum;
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
                    var flags = (MapFlags)reader.ReadUInt32();
                    sbyte music = reader.ReadSByte();
                    var newMap = new Map(mapId, sizeX, sizeY, flags, name, music, reader);

                    //add the map to the map list
                    Maps.Add(mapId, newMap);
                }
            }

            Guilds.TryAdd(CONSTANTS.DEVELOPER_GUILD_NAME, new Guild());

            using (var reader = new BinaryReader(new MemoryStream(Server.DataBase.GuildData)))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    string name;
                    var ranks = new List<string>();
                    var members = new Dictionary<string, string>();

                    name = reader.ReadString();
                    for (int x = 0; x < reader.ReadInt32(); x++)
                        ranks.Add(reader.ReadString());
                    for (int x = 0; x < reader.ReadInt32(); x++)
                        members.Add(reader.ReadString(), reader.ReadString());

                    Guilds.TryAdd(name, new Guild(name, members, ranks));
                }

                if (!Guilds.ContainsKey(CONSTANTS.DEVELOPER_GUILD_NAME))
                    Guilds.TryAdd(CONSTANTS.DEVELOPER_GUILD_NAME, new Guild());
            }
            #endregion

            #region Populate Maps
            foreach (Merchant merchant in Game.Merchants.GetMerchants)
                merchant.Map.AddObject(merchant, merchant.Point);
            #endregion
        }

        /// <summary>
        /// Saves all guilds to the database.
        /// </summary>
        internal void Save()
        {
            Server.WriteLogAsync("Saving world...");

            var buffer = new MemoryStream();
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(Guilds.Count - 1);
                foreach (Guild guild in Guilds.Values)
                    if (guild.Name != CONSTANTS.DEVELOPER_GUILD_NAME)
                        guild.Save(writer);

                Server.DataBase.TrySaveGuilds(buffer.ToArray());
            }
        }

        /// <summary>
        /// Cleans up active world info of the user.
        /// </summary>
        /// <param name="user">The user to clean up</param>
        internal void RemoveClient(Client client)
        {
            User user = client.User;

            user.Group?.TryRemove(user.ID);
            user.Exchange?.Cancel(user);
            user.Save();
            user.Map.RemoveObject(user, true);

            //remove from other things
        }
    }
}
