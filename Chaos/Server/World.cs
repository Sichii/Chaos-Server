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

namespace Chaos
{
    internal class World
    {
        internal Server Server { get; }
        internal Dictionary<ushort, Map> Maps { get; set; }
        internal Dictionary<uint, WorldMap> WorldMaps { get; set; }
        internal ConcurrentDictionary<string, Guild> Guilds { get; set; }
        internal ConcurrentDictionary<int, Group> Groups { get; set; }
        internal ConcurrentDictionary<int, Exchange> Exchanges { get; set; }

        internal World(Server server)
        {
            Server = server;
            Maps = new Dictionary<ushort, Map>();
            WorldMaps = new Dictionary<uint, WorldMap>();
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
                        newMap.AddEffect(new Effect(4500, TimeSpan.Zero, false, new Animation(warp.Point, 96, 250)));
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
                    Maps.Add(mapId, newMap);
                    newMap.LoadData($@"{Paths.MapFiles}lod{newMap.Id}.map");
                }
            }
            #endregion
        }

        internal void Populate()
        {
            #region Load Merchants
            foreach (Merchant merchant in Game.Merchants)
                merchant.Map.AddObject(merchant, merchant.Point);
            #endregion
        }

        /// <summary>
        /// Cleans up active world info of the user.
        /// </summary>
        /// <param name="user">The user to clean up</param>
        internal void RemoveClient(Client client)
        {
            User user = client.User;

            user.Group?.TryRemove(user.Id);
            user.Exchange?.Cancel(user);
            user.Save();
            user.Map.RemoveObject(user);

            //remove from other things
        }
    }
}
