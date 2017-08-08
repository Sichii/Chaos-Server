using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Chaos;
using Newtonsoft.Json;
using System.Linq;

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class Map
    {
        internal readonly object Sync = new object();
        internal ConcurrentDictionary<int, WorldObject> Objects { get; set; }
        internal ConcurrentDictionary<Point, Door> Doors { get; set; }
        [JsonProperty]
        internal ushort Id { get; }
        internal byte SizeX { get; }
        internal byte SizeY { get; }
        internal byte[] Data { get; private set; }
        internal ushort CheckSum => Crypto.Generate16(Data);
        internal Dictionary<Point, Tile> Tiles { get; }
        internal MapFlags Flags { get; set; }
        internal string Name { get; set; }
        internal sbyte Music { get; set; }
        internal Dictionary<Point, Warp> Exits { get; set; }
        internal Dictionary<Point, WorldMap> WorldMaps { get; set; }

        internal Map(ushort id, byte sizeX, byte sizeY, MapFlags flags, string name, sbyte music)
        {
            Id = id;
            SizeX = sizeX;
            SizeY = sizeY;
            Flags = flags;
            Name = name;
            Tiles = new Dictionary<Point, Tile>();
            Exits = new Dictionary<Point, Warp>();
            WorldMaps = new Dictionary<Point, WorldMap>();
            Objects = new ConcurrentDictionary<int, WorldObject>();
            Doors = new ConcurrentDictionary<Point, Door>();
        }

        [JsonConstructor]
        internal Map(ushort id)
        {
            Id = id;
        }

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
        }

        internal bool HasFlag(MapFlags flag) => Flags.HasFlag(flag);
        internal bool IsWall(ushort x, ushort y) => x < 0 || y < 0 || x >= SizeX || y >= SizeY || Tiles[new Point(x, y)].IsWall;
        internal bool IsWall(Point p) => IsWall(p.X, p.Y);
        internal bool IsWalkable(Point p) => !IsWall(p) && !Doors.Keys.Contains(p) && !Objects.Values.OfType<Creature>().Any(creature => creature.Type != CreatureType.WalkThrough && creature.Point == p);
    }
}
