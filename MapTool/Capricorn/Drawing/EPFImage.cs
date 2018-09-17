// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.EPFImage
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

using Capricorn.IO;
using System.IO;

namespace Capricorn.Drawing
{
    public class EPFImage
    {
        private int expectedFrames;
        private int width;
        private int height;
        private int unknown;
        private long tocAddress;
        private EPFFrame[] frames;
        private byte[] rawData;

        public EPFFrame this[int index]
        {
            get => frames[index];
            set => frames[index] = value;
        }

        public EPFFrame[] Frames => frames;

        public long TOCAddress => tocAddress;

        public int Unknown => unknown;

        public int Height => height;

        public byte[] RawData => rawData;

        public int Width => width;

        public int ExpectedFrames => expectedFrames;

        public static EPFImage FromFile(string file) => EPFImage.LoadEPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));

        public static EPFImage FromRawData(byte[] data) => EPFImage.LoadEPF(new MemoryStream(data));

        public static EPFImage FromArchive(string file, DATArchive archive)
        {
            if (!archive.Contains(file))
                return null;
            return FromRawData(archive.ExtractFile(file));
        }

        public static EPFImage FromArchive(string file, bool ignoreCase, DATArchive archive)
        {
            if (!archive.Contains(file, ignoreCase))
                return null;
            return FromRawData(archive.ExtractFile(file, ignoreCase));
        }

        private static EPFImage LoadEPF(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            var binaryReader = new BinaryReader(stream);
            var epfImage = new EPFImage
            {
                expectedFrames = binaryReader.ReadUInt16(),
                width = binaryReader.ReadUInt16(),
                height = binaryReader.ReadUInt16(),
                unknown = binaryReader.ReadUInt16(),
                tocAddress = binaryReader.ReadUInt32() + 12U
            };
            if (epfImage.expectedFrames <= 0)
                return epfImage;
            epfImage.frames = new EPFFrame[epfImage.expectedFrames];
            for (int index = 0; index < epfImage.expectedFrames; ++index)
            {
                binaryReader.BaseStream.Seek(epfImage.tocAddress + index * 16, SeekOrigin.Begin);
                int left = binaryReader.ReadUInt16();
                int top = binaryReader.ReadUInt16();
                int num1 = binaryReader.ReadUInt16();
                int num2 = binaryReader.ReadUInt16();
                int width = num1 - left;
                int height = num2 - top;
                uint num3 = binaryReader.ReadUInt32() + 12U;
                uint num4 = binaryReader.ReadUInt32() + 12U;
                binaryReader.BaseStream.Seek(num3, SeekOrigin.Begin);
                epfImage.rawData = num4 - num3 == width * height ? binaryReader.ReadBytes((int)num4 - (int)num3) : binaryReader.ReadBytes((int)(epfImage.tocAddress - num3));
                epfImage.frames[index] = new EPFFrame(left, top, width, height, epfImage.rawData);
            }
            return epfImage;
        }

        public override string ToString() => "{Frames = " + expectedFrames.ToString() + ", Width = " + width.ToString() + ", Height = " + height.ToString() + ", TOC Address = 0x" + tocAddress.ToString("X").PadLeft(8, '0') + "}";
    }
}
