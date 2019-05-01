// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.MPFImage
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

using Capricorn.IO;
using System.IO;

namespace Capricorn.Drawing
{
    public class MPFImage
    {
        private int expectedFrames;
        private int width;
        private int height;
        private MPFFrame[] frames;
        private uint expectedDataSize;
        public byte walkStart;
        public byte walkLength;
        private uint ffUnknown;
        private bool isNewFormat;
        private bool isFFFormat;
        public byte attack1Start;
        public byte attack1Length;
        public byte idleStart;
        public byte idleLength;
        public ushort idleSpeed;
        public byte attack2Start;
        public byte attack2Length;
        public byte attack3Start;
        public byte attack3Length;
        public string palette;

        public MPFFrame this[int index]
        {
            get
            {
                return frames[index];
            }
            set
            {
                frames[index] = value;
            }
        }

        public bool IsFFFormat
        {
            get
            {
                return isFFFormat;
            }
        }

        public bool IsNewFormat
        {
            get
            {
                return isNewFormat;
            }
        }

        public uint FFUnknown
        {
            get
            {
                return ffUnknown;
            }
        }

        public uint ExpectedDataSize
        {
            get
            {
                return expectedDataSize;
            }
        }

        public MPFFrame[] Frames
        {
            get
            {
                return frames;
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

        public int ExpectedFrames
        {
            get
            {
                return expectedFrames;
            }
        }

        public override string ToString()
        {
            return string.Format("{{Frames = {0}, Width = {1}, Height = {2}, WalkStart = {3}, WalkLength = {4}, Attack1Start = {5}, Attack1Length = {6}, IdleStart = {7}, IdleLength = {8}}}", expectedFrames, width, height, walkStart, walkLength, attack1Start, attack1Length, idleStart, idleLength);
        }

        public static MPFImage FromFile(string file)
        {
            return MPFImage.LoadMPF(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public static MPFImage FromRawData(byte[] data)
        {
            return MPFImage.LoadMPF(new MemoryStream(data));
        }

        public static MPFImage FromArchive(string file, DATArchive archive)
        {
            if (!archive.Contains(file))
                return null;
            return MPFImage.FromRawData(archive.ExtractFile(file));
        }

        public static MPFImage FromArchive(string file, bool ignoreCase, DATArchive archive)
        {
            if (!archive.Contains(file, ignoreCase))
                return null;
            return MPFImage.FromRawData(archive.ExtractFile(file, ignoreCase));
        }

        private static MPFImage LoadMPF(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            BinaryReader binaryReader = new BinaryReader(stream);
            MPFImage mpfImage = new MPFImage();
            if ((int)binaryReader.ReadUInt32() == -1)
            {
                mpfImage.isFFFormat = true;
                mpfImage.ffUnknown = binaryReader.ReadUInt32();
            }
            else
                binaryReader.BaseStream.Seek(-4L, SeekOrigin.Current);
            mpfImage.expectedFrames = binaryReader.ReadByte();
            mpfImage.frames = new MPFFrame[mpfImage.expectedFrames];
            mpfImage.width = binaryReader.ReadUInt16();
            mpfImage.height = binaryReader.ReadUInt16();
            mpfImage.expectedDataSize = binaryReader.ReadUInt32();
            mpfImage.walkStart = binaryReader.ReadByte();
            mpfImage.walkLength = binaryReader.ReadByte();
            mpfImage.isNewFormat = binaryReader.ReadUInt16() == ushort.MaxValue;
            if (mpfImage.isNewFormat)
            {
                mpfImage.idleStart = binaryReader.ReadByte();
                mpfImage.idleLength = binaryReader.ReadByte();
                mpfImage.idleSpeed = binaryReader.ReadUInt16();
                mpfImage.attack1Start = binaryReader.ReadByte();
                mpfImage.attack1Length = binaryReader.ReadByte();
                mpfImage.attack2Start = binaryReader.ReadByte();
                mpfImage.attack2Length = binaryReader.ReadByte();
                mpfImage.attack3Start = binaryReader.ReadByte();
                mpfImage.attack3Length = binaryReader.ReadByte();
            }
            else
            {
                binaryReader.BaseStream.Seek(-2L, SeekOrigin.Current);
                mpfImage.attack1Start = binaryReader.ReadByte();
                mpfImage.attack1Length = binaryReader.ReadByte();
                mpfImage.idleStart = binaryReader.ReadByte();
                mpfImage.idleLength = binaryReader.ReadByte();
                mpfImage.idleSpeed = binaryReader.ReadUInt16();
            }
            long num1 = binaryReader.BaseStream.Length - mpfImage.expectedDataSize;
            for (int index = 0; index < mpfImage.expectedFrames; ++index)
            {
                int left = binaryReader.ReadUInt16();
                int top = binaryReader.ReadUInt16();
                int num2 = binaryReader.ReadUInt16();
                int num3 = binaryReader.ReadUInt16();
                int width = num2 - left;
                int height = num3 - top;
                int num4 = binaryReader.ReadUInt16();
                int num5 = binaryReader.ReadUInt16();
                int xOffset = (num4 % 256 << 8) + num4 / 256;
                int yOffset = (num5 % 256 << 8) + num5 / 256;
                long num6 = binaryReader.ReadUInt32();
                if (left == ushort.MaxValue && num2 == ushort.MaxValue)
                {
                    mpfImage.palette = string.Format("mns{0:D3}.pal", num6);
                    --mpfImage.expectedFrames;
                }
                else
                    mpfImage.palette = "mns000.pal";
                byte[] rawData = null;
                if (height > 0 && width > 0)
                {
                    long position = binaryReader.BaseStream.Position;
                    binaryReader.BaseStream.Seek(num1 + num6, SeekOrigin.Begin);
                    rawData = binaryReader.ReadBytes(height * width);
                    binaryReader.BaseStream.Seek(position, SeekOrigin.Begin);
                }
                mpfImage.frames[index] = new MPFFrame(left, top, width, height, xOffset, yOffset, rawData);
            }
            binaryReader.Close();
            return mpfImage;
        }
    }
}
