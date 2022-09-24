using System.Runtime.CompilerServices;

namespace Chaos.Extensions.Common;

public static class ArrayExtensions
{
    /// <summary>
    ///     Flattens a multi-dimensional array, reading it left to right, top to bottom.
    /// </summary>
    public static IEnumerable<T> Flatten<T>(this T[,] map)
    {
        for (var y = 0; y < map.GetLength(1); y++)
            for (var x = 0; x < map.GetLength(0); x++)
                yield return map[x, y];
    }

    /// <summary>
    ///     Flattens a multi-dimensional array, reading it left to right, top to bottom.
    /// </summary>
    public static IEnumerable<T> Flatten<T>(this T[][] map)
    {
        for (var y = 0; y < map.Length; y++)
        {
            var arr = map[y];

            for (var x = 0; x < arr.Length; x++)
                yield return arr[x];
        }
    }

    /// <summary>
    ///     Randomized the order of the elements in the array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Shuffle<T>(this IList<T> arr)
    {
        for (var i = arr.Count - 1; i > 0; i--)
        {
            var j = Random.Shared.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
}