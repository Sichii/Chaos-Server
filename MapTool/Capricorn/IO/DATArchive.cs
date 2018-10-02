// Decompiled with JetBrains decompiler
// Type: Capricorn.IO.DATArchive
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

using System.IO;
using System.Text;

namespace Capricorn.IO
{
    public class DATArchive
    {
        public DATFileEntry this[int index]
        {
            get => Files[index];
            set => Files[index] = value;
        }

        public string FileName { get; set; }

        public DATFileEntry[] Files { get; private set; }

        public int ExpectedFiles { get; private set; }

        public static DATArchive FromFile(string file)
        {
            var binaryReader = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
            var datArchive = new DATArchive
            {
                FileName = file,
                ExpectedFiles = binaryReader.ReadInt32()
            };
            datArchive.Files = new DATFileEntry[datArchive.ExpectedFiles - 1];
            for (int index = 0; index < datArchive.ExpectedFiles - 1; ++index)
            {
                long startAddress = binaryReader.ReadUInt32();
                string name = Encoding.ASCII.GetString(binaryReader.ReadBytes(13));
                long endAddress = binaryReader.ReadUInt32();
                binaryReader.BaseStream.Seek(-4L, SeekOrigin.Current);
                int startIndex = name.IndexOf(char.MinValue);
                if (startIndex != -1)
                    name = name.Remove(startIndex, 13 - startIndex);
                datArchive.Files[index] = new DATFileEntry(name, startAddress, endAddress);
            }
            binaryReader.Close();
            return datArchive;
        }

        public bool Contains(string name)
        {
            foreach (DATFileEntry datFileEntry in Files)
            {
                if (datFileEntry.Name == name)
                    return true;
            }
            return false;
        }

        public bool Contains(string name, bool ignoreCase)
        {
            foreach (DATFileEntry datFileEntry in Files)
            {
                if (ignoreCase)
                {
                    if (datFileEntry.Name.ToUpper() == name.ToUpper())
                        return true;
                }
                else if (datFileEntry.Name == name)
                    return true;
            }
            return false;
        }

        public int IndexOf(string name)
        {
            for (int index = 0; index < Files.Length; ++index)
            {
                if (Files[index].Name == name)
                    return index;
            }
            return -1;
        }

        public int IndexOf(string name, bool ignoreCase)
        {
            for (int index = 0; index < Files.Length; ++index)
            {
                if (ignoreCase)
                {
                    if (Files[index].Name.ToUpper() == name.ToUpper())
                        return index;
                }
                else if (Files[index].Name == name)
                    return index;
            }
            return -1;
        }

        public byte[] ExtractFile(string name)
        {
            if (!Contains(name))
                return null;
            var binaryReader = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            int index = IndexOf(name);
            binaryReader.BaseStream.Seek(Files[index].StartAddress, SeekOrigin.Begin);
            byte[] numArray = binaryReader.ReadBytes((int)Files[index].FileSize);
            binaryReader.Close();
            return numArray;
        }

        public byte[] ExtractFile(string name, bool ignoreCase)
        {
            if (!Contains(name, ignoreCase))
                return null;
            var binaryReader = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            int index = IndexOf(name, ignoreCase);
            binaryReader.BaseStream.Seek(Files[index].StartAddress, SeekOrigin.Begin);
            byte[] numArray = binaryReader.ReadBytes((int)Files[index].FileSize);
            binaryReader.Close();
            return numArray;
        }

        public byte[] ExtractFile(DATFileEntry entry)
        {
            if (!Contains(entry.Name))
                return null;
            var binaryReader = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
            binaryReader.BaseStream.Seek(entry.StartAddress, SeekOrigin.Begin);
            byte[] numArray = binaryReader.ReadBytes((int)entry.FileSize);
            binaryReader.Close();
            return numArray;
        }

        public override string ToString() => "{Name = " + FileName + ", Files = " + ExpectedFiles.ToString() + "}";
    }
}
