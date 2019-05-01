// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.PaletteTableEntry
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

namespace Capricorn.Drawing
{
    public class PaletteTableEntry
    {
        private int min;
        private int max;
        private int palette;

        public int Palette
        {
            get
            {
                return palette;
            }
            set
            {
                palette = value;
            }
        }

        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
            }
        }

        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                min = value;
            }
        }

        public PaletteTableEntry(int min, int max, int palette)
        {
            this.min = min;
            this.max = max;
            this.palette = palette;
        }

        public override string ToString()
        {
            return "{Min = " + min.ToString() + ", Max = " + max.ToString() + ", Palette = " + palette.ToString() + "}";
        }
    }
}
