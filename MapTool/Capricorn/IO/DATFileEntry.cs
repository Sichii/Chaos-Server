// Decompiled with JetBrains decompiler
// Type: Capricorn.IO.DATFileEntry
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

namespace Capricorn.IO
{
    public class DATFileEntry
    {
        private string name;
        private long startAddress;
        private long endAddress;

        public long FileSize
        {
            get
            {
                return endAddress - startAddress;
            }
        }

        public long EndAddress
        {
            get
            {
                return endAddress;
            }
        }

        public long StartAddress
        {
            get
            {
                return startAddress;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public DATFileEntry(string name, long startAddress, long endAddress)
        {
            this.name = name;
            this.startAddress = startAddress;
            this.endAddress = endAddress;
        }

        public override string ToString()
        {
            return "{Name = " + name + ", Size = " + FileSize.ToString("###,###,###,###,###0") + "}";
        }
    }
}
