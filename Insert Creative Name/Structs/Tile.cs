using System.IO;
using System.Reflection;

namespace Chaos
{
    internal struct Tile
    {
        internal byte[] sotp;
        internal short Background { get; }
        internal short LeftForeground { get; }
        internal short RightForeground { get; }
        public bool IsWall => (LeftForeground > 0 && (sotp[LeftForeground - 1] & 15) == 15) || (RightForeground > 0 && (sotp[RightForeground - 1] & 15) == 15);

        internal Tile(ushort mapId, short x, short y, short background, short leftForeground, short rightForeground)
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Chaos.sotp.dat"))
            {
                int num = (int)manifestResourceStream.Length;
                sotp = new byte[num];
                manifestResourceStream.Read(sotp, 0, num);
            }

            Background = background;
            LeftForeground = leftForeground;
            RightForeground = rightForeground;
        }
    }
}
