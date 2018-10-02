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
        public byte[] HeaderData { get; private set; }

        public byte[] RawData { get; private set; }

        public int Height { get; private set; }

        public int Width { get; private set; }

        public static HPFImage FromFile(string file) => HPFImage.LoadHPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));

        public static HPFImage FromRawData(byte[] data) => HPFImage.LoadHPF(new MemoryStream(data));

        public static HPFImage FromArchive(string file, DATArchive archive) => !archive.Contains(file) ? null : HPFImage.FromRawData(archive.ExtractFile(file));

        public static HPFImage FromArchive(string file, bool ignoreCase, DATArchive archive) => !archive.Contains(file, ignoreCase) ? null : HPFImage.FromRawData(archive.ExtractFile(file, ignoreCase));

        private static HPFImage LoadHPF(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            var binaryReader1 = new BinaryReader(stream);
            uint num = binaryReader1.ReadUInt32();
            binaryReader1.BaseStream.Seek(-4L, SeekOrigin.Current);
            if ((int)num != -16602539)
                throw new ArgumentException("Invalid file format, does not contain HPF signature bytes.");
            var hpfImage = new HPFImage();
            byte[] buffer = HPFCompression.Decompress(binaryReader1.ReadBytes((int)binaryReader1.BaseStream.Length));
            binaryReader1.Close();
            var binaryReader2 = new BinaryReader(new MemoryStream(buffer));
            hpfImage.HeaderData = binaryReader2.ReadBytes(8);
            hpfImage.RawData = binaryReader2.ReadBytes(buffer.Length - 8);
            binaryReader2.Close();
            hpfImage.Width = 28;
            if (hpfImage.RawData.Length % hpfImage.Width != 0)
                throw new ArgumentException("HPF file does not use the standard 28 pixel width.");
            hpfImage.Height = hpfImage.RawData.Length / hpfImage.Width;
            return hpfImage;
        }

        public override string ToString() => "{Width = " + Width.ToString() + ", Height = " + Height.ToString() + "}";
    }
}
