namespace Chaos.Networking.Extensions;

internal static class ListExtensions
{
    /// <summary>
    ///     Mimics index-based writing for a list.
    /// </summary>
    internal static void Write<T>(
        this List<T> thisList,
        int startIndex,
        T[] sourceArray,
        int sourceIndex,
        int count
    )
    {
        for (var i = 0; i < count; i++)
        {
            var dstIndex = startIndex + i;
            var srcIndex = sourceIndex + i;

            if (dstIndex >= thisList.Count)
                thisList.Add(sourceArray[srcIndex]);
            else
                thisList[dstIndex] = sourceArray[srcIndex];
        }
    }
}