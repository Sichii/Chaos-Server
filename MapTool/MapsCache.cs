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


using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Collections.Generic;
using System.IO;
using Chaos.Containers.WorldContainers;
using Chaos.Objects.Data;
using Chaos.Server;
using Chaos.Structs;

namespace ChaosTool
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    internal class MapsCache
    {
        internal MainForm MainForm { get; }
        internal Dictionary<uint, WorldMap> WorldMaps { get; set; }
        internal Dictionary<ushort, Map> Maps { get; set; }
        private string MapKey => Crypto.GetMD5Hash("Maps") + Crypto.GetMD5Hash("ServerObjSuffix");
        internal NewtonsoftSerializer Serializer { get; }
        internal StackExchangeRedisCacheClient Cache { get; }
        internal ConnectionMultiplexer DataConnection { get; }

        internal MapsCache(MainForm mainForm)
        {
            //create the serializing cache db
            var jSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            Serializer = new NewtonsoftSerializer(jSettings);
            DataConnection = ConnectionMultiplexer.Connect("localhost:6379");
            Cache = new StackExchangeRedisCacheClient(DataConnection, Serializer);

            //sets
            Maps = new Dictionary<ushort, Map>();
            WorldMaps = new Dictionary<uint, WorldMap>();
            MainForm = mainForm;

            if (!Cache.Exists(MapKey))
                Cache.Add(MapKey, new byte[100]);
        }
        internal void Load()
        {
            using (var reader = new BinaryReader(new MemoryStream(Cache.Get<byte[]>(MapKey))))
            {
                reader.ReadInt32();

                //load worldmaps
                var worldMapCount = reader.ReadUInt16();
                for (var wMap = 0; wMap < worldMapCount; ++wMap)
                {
                    var field = reader.ReadString();

                    var nodeCount = reader.ReadByte();
                    var nodes = new WorldMapNode[nodeCount];
                    for (var i = 0; i < nodeCount; i++)
                    {
                        var x = reader.ReadUInt16();
                        var y = reader.ReadUInt16();
                        var name = reader.ReadString();
                        var mapId = reader.ReadUInt16();
                        var dX = reader.ReadByte();
                        var dY = reader.ReadByte();
                        nodes[i] = new WorldMapNode((x, y), name, mapId, (dX, dY));
                    }

                    var worldMap = new WorldMap(field, nodes);
                    var crc32 = worldMap.CheckSum;
                    WorldMaps[crc32] = worldMap;
                }

                var mapCount = reader.ReadUInt16();
                for (var map = 0; map < mapCount; map++)
                {
                    //load map information
                    var mapId = reader.ReadUInt16();
                    var sizeX = reader.ReadByte();
                    var sizeY = reader.ReadByte();
                    var name = reader.ReadString();
                    var flags = (MapFlags)reader.ReadUInt32();
                    var music = reader.ReadSByte();
                    var newMap = new Map(mapId, sizeX, sizeY, flags, name, music);

                    //unused index byte for future use
                    reader.ReadByte();

                    //load warps
                    var warpCount = reader.ReadInt16();
                    for (var i = 0; i < warpCount; i++)
                    {
                        var sourceX = reader.ReadByte();
                        var sourceY = reader.ReadByte();
                        var targetMapId = reader.ReadUInt16();
                        var targetX = reader.ReadByte();
                        var targetY = reader.ReadByte();
                        var warp = new Warp(mapId, sourceX, sourceY, targetMapId, targetX, targetY);
                        newMap.Warps[(sourceX, sourceY)] = warp;
                    }

                    //load worldmaps for this map
                    var wMapCount = reader.ReadByte();
                    for (var i = 0; i < wMapCount; i++)
                    {
                        var x = reader.ReadByte();
                        var y = reader.ReadByte();
                        var CRC = reader.ReadUInt32();
                        if (WorldMaps.ContainsKey(CRC))
                            newMap.WorldMaps[(x, y)] = WorldMaps[CRC];
                    }

                    //add the map to the map list
                    Maps.Add(mapId, newMap);
                }
            }
        }


        internal void Save()
        {
            var cacheStream = new MemoryStream();
            using (var writer = new BinaryWriter(cacheStream))
            {
                writer.Write(1);

                //write world maps
                writer.Write((ushort)WorldMaps.Count);
                foreach (var worldMap in WorldMaps.Values)
                {
                    writer.Write(worldMap.Field);
                    writer.Write((byte)worldMap.Nodes.Count);
                    foreach (var worldMapNode in worldMap.Nodes)
                    {
                        writer.Write(worldMapNode.Position.X);
                        writer.Write(worldMapNode.Position.Y);
                        writer.Write(worldMapNode.Name);
                        writer.Write(worldMapNode.MapId);
                        writer.Write((byte)worldMapNode.Point.X);
                        writer.Write((byte)worldMapNode.Point.Y);
                    }
                }

                //write maps
                writer.Write((ushort)Maps.Count);
                foreach (var map in Maps.Values)
                {
                    //write map info
                    writer.Write(map.Id);
                    writer.Write(map.SizeX);
                    writer.Write(map.SizeY);
                    writer.Write(map.Name);
                    writer.Write((uint)map.Flags);
                    writer.Write(map.Music);

                    //unused index for future use
                    writer.Write((byte)0);

                    //write warps
                    writer.Write((ushort)map.Warps.Count);
                    foreach (var warp in map.Warps.Values)
                    {
                        writer.Write((byte)warp.Point.X);
                        writer.Write((byte)warp.Point.Y);
                        writer.Write(warp.TargetLocation.MapID);
                        writer.Write((byte)warp.TargetPoint.X);
                        writer.Write((byte)warp.TargetPoint.Y);
                    }

                    //write worldmaps for this map
                    writer.Write((byte)map.WorldMaps.Count);
                    foreach (var keyValuePair in map.WorldMaps)
                    {
                        writer.Write((byte)keyValuePair.Key.X);
                        writer.Write((byte)keyValuePair.Key.Y);
                        writer.Write(keyValuePair.Value.CheckSum);
                    }
                }
                Cache.Replace(MapKey, cacheStream.ToArray());
            }
        }
    }
}