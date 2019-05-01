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
    /// <summary>
    /// Contains useful static methods for various purposes, not directly affecting the game.
    /// </summary>
    internal static class Utilities
    {
        private static readonly Random random = new Random((int)DateTime.UtcNow.Ticks);

        /// <summary>
        /// Get a random number between two values. DOUBLE INCLUSIVE.
        /// </summary>
        /// <param name="minValue">Minimum value of the randomly generated number.</param>
        /// <param name="maxValue">Maximum value of the randomly generated number.</param>
        /// <param name="positiveOnly">Whether or not the number produced should be an absolute value.</param>
        internal static int Random(int minValue = int.MinValue, int maxValue = int.MaxValue, bool positiveOnly = true) => positiveOnly ? Math.Abs(random.Next(minValue, maxValue + 1)) : random.Next(minValue, maxValue + 1);

        /// <summary>
        /// Converts an image to a byte array and returns it.
        /// </summary>
        /// <param name="img">The image to convert.</param>
        internal static byte[] ImageToByteArray(Image img)
        {
            byte[] byteArray = new byte[0];
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                byteArray = stream.ToArray();
            }

            return byteArray;
        }

        /// <summary>
        /// Clamps a number within given bounds. Returns the number itself if it does not violate those bounds.
        /// </summary>
        /// <typeparam name="T">Any numeric type.</typeparam>
        /// <param name="value">A value to be clamped.</param>
        /// <param name="min">The minimum value of the return.</param>
        /// <param name="max">The maximum value of the return.</param>
        internal static T Clamp<T>(int value, int min, int max) where T : struct, IComparable, IConvertible, IFormattable =>
            (T)Convert.ChangeType((value < min) ? min 
                : (value > max) ? max 
                : value, typeof(T));

        /// <summary>
        /// Clamps a number within given bounds. Returns the number itself if it does not violate those bounds.
        /// </summary>
        /// <typeparam name="T">Any numeric type.</typeparam>
        /// <param name="value">A value to be clamped.</param>
        /// <param name="min">The minimum value of the return.</param>
        /// <param name="max">The maximum value of the return.</param>
        internal static T Clamp<T>(long value, long min, long max) where T : struct, IComparable, IConvertible, IFormattable =>
            (T)Convert.ChangeType((value < min) ? min 
                : (value > max) ? max 
                : value, typeof(T));

        /// <summary>
        /// Clamps a number within given bounds. Returns the number itself if it does not violate those bounds.
        /// </summary>
        /// <typeparam name="T">Any numeric type.</typeparam>
        /// <param name="value">A value to be clamped.</param>
        /// <param name="min">The minimum value of the return.</param>
        /// <param name="max">The maximum value of the return.</param>
        internal static T Clamp<T>(float value, float min, float max) where T : struct, IComparable, IConvertible, IFormattable =>
            (T)Convert.ChangeType((value < min) ? min 
                : (value > max) ? max 
                : value, typeof(T));

        /// <summary>
        /// Clamps a number within given bounds. Returns the number itself if it does not violate those bounds.
        /// </summary>
        /// <typeparam name="T">Any numeric type.</typeparam>
        /// <param name="value">A value to be clamped.</param>
        /// <param name="min">The minimum value of the return.</param>
        /// <param name="max">The maximum value of the return.</param>
        internal static T Clamp<T>(double value, double min, double max) where T : struct, IComparable, IConvertible, IFormattable =>
            (T)Convert.ChangeType((value < min) ? min 
                : (value > max) ? max 
                : value, typeof(T));


        /// <summary>
        /// Converts the first letter of a string to uppercase, and returns it.
        /// </summary>
        /// <param name="str">A string to be modified.</param>
        internal static string FirstUpper(string str)
        {
            char[] strArr = str.ToCharArray();
            strArr[0] = char.ToUpper(strArr[0]);

            return new string(strArr);
        }
    }
}
