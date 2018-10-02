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
        public MapTile this[int x, int y]
        {
            get => Tiles[(y * Width) + x];
            set => Tiles[(y * Width) + x] = value;
        }

        public string Name { get; set; }

        public int ID { get; set; }

        public MapTile[] Tiles { get; private set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public static MAPFile FromFile(string file)
        {
            var mapFile = MAPFile.LoadMap(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
            mapFile.ID = Convert.ToInt32(Path.GetFileNameWithoutExtension(file).Remove(0, 3));
            return mapFile;
        }

        public static MAPFile FromFile(string file, int width, int height)
        {
            var mapFile = MAPFile.LoadMap(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
            mapFile.Width = width;
            mapFile.Height = height;
            mapFile.ID = Convert.ToInt32(Path.GetFileNameWithoutExtension(file).Remove(0, 3));
            return mapFile;
        }

        public static MAPFile FromRawData(byte[] data) => MAPFile.LoadMap(new MemoryStream(data));

        public static MAPFile FromRawData(byte[] data, int width, int height)
        {
            var mapFile = MAPFile.LoadMap(new MemoryStream(data));
            mapFile.Width = width;
            mapFile.Height = height;
            return mapFile;
        }

        private static MAPFile LoadMap(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            var binaryReader = new BinaryReader(stream);
            int length = (int)(binaryReader.BaseStream.Length / 6L);
            var mapFile = new MAPFile
            {
                Tiles = new MapTile[length]
            };
            for (int index = 0; index < length; ++index)
            {
                ushort floor = binaryReader.ReadUInt16();
                ushort leftWall = binaryReader.ReadUInt16();
                ushort rightWall = binaryReader.ReadUInt16();
                mapFile.Tiles[index] = new MapTile(floor, leftWall, rightWall);
            }
            binaryReader.Close();
            return mapFile;
        }

        public override string ToString() => "{Name = " + Name + ", ID = " + ID.ToString() + ", Width = " + Width.ToString() + ", Height = " + Height.ToString() + "}";
    }
}
