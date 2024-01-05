// ReSharper disable once CheckNamespace

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="Array" />.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    ///     Performs the specified action on each element of the array.
    /// </summary>
    /// <param name="array">
    /// </param>
    /// <param name="action">
    /// </param>
    internal static void ForEach(this Array array, Action<Array, int[]> action)
    {
        if (array.LongLength == 0)
            return;

        var walker = new ArrayTraverse(array);

        do
            action(array, walker.Position);
        while (walker.Step());
    }

    internal sealed class ArrayTraverse
    {
        private readonly int[] MaxLengths;
        public readonly int[] Position;

        public ArrayTraverse(Array array)
        {
            MaxLengths = new int[array.Rank];

            for (var i = 0; i < array.Rank; ++i)
                MaxLengths[i] = array.GetLength(i) - 1;

            Position = new int[array.Rank];
        }

        public bool Step()
        {
            for (var i = 0; i < Position.Length; ++i)
                if (Position[i] < MaxLengths[i])
                {
                    Position[i]++;

                    for (var j = 0; j < i; j++)
                        Position[j] = 0;

                    return true;
                }

            return false;
        }
    }
}