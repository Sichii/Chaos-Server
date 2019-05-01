// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.HPFImage
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

using Capricorn.IO;
using Capricorn.IO.Compression;
using System;
using System.IO;

namespace Capricorn.Drawing
{
    public class HPFImage
    {
        private int width;
        private int height;
        private byte[] rawData;
        private byte[] headerData;

        public byte[] HeaderData
        {
            get
            {
                return headerData;
            }
        }

        public byte[] RawData
        {
            get
            {
                return rawData;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public static HPFImage FromFile(string file)
        {
            return HPFImage.LoadHPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public static HPFImage FromRawData(byte[] data)
        {
            return HPFImage.LoadHPF(new MemoryStream(data));
        }

        public static HPFImage FromArchive(string file, DATArchive archive)
        {
            if (!archive.Contains(file))
                return null;
            return HPFImage.FromRawData(archive.ExtractFile(file));
        }

        public static HPFImage FromArchive(string file, bool ignoreCase, DATArchive archive)
        {
            if (!archive.Contains(file, ignoreCase))
                return null;
            return HPFImage.FromRawData(archive.ExtractFile(file, ignoreCase));
        }

        private static HPFImage LoadHPF(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            BinaryReader binaryReader1 = new BinaryReader(stream);
            uint num = binaryReader1.ReadUInt32();
            binaryReader1.BaseStream.Seek(-4L, SeekOrigin.Current);
            if ((int)num != -16602539)
                throw new ArgumentException("Invalid file format, does not contain HPF signature bytes.");
            HPFImage hpfImage = new HPFImage();
            byte[] buffer = HPFCompression.Decompress(binaryReader1.ReadBytes((int)binaryReader1.BaseStream.Length));
            binaryReader1.Close();
            BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(buffer));
            hpfImage.headerData = binaryReader2.ReadBytes(8);
            hpfImage.rawData = binaryReader2.ReadBytes(buffer.Length - 8);
            binaryReader2.Close();
            hpfImage.width = 28;
            if (hpfImage.rawData.Length % hpfImage.width != 0)
                throw new ArgumentException("HPF file does not use the standard 28 pixel width.");
            hpfImage.height = hpfImage.rawData.Length / hpfImage.width;
            return hpfImage;
        }

        public override string ToString()
        {
            return "{Width = " + width.ToString() + ", Height = " + height.ToString() + "}";
        }
    }
}
