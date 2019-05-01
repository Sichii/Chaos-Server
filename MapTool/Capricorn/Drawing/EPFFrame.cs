// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.EPFFrame
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

namespace Capricorn.Drawing
{
    public class EPFFrame
    {
        private int left;
        private int top;
        private int width;
        private int height;
        private byte[] rawData;

        public bool IsValid
        {
            get
            {
                return rawData != null && rawData.Length >= 1 && (width >= 1 && height >= 1) && width * height == rawData.Length;
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

        public int Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
            }
        }

        public int Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }

        public EPFFrame(int left, int top, int width, int height, byte[] rawData)
        {
            this.left = left;
            this.top = top;
            this.width = width;
            this.height = height;
            this.rawData = rawData;
        }

        public override string ToString()
        {
            return "{X = " + left.ToString() + ", Y = " + top.ToString() + ", Width = " + width.ToString() + ", Height = " + height.ToString() + "}";
        }
    }
}
