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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Chaos
{
    /// <summary>
    /// Contains useful static methods for various purposes, not directly affecting the game.
    /// </summary>
    internal static class Utility
    {
        private static Random random = new Random();

        /// <summary>
        /// Get a random number between two values.
        /// </summary>
        /// <param name="minValue">Minimum value of the randomly generated number.</param>
        /// <param name="maxValue">Maximum value of the randomly generated number.</param>
        /// <param name="positiveOnly">Whether or not the number produced should be an absolute value.</param>
        internal static int Random(int minValue = int.MinValue, int maxValue = int.MaxValue, bool positiveOnly = true) => positiveOnly ? Math.Abs(random.Next(minValue, maxValue)) : random.Next(minValue, maxValue);

        /// <summary>
        /// Gets the current elapsed tick count as an Int64
        /// </summary>
        internal static long CurrentTicks => Stopwatch.GetTimestamp();

        /// <summary>
        /// Calculate DeltaTime as miliseconds and returns it.
        /// </summary>
        /// <param name="oldTicks">A previous metric of currently elapsed time in ticks.</param>
        internal static int DeltaTime(long oldTicks) => (int)(Stopwatch.GetTimestamp() - oldTicks)/10000;

        /// <summary>
        /// Calculate a DeltaTime and adjust it based on a capped delay as miliseconds and return it.
        /// </summary>
        /// <param name="oldTicks">A previous metric of currently elapsed time in ticks.</param>
        /// <param name="maxDelay">The longest period of time in miliseconds to wait.</param>
        internal static int ClampedDeltaTime(long oldTicks, int maxDelay) => Math.Max(0, 100 - DeltaTime(oldTicks));
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
        internal static T Clamp<T>(double value, int min, int max) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable =>
            (T)Convert.ChangeType(((value < min) ? min : (value > max) ? max : value), typeof(T));

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

        /// <summary>
        /// Creates an enumerable list of points representing a path between two given points, and returns it.
        /// </summary>
        /// <param name="start">Starting point for the creation of the path.</param>
        /// <param name="end">Ending point for the creation of the path.</param>
        /// <param name="includeStart">Whether or not to include the starting point in the result enumerable.</param>
        /// <param name="includeEnd">Whether or not to include the ending point in the result enumerable.</param>
        /// <returns></returns>
        internal static IEnumerable<Point> GetPath(Point start, Point end, bool includeStart = false, bool includeEnd = false)
        {
            if (includeStart)
                yield return start;

            while(start != end)
            {
                start.Offset(end.Relation(start));
                yield return start;
            }

            if (includeEnd)
                yield return end;
        }

        /// <summary>
        /// Retreives a list of diagonal points in relevance to the user, with an optional distance and direction. Direction.All is optional. Direction.Invalid direction returns empty list.
        /// </summary>
        internal static List<Point> GetDiagonalPoints(Point start, int degree = 1, Direction direction = Direction.All)
        {
            var diagonals = new List<Point>();

            if (direction == Direction.Invalid)
                return diagonals;

            for (int i = 1; i <= degree; i++)
            {
                switch (direction)
                {
                    case Direction.North:
                        diagonals.Add((start.X - i, start.Y - i));
                        diagonals.Add((start.X + i, start.Y - i));
                        break;
                    case Direction.East:
                        diagonals.Add((start.X + i, start.Y - i));
                        diagonals.Add((start.X + i, start.Y + i));
                        break;
                    case Direction.South:
                        diagonals.Add((start.X + i, start.Y + i));
                        diagonals.Add((start.X - i, start.Y + i));
                        break;
                    case Direction.West:
                        diagonals.Add((start.X - i, start.Y - i));
                        diagonals.Add((start.X - i, start.Y + i));
                        break;
                    case Direction.All:
                        diagonals.Add((start.X - i, start.Y - i));
                        diagonals.Add((start.X + i, start.Y - i));
                        diagonals.Add((start.X + i, start.Y + i));
                        diagonals.Add((start.X - i, start.Y + i));
                        break;
                }
            }

            return diagonals;
        }

        /// <summary>
        /// Retreives a list of points in a line from the user, with an option for distance and direction. Direction.All is optional. Direction.Invalid direction returns empty list.
        /// </summary>
        internal static List<Point> GetLinePoints(Point start, int degree = 1, Direction direction = Direction.All)
        {
            var linePoints = new List<Point>();

            if (direction == Direction.Invalid)
                return linePoints;

            for (int i = 1; i <= degree; i++)
            {
                switch (direction)
                {
                    case Direction.All:
                        linePoints.Add((start.X + i, start.Y));
                        linePoints.Add((start.X - i, start.Y));
                        linePoints.Add((start.X, start.Y + i));
                        linePoints.Add((start.X, start.Y - i));
                        break;
                    default:
                        start.Offset(direction);
                        linePoints.Add(start);
                        break;
                }
            }

            return linePoints;
        }
    }
}
