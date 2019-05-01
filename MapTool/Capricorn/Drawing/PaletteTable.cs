// Decompiled with JetBrains decompiler
// Type: Capricorn.Drawing.PaletteTable
// Assembly: Accolade, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: A987DE0D-CB54-451E-92F3-D381FD0B091A
// Assembly location: D:\Dropbox\Ditto (1)\Other Bots and TOols\Kyle's Known Bots\Accolade\Accolade.exe

using Capricorn.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Capricorn.Drawing
{
    public class PaletteTable
    {
        private List<PaletteTableEntry> entries = new List<PaletteTableEntry>();
        private Dictionary<int, Palette256> palettes = new Dictionary<int, Palette256>();
        private List<PaletteTableEntry> overrides = new List<PaletteTableEntry>();

        public Palette256 this[int index]
        {
            get
            {
                int index1 = 0;
                foreach (PaletteTableEntry paletteTableEntry in overrides)
                {
                    if (index >= paletteTableEntry.Min && index <= paletteTableEntry.Max)
                        index1 = paletteTableEntry.Palette;
                }
                foreach (PaletteTableEntry paletteTableEntry in entries)
                {
                    if (index >= paletteTableEntry.Min && index <= paletteTableEntry.Max)
                        index1 = paletteTableEntry.Palette;
                }
                return palettes[index1];
            }
        }

        public Palette256 GetPalette(string image)
        {
            int index = 0;
            int int32 = Convert.ToInt32(image.Substring(2, 3));
            if (image.StartsWith("w"))
            {
                foreach (PaletteTableEntry paletteTableEntry in this.overrides)
                {
                    if (int32 >= paletteTableEntry.Min && int32 <= paletteTableEntry.Max)
                        index = paletteTableEntry.Palette;
                }
            }
            else if (image.StartsWith("m"))
            {
                foreach (PaletteTableEntry entry in this.entries)
                {
                    if (int32 >= entry.Min && int32 <= entry.Max)
                        index = entry.Palette;
                }
            }
            if (index < 0 || index > this.palettes.Count)
                index = 0;
            return this.palettes[index];
        }

        private int LoadTableInternal(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            StreamReader streamReader = new StreamReader(stream);
            entries.Clear();
            while (!streamReader.EndOfStream)
            {
                string[] strArray = streamReader.ReadLine().Split(' ');
                if (strArray.Length == 3)
                    entries.Add(new PaletteTableEntry(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2])));
                else if (strArray.Length == 2)
                {
                    int min = Convert.ToInt32(strArray[0]);
                    int max = min;
                    int palette = Convert.ToInt32(strArray[1]);
                    entries.Add(new PaletteTableEntry(min, max, palette));
                }
            }
            streamReader.Close();
            return entries.Count;
        }

        public int LoadPalettes(string pattern, DATArchive archive)
        {
            palettes.Clear();
            foreach (DATFileEntry datFileEntry in archive.Files)
            {
                if (datFileEntry.Name.ToUpper().EndsWith(".PAL") && datFileEntry.Name.ToUpper().StartsWith(pattern.ToUpper()))
                    palettes.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(datFileEntry.Name).Remove(0, pattern.Length)), Palette256.FromArchive(datFileEntry.Name, archive));
            }
            return palettes.Count;
        }

        public int LoadPalettes(string pattern, string path)
        {
            string[] files = Directory.GetFiles(path, pattern + "*.PAL", SearchOption.TopDirectoryOnly);
            palettes.Clear();
            foreach (string str in files)
            {
                if (Path.GetFileName(str).ToUpper().EndsWith(".PAL") && Path.GetFileName(str).ToUpper().StartsWith(pattern.ToUpper()))
                    palettes.Add(Convert.ToInt32(Path.GetFileNameWithoutExtension(str).Remove(0, pattern.Length)), Palette256.FromFile(str));
            }
            return palettes.Count;
        }

        public int LoadTables(string pattern, DATArchive archive)
        {
            entries.Clear();
            foreach (DATFileEntry entry in archive.Files)
            {
                if (entry.Name.ToUpper().EndsWith(".TBL") && entry.Name.ToUpper().StartsWith(pattern.ToUpper()))
                {
                    string s = Path.GetFileNameWithoutExtension(entry.Name).Remove(0, pattern.Length);
                    if (s != "ani")
                    {
                        StreamReader streamReader = new StreamReader(new MemoryStream(archive.ExtractFile(entry)));
                        while (!streamReader.EndOfStream)
                        {
                            string[] strArray = streamReader.ReadLine().Split(' ');
                            if (strArray.Length == 3)
                            {
                                int min = Convert.ToInt32(strArray[0]);
                                int max = Convert.ToInt32(strArray[1]);
                                int palette = Convert.ToInt32(strArray[2]);
                                int result = 0;
                                if (int.TryParse(s, out result))
                                    overrides.Add(new PaletteTableEntry(min, max, palette));
                                else
                                    entries.Add(new PaletteTableEntry(min, max, palette));
                            }
                            else if (strArray.Length == 2)
                            {
                                int min = Convert.ToInt32(strArray[0]);
                                int max = min;
                                int palette = Convert.ToInt32(strArray[1]);
                                int result = 0;
                                if (int.TryParse(s, out result))
                                    overrides.Add(new PaletteTableEntry(min, max, palette));
                                else
                                    entries.Add(new PaletteTableEntry(min, max, palette));
                            }
                        }
                        streamReader.Close();
                    }
                }
            }
            return entries.Count;
        }

        public int LoadTables(string pattern, string path)
        {
            string[] files = Directory.GetFiles(path, pattern + "*.TBL", SearchOption.TopDirectoryOnly);
            entries.Clear();
            foreach (string path1 in files)
            {
                if (Path.GetFileName(path1).ToUpper().EndsWith(".TBL") && Path.GetFileName(path1).ToUpper().StartsWith(pattern.ToUpper()))
                {
                    string s = Path.GetFileNameWithoutExtension(path1).Remove(0, pattern.Length);
                    if (s != "ani")
                    {
                        foreach (string str in File.ReadAllLines(path1))
                        {
                            char[] chArray = new char[1]
                            {
                ' '
                            };
                            string[] strArray = str.Split(chArray);
                            if (strArray.Length == 3)
                            {
                                int min = Convert.ToInt32(strArray[0]);
                                int max = Convert.ToInt32(strArray[1]);
                                int palette = Convert.ToInt32(strArray[2]);
                                int result = 0;
                                if (int.TryParse(s, out result))
                                    overrides.Add(new PaletteTableEntry(min, max, palette));
                                else
                                    entries.Add(new PaletteTableEntry(min, max, palette));
                            }
                            else if (strArray.Length == 2)
                            {
                                int min = Convert.ToInt32(strArray[0]);
                                int max = min;
                                int palette = Convert.ToInt32(strArray[1]);
                                int result = 0;
                                if (int.TryParse(s, out result))
                                    overrides.Add(new PaletteTableEntry(min, max, palette));
                                else
                                    entries.Add(new PaletteTableEntry(min, max, palette));
                            }
                        }
                    }
                }
            }
            return entries.Count;
        }

        public override string ToString()
        {
            return "{Entries = " + entries.Count + ", Palettes = " + (string)(object)palettes.Count + "}";
        }
    }
}
