using System.IO;
using System.Reflection;

namespace Chaos
{
    internal struct Tile
    {
        internal byte[] sotp => Properties.Resources.sotp;
        internal short Background { get; }
        internal short LeftForeground { get; }
        internal short RightForeground { get; }
        public bool IsWall => (LeftForeground > 0 && (sotp[LeftForeground - 1] & 15) == 15) || (RightForeground > 0 && (sotp[RightForeground - 1] & 15) == 15);

        internal Tile(short background, short leftForeground, short rightForeground)
        {
            Background = background;
            LeftForeground = leftForeground;
            RightForeground = rightForeground;
        }
    }
}
