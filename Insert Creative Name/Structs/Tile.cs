using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal struct Tile
    {
        internal static byte[] sotp;
        internal short background;
        internal short leftForeground;
        internal short rightForeground;
        public bool IsWall => (leftForeground > 0 && (sotp[leftForeground - 1] & 15) == 15) || (rightForeground > 0 && (sotp[rightForeground - 1] & 15) == 15);

        static Tile()
        {
            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Insert_Creative_Name.sotp.dat"))
            {
                int num = (int)manifestResourceStream.Length;
                sotp = new byte[num];
                manifestResourceStream.Read(sotp, 0, num);
            }
        }
        internal Tile(short background, short leftForeground, short rightForeground)
        {
            this.background = background;
            this.leftForeground = leftForeground;
            this.rightForeground = rightForeground;
        }
    }
}
