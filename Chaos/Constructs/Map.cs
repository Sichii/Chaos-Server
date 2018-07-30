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

namespace Chaos
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Map
    {
        internal readonly object Sync = new object();
        internal ConcurrentDictionary<int, WorldObject> Objects { get; set; }
        public ConcurrentDictionary<Point, Door> Doors { get; set; }
        [JsonProperty]
        public ushort Id { get; }
        public byte SizeX { get; set; }
        public byte SizeY { get; set; }
        internal byte[] Data { get; private set; }
        internal ushort CheckSum { get; private set; }
        internal Dictionary<Point, Tile> Tiles { get; }
        public MapFlags Flags { get; set; }
        public string Name { get; set; }
        public sbyte Music { get; set; }
        public Dictionary<Point, Warp> Warps { get; set; }
        public Dictionary<Point, WorldMap> WorldMaps { get; set; }

        /// <summary>
        /// Object representing a map.
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
            Objects = new ConcurrentDictionary<int, WorldObject>();
            Doors = new ConcurrentDictionary<Point, Door>();
        }

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
        /// Checks if the map has a certain flag.
        /// </summary>
        internal bool HasFlag(MapFlags flag) => Flags.HasFlag(flag);

        /// <summary>
        /// Checks if a set of co-ordinates is inside the map, and a wall.
        /// </summary>
        internal bool IsWall(ushort x, ushort y) => x < 0 || y < 0 || x >= SizeX || y >= SizeY || Tiles[new Point(x, y)].IsWall;

        /// <summary>
        /// Checks if a given point is inside the map, and a wall.
        /// </summary>
        internal bool IsWall(Point p) => IsWall(p.X, p.Y);

        /// <summary>
        /// Checks if a given point is a wall, or has a monster, door, or other object already on it.
        /// </summary>
        internal bool IsWalkable(Point p)
        {
            lock (Sync)
                return !IsWall(p) && (Doors.Keys.Contains(p) ? Doors[p].Opened : true) && !Objects.Values.OfType<Creature>().Any(creature => creature.Type != CreatureType.WalkThrough && creature.Point == p);
        }
    }
}
