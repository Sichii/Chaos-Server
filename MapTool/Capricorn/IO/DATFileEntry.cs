// Decompiled with JetBrains decompiler
// Type: Capricorn.IO.DATFileEntry
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

namespace Capricorn.IO
{
    public class DATFileEntry
    {
        public long FileSize => EndAddress - StartAddress;

        public long EndAddress { get; }

        public long StartAddress { get; }

        public string Name { get; set; }

        public DATFileEntry(string name, long startAddress, long endAddress)
        {
            Name = name;
            StartAddress = startAddress;
            EndAddress = endAddress;
        }

        public override string ToString() => "{Name = " + Name + ", Size = " + FileSize.ToString("###,###,###,###,###0") + "}";
    }
}
