using System;

namespace ChaosLauncher
{
    internal struct StartInfo
    {
        public int Size { get; set; }
        public string Reserved { get; set; }
        public string Desktop { get; set; }
        public string Title { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int XCountChars { get; set; }
        public int YCountChars { get; set; }
        public int FillAttribute { get; set; }
        public int Flags { get; set; }
        public short ShowWindow { get; set; }
        public short Reserved2 { get; set; }
        public IntPtr Reserved3 { get; set; }
        public IntPtr StdInputHandle { get; set; }
        public IntPtr StdOutputHandle { get; set; }
        public IntPtr StdErrorHandle { get; set; }
    }
}
