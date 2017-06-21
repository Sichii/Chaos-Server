using System;
using System.Drawing;
using System.IO;

namespace Chaos
{
    internal static class Utility
    {
        private static Random random = new Random();

        internal static int Random() => random.Next();
        internal static int Random(int maxValue) => random.Next(maxValue);
        internal static int Random(int minValue, int maxValue) => random.Next(minValue, maxValue);

        internal static byte[] ImageToByteArray(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }
    }
}
