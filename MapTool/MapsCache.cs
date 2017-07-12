using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapTool
{
    internal class MapsCache
    {
        internal MainForm MainForm { get; }
        internal Dictionary<uint, WorldMap> WorldMaps { get; set; }
        internal Dictionary<ushort, Map> Maps { get; set; }
        private string HashKey => "edl396yhvnw85b6kd8vnsj296hj285bq";
        internal NewtonsoftSerializer Serializer { get; }
        internal StackExchangeRedisCacheClient Cache { get; }
        internal ConnectionMultiplexer DataConnection { get; }

        internal MapsCache(MainForm mainForm)
        {
            //create the serializing cache db
            JsonSerializerSettings jSettings = new JsonSerializerSettings();
            jSettings.TypeNameHandling = TypeNameHandling.All;
            Serializer = new NewtonsoftSerializer(jSettings);
            ConfigurationOptions config = new ConfigurationOptions();
            DataConnection = ConnectionMultiplexer.Connect("localhost:6379");
            Cache = new StackExchangeRedisCacheClient(DataConnection, Serializer);

            //sets
            Maps = new Dictionary<ushort, Map>();
            WorldMaps = new Dictionary<uint, WorldMap>();
            MainForm = mainForm;

            if (!Cache.Exists(HashKey))
                Cache.Add(HashKey, new byte[1]);
        }
        internal void Load()
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(Cache.Get<byte[]>(HashKey))))
            {
                reader.ReadInt32();

                //load worldmaps
                ushort worldMapCount = reader.ReadUInt16();
                for (int wMap = 0; wMap < worldMapCount; ++wMap)
                {
                    WorldMap worldMap = new WorldMap(reader.ReadString(), new WorldMapNode[0]);

                    byte nodeCount = reader.ReadByte();
                    for (int i = 0; i < nodeCount; i++)
                    {
                        short x = reader.ReadInt16();
                        short y = reader.ReadInt16();
                        string name = reader.ReadString();
                        ushort mapId = reader.ReadUInt16();
                        byte dX = reader.ReadByte();
                        byte dY = reader.ReadByte();
                        worldMap.Nodes.Add(new WorldMapNode(new Point(x, y), name, mapId, new Point(dX, dY)));
                    }

                    uint crc32 = worldMap.GetCrc32();
                    WorldMaps[crc32] = worldMap;
                }

                short mapCount = reader.ReadInt16();
                for (int map = 0; map < mapCount; map++)
                {
                    //load map information
                    ushort mapId = reader.ReadUInt16();
                    byte sizeX = reader.ReadByte();
                    byte sizeY = reader.ReadByte();
                    string name = reader.ReadString();
                    MapFlags flags = (MapFlags)reader.ReadByte();
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
                    short warpCount = reader.ReadInt16();
                    for (int i = 0; i < warpCount; i++)
                    {
                        byte sourceX = reader.ReadByte();
                        byte sourceY = reader.ReadByte();
                        ushort targetMapId = reader.ReadUInt16();
                        byte targetX = reader.ReadByte();
                        byte targetY = reader.ReadByte();
                        Warp warp = new Warp(sourceX, sourceY, targetX, targetY, mapId, targetMapId);
                        newMap.Exits[new Point(sourceX, sourceY)] = warp;
                    }

                    //load worldmaps for this map
                    byte wMapCount = reader.ReadByte();
                    for (int i = 0; i < wMapCount; i++)
                    {
                        byte x = reader.ReadByte();
                        byte y = reader.ReadByte();
                        uint CRC = reader.ReadUInt32();
                        if (WorldMaps.ContainsKey(CRC))
                            newMap.WorldMaps[new Point(x, y)] = WorldMaps[CRC];
                    }

                    //add the map to the map list
                    Maps.Add(mapId, newMap);
                }
            }
        }


        internal void Save()
        {
            MemoryStream cacheStream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(cacheStream))
            {
                writer.Write(1);

                //write world maps
                writer.Write((short)WorldMaps.Count);
                foreach (WorldMap worldMap in WorldMaps.Values)
                {
                    writer.Write(worldMap.Field);
                    writer.Write((byte)worldMap.Nodes.Count);
                    foreach (WorldMapNode worldMapNode in worldMap.Nodes)
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
                writer.Write((short)Maps.Count);
                foreach (Map map in Maps.Values)
                {
                    //write map info
                    writer.Write(map.Id);
                    writer.Write(map.SizeX);
                    writer.Write(map.SizeY);
                    writer.Write(map.Name);
                    writer.Write((uint)map.Flags);
                    writer.Write(map.Music);

                    //write doors
                    writer.Write((byte)map.Doors.Count);
                    foreach (Door door in map.Doors.Values)
                    {
                        writer.Write(door.Point.X);
                        writer.Write(door.Point.Y);
                        writer.Write(door.OpenRight);
                    }

                    //write warps
                    writer.Write((short)map.Exits.Count);
                    foreach (Warp warp in map.Exits.Values)
                    {
                        writer.Write(warp.SourceX);
                        writer.Write(warp.SourceY);
                        writer.Write(warp.TargetMapId);
                        writer.Write(warp.TargetX);
                        writer.Write(warp.TargetY);
                    }

                    //write worldmaps for this map
                    writer.Write((byte)map.WorldMaps.Count);
                    foreach (KeyValuePair<Point, WorldMap> keyValuePair in map.WorldMaps)
                    {
                        writer.Write((byte)keyValuePair.Key.X);
                        writer.Write((byte)keyValuePair.Key.Y);
                        writer.Write(keyValuePair.Value.GetCrc32());
                    }
                }

                Cache.Replace(HashKey, cacheStream.ToArray());
            }
        }
    }
}
