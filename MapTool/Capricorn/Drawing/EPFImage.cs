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
        public EPFFrame this[int index]
        {
            get => Frames[index];
            set => Frames[index] = value;
        }

        public EPFFrame[] Frames { get; private set; }

        public long TOCAddress { get; private set; }

        public int Unknown { get; private set; }

        public int Height { get; private set; }

        public byte[] RawData { get; private set; }

        public int Width { get; private set; }

        public int ExpectedFrames { get; private set; }

        public static EPFImage FromFile(string file) => EPFImage.LoadEPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));

        public static EPFImage FromRawData(byte[] data) => EPFImage.LoadEPF(new MemoryStream(data));

        public static EPFImage FromArchive(string file, DATArchive archive) => !archive.Contains(file) ? null : FromRawData(archive.ExtractFile(file));

        public static EPFImage FromArchive(string file, bool ignoreCase, DATArchive archive) => !archive.Contains(file, ignoreCase) ? null : FromRawData(archive.ExtractFile(file, ignoreCase));

        private static EPFImage LoadEPF(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            var binaryReader = new BinaryReader(stream);
            var epfImage = new EPFImage
            {
                ExpectedFrames = binaryReader.ReadUInt16(),
                Width = binaryReader.ReadUInt16(),
                Height = binaryReader.ReadUInt16(),
                Unknown = binaryReader.ReadUInt16(),
                TOCAddress = binaryReader.ReadUInt32() + 12U
            };
            if (epfImage.ExpectedFrames <= 0)
                return epfImage;
            epfImage.Frames = new EPFFrame[epfImage.ExpectedFrames];
            for (int index = 0; index < epfImage.ExpectedFrames; ++index)
            {
                binaryReader.BaseStream.Seek(epfImage.TOCAddress + (index * 16), SeekOrigin.Begin);
                int left = binaryReader.ReadUInt16();
                int top = binaryReader.ReadUInt16();
                int num1 = binaryReader.ReadUInt16();
                int num2 = binaryReader.ReadUInt16();
                int width = num1 - left;
                int height = num2 - top;
                uint num3 = binaryReader.ReadUInt32() + 12U;
                uint num4 = binaryReader.ReadUInt32() + 12U;
                binaryReader.BaseStream.Seek(num3, SeekOrigin.Begin);
                epfImage.RawData = num4 - num3 == width * height ? binaryReader.ReadBytes((int)num4 - (int)num3) : binaryReader.ReadBytes((int)(epfImage.TOCAddress - num3));
                epfImage.Frames[index] = new EPFFrame(left, top, width, height, epfImage.RawData);
            }
            return epfImage;
        }

        public override string ToString() => "{Frames = " + ExpectedFrames.ToString() + ", Width = " + Width.ToString() + ", Height = " + Height.ToString() + ", TOC Address = 0x" + TOCAddress.ToString("X").PadLeft(8, '0') + "}";
    }
}
