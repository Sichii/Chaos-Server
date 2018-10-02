// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.MPFFrame
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

namespace Capricorn.Drawing
{
    public class MPFFrame
    {
        public bool IsValid => RawData != null && RawData.Length >= 1 && Width >= 1 && Height >= 1 && Width * Height == RawData.Length;

        public int OffsetY { get; }

        public int OffsetX { get; }

        public byte[] RawData { get; }

        public int Height { get; }

        public int Width { get; }

        public int Top { get; set; }

        public int Left { get; set; }

        public MPFFrame(int left, int top, int width, int height, int xOffset, int yOffset, byte[] rawData)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            OffsetX = xOffset;
            this.OffsetY = yOffset;
            RawData = rawData;
        }

        public override string ToString() => "{X = " + Left.ToString() + ", Y = " + Top.ToString() + ", Width = " + Width.ToString() + ", Height = " + Height.ToString() + ", Offset = (" + OffsetX.ToString() + ", " + OffsetY.ToString() + ")}";
    }
}
