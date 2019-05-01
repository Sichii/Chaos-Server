// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.MAPFile
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

using System;
using System.IO;

namespace Capricorn.Drawing
{
    public class MAPFile
    {
        private int width;
        private int height;
        private MapTile[] tiles;
        private int id;
        private string name;

        public MapTile this[int x, int y]
        {
            get
            {
                return tiles[y * width + x];
            }
            set
            {
                tiles[y * width + x] = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public MapTile[] Tiles
        {
            get
            {
                return tiles;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public static MAPFile FromFile(string file)
        {
            MAPFile mapFile = MAPFile.LoadMap(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
            mapFile.id = Convert.ToInt32(Path.GetFileNameWithoutExtension(file).Remove(0, 3));
            return mapFile;
        }

        public static MAPFile FromFile(string file, int width, int height)
        {
            MAPFile mapFile = MAPFile.LoadMap(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
            mapFile.width = width;
            mapFile.height = height;
            mapFile.id = Convert.ToInt32(Path.GetFileNameWithoutExtension(file).Remove(0, 3));
            return mapFile;
        }

        public static MAPFile FromRawData(byte[] data)
        {
            return MAPFile.LoadMap(new MemoryStream(data));
        }

        public static MAPFile FromRawData(byte[] data, int width, int height)
        {
            MAPFile mapFile = MAPFile.LoadMap(new MemoryStream(data));
            mapFile.width = width;
            mapFile.height = height;
            return mapFile;
        }

        private static MAPFile LoadMap(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            BinaryReader binaryReader = new BinaryReader(stream);
            int length = (int)(binaryReader.BaseStream.Length / 6L);
            MAPFile mapFile = new MAPFile();
            mapFile.tiles = new MapTile[length];
            for (int index = 0; index < length; ++index)
            {
                ushort floor = binaryReader.ReadUInt16();
                ushort leftWall = binaryReader.ReadUInt16();
                ushort rightWall = binaryReader.ReadUInt16();
                mapFile.tiles[index] = new MapTile(floor, leftWall, rightWall);
            }
            binaryReader.Close();
            return mapFile;
        }

        public override string ToString()
        {
            return "{Name = " + name + ", ID = " + id.ToString() + ", Width = " + width.ToString() + ", Height = " + height.ToString() + "}";
        }
    }
}
