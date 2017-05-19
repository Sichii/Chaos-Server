using System.Collections.Generic;
using System.IO;

namespace Insert_Creative_Name.Objects
{
    internal sealed class Map
    {
        internal ushort Id { get; }
        internal byte SizeX { get; }
        internal byte SizeY { get; }
        internal byte Flags { get; set; }
        internal ushort CRC { get; }
        internal string Name { get; set; }
        internal Dictionary<Point, Tile> Tiles { get; }
        internal bool CanUseSkills { get; set; }
        internal bool CanUseSpells { get; set; }
        private bool IsLoaded { get; set; }
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
            for (short num = 0; num < sizeX; num += 1)
                for (short num2 = 0; num2 < sizeY; num2 += 1)
                    Tiles[new Point(num, num2)] = new Tile(0, 0, 0);
        }
        internal void SetData(byte[] data)
        {
            Tiles.Clear();
            int num = 0;
            for (short num2 = 0; num2 < SizeY; num2 += 1)
                for (short num3 = 0; num3 < SizeX; num3 += 1)
                {
                    Point key = new Point(num3, num2);
                    short background = (short)(data[num++] | data[num++] << 8);
                    short leftForeground = (short)(data[num++] | data[num++] << 8);
                    short rightForeground = (short)(data[num++] | data[num++] << 8);
                    Tiles[key] = new Tile(background, leftForeground, rightForeground);
                }

            IsLoaded = true;
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

            Point key = new Point(x, y);
            return Tiles[key].IsWall;
        }
    }
}
