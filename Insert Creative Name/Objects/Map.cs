using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Chaos.Objects
{
    [Serializable]
    internal sealed class Map
    {
        internal ConcurrentDictionary<uint, WorldObject> Objects { get; set; }
        internal ushort Id { get; }
        internal byte SizeX { get; }
        internal byte SizeY { get; }
        internal byte[] Data { get; }
        internal ushort CRC { get; }
        internal Dictionary<Point, Tile> Tiles { get; }
        internal byte Flags { get; set; }
        internal string Name { get; set; }
        internal bool CanUseSkills { get; set; }
        internal bool CanUseSpells { get; set; }
        internal Tile this[short x, short y] => Tiles[new Point(x, y)];
        internal Tile this[Point point] => Tiles[point];


        internal Map(ushort number, byte sizeX, byte sizeY, byte flags, ushort crc, string name)
        {
            Id = number;
            SizeX = sizeX;
            SizeY = sizeY;
            Flags = flags;
            CRC = crc;
            Name = name;
            Tiles = new Dictionary<Point, Tile>();
            CanUseSkills = true;
            CanUseSpells = true;
            Objects = new ConcurrentDictionary<uint, WorldObject>();
            for (short x = 0; x < sizeX; x++)
                for (short y = 0; y < sizeY; y++)
                    Tiles[new Point(x, y)] = new Tile(Id, x, y, 0, 0, 0);
        }
        internal void SetData(byte[] data)
        {
            Tiles.Clear();
            int num = 0;
            for (short y = 0; y < SizeY; y++)
                for (short x = 0; x < SizeX; x++)
                {
                    Point key = new Point(x, y);
                    short background = (short)(data[num++] | data[num++] << 8);
                    short leftForeground = (short)(data[num++] | data[num++] << 8);
                    short rightForeground = (short)(data[num++] | data[num++] << 8);
                    Tiles[key] = new Tile(Id, x, y, background, leftForeground, rightForeground);
                }
        }
        internal void SetData(string path)
        {
            if (File.Exists(path))
                SetData(File.ReadAllBytes(path));
        }

        internal bool IsWall(short x, short y)
        {
            if (x < 0 || y < 0 || x >= SizeX || y >= SizeY)
                return true;

            return this[x, y].IsWall;
        }
    }
}
