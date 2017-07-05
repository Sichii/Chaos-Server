using System;
using System.Drawing;
using System.IO;

namespace Chaos
{
    internal static class Utility
    {
        private static Random random = new Random();
        internal static int Random(int minValue = int.MinValue, int maxValue = int.MaxValue, bool positiveOnly = true) => positiveOnly ? Math.Abs(random.Next(minValue, maxValue)) : random.Next(minValue, maxValue);

        internal static byte[] ImageToByteArray(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byteArray = stream.ToArray();
            }
            return byteArray;
        }
    }
}
