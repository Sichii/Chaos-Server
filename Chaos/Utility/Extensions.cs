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

namespace Chaos
{
    internal static class Extensions
    {
        /// <summary>
        /// Returns the Direction Enum equivalent of the reverse of a given cardinal direction.
        /// </summary>
        internal static Direction Reverse(this Direction direction)
        {
            switch(direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                default:
                    return Direction.Invalid;
            }
        }

        internal static IEnumerable<Direction> StartEnumerable(this Direction direction)
        {
            int dir = (int)direction;
            for(int i = 0; i < 4; i++)
            {
                yield return (Direction)dir;

                dir++;
                dir = (dir > 3) ? dir - 4 : dir;
            }
        }

        

        /// <summary>
        /// Gets a generic IEnumerable at compile time, from an array.
        /// </summary>
        internal static IEnumerable<T> GetEnumerable<T>(this T[] arr) => arr;

        /// <summary>
        /// Flattens a 2d array and returns an iterator to go through all of it's indexes.
        /// </summary>
        internal static IEnumerable<T> Flatten<T>(this T[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                    yield return map[x, y];
        }

        /// <summary>
        /// Checks if a floating point number is effectively equivalent to another, with room for rounding errors.
        /// </summary>
        internal static bool IsNearly(this float f1, float f2, float epsilon = 0.000001f) => Math.Abs(f1 - f2) <= epsilon;
    }
}
