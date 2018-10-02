// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.PaletteTableEntry
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

namespace Capricorn.Drawing
{
    public class PaletteTableEntry
    {
        public int Palette { get; set; }

        public int Max { get; set; }

        public int Min { get; set; }

        public PaletteTableEntry(int min, int max, int palette)
        {
            Min = min;
            Max = max;
            Palette = palette;
        }

        public override string ToString() => "{Min = " + Min.ToString() + ", Max = " + Max.ToString() + ", Palette = " + Palette.ToString() + "}";
    }
}
