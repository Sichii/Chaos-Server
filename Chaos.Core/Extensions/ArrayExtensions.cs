namespace Chaos.Core.Extensions;

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
}