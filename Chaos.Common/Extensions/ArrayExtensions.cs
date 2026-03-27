// ReSharper disable once CheckNamespace

#region
using JetBrains.Annotations;
#endregion

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="Array" />.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    ///     Provides extension methods for <see cref="Array" />.
    /// </summary>
    extension(Array array)
    {
        /// <summary>
        ///     Performs the specified action on each element of the array.
        /// </summary>
        /// <param name="action">
        /// </param>
        internal void ForEach(Action<Array, int[]> action)
        {
            if (array.LongLength == 0)
                return;

            var walker = new ArrayTraverse(array);

            do
                action(array, walker.Position);
            while (walker.Step());
        }
    }

    /// <summary>
    ///     Provides extension methods for <see cref="Array" />.
    /// </summary>
    extension<T>(T[,] map)
    {
        /// <summary>
        ///     Flattens a multi-dimensional array, reading it left to right, top to bottom.
        /// </summary>
        public IEnumerable<T> Flatten()
        {
            for (var y = 0; y < map.GetLength(0); y++)
                for (var x = 0; x < map.GetLength(1); x++)
                    yield return map[y, x];
        }
    }

    /// <summary>
    ///     Provides extension methods for <see cref="Array" />.
    /// </summary>
    extension<T>(T[][] map)
    {
        /// <summary>
        ///     Flattens a multi-dimensional array, reading it left to right, top to bottom.
        /// </summary>
        public IEnumerable<T> Flatten()
        {
            for (var y = 0; y < map.Length; y++)
            {
                var arr = map[y];

                for (var x = 0; x < arr.Length; x++)
                    yield return arr[x];
            }
        }
    }

    /// <summary>
    ///     Provides extension methods for <see cref="Array" />.
    /// </summary>
    extension<T>(IList<T> arr)
    {
        /// <summary>
        ///     Randomizes in-place the order of the elements in the list. This will be significantly faster than using linq's
        ///     OrderBy.
        /// </summary>
        public void ShuffleInPlace()
        {
            for (var i = arr.Count - 1; i > 0; i--)
            {
                var j = Random.Shared.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }
    }

    extension<T>(T[] arr)
    {
        /// <summary>
        ///     Returns an enumerator that iterates through the array.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        [MustDisposeResource]
        public IEnumerator<T> GetGenericEnumerator() => ((IEnumerable<T>)arr).GetEnumerator();
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