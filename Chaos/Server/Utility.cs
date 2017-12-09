// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

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

        internal static T Clamp<T>(T value, T min, T max) where T : IComparable<T> => (value.CompareTo(min) < 0) ? min : value.CompareTo(max) > 0 ? max : value;
    }
}
