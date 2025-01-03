#region
using Chaos.Models.Legend;
using Chaos.Time;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a collection of legend marks
/// </summary>
public sealed class Legend : IEnumerable<LegendMark>
{
    private readonly ConcurrentDictionary<string, LegendMark> Marks;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Legend" /> class
    /// </summary>
    /// <param name="marks">
    ///     The marks to populate the legend with
    /// </param>
    public Legend(IEnumerable<LegendMark>? marks = null)
    {
        marks ??= [];
        Marks = new ConcurrentDictionary<string, LegendMark>(marks.ToDictionary(mark => mark.Key), StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<LegendMark> GetEnumerator()
    {
        foreach (var kvp in Marks.OrderBy(kvp => kvp.Value.Added))
            yield return kvp.Value;
    }

    /// <summary>
    ///     Adds a mark to the legend, or increments the count of an existing mark
    /// </summary>
    /// <param name="mark">
    ///     The mark to add or increment the value of
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the mark was added or incremented, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <remarks>
    ///     Marks are considered equal if their text is the same. When an existing mark is found, it's count is incremented,
    ///     and the added time is updated. This causes the mark to move to the bottom of the legend
    /// </remarks>
    public bool AddOrAccumulate(LegendMark mark)
    {
        if (Marks.TryGetValue(mark.Key, out var existingMark))
        {
            existingMark.Text = mark.Text;
            existingMark.Count++;
            existingMark.Added = GameTime.Now;

            return true;
        }

        return Marks.TryAdd(mark.Key, mark);
    }

    /// <summary>
    ///     Adds a unique mark to the legend
    /// </summary>
    /// <param name="mark">
    ///     The mark to add
    /// </param>
    /// <remarks>
    ///     Unique marks do not accumulate counts. If a mark with the same key already exists, it is replaced
    /// </remarks>
    public void AddUnique(LegendMark mark) => Marks[mark.Key] = mark;

    /// <summary>
    ///     Determines if the legend contains the specified mark
    /// </summary>
    /// <param name="mark">
    ///     The mark to check for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the legend contains another entry with the same key, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool Contains(LegendMark mark) => ContainsKey(mark.Key);

    /// <summary>
    ///     Determines if the legend contains a mark with the specified key
    /// </summary>
    /// <param name="key">
    ///     The key to check for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the legend contains another entry with the same key, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool ContainsKey(string key) => Marks.ContainsKey(key);

    /// <summary>
    ///     Gets the count of the mark with the specified key
    /// </summary>
    /// <param name="key">
    ///     The key to check for
    /// </param>
    public int GetCount(string key)
    {
        if (Marks.TryGetValue(key, out var existingMark))
            return existingMark.Count;

        return 0;
    }

    /// <summary>
    ///     Removes the mark with the specified key from the legend
    /// </summary>
    /// <param name="key">
    ///     The key to check for
    /// </param>
    /// <param name="mark">
    ///     The mark that got removed, if found
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the mark was found and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool Remove(string key, [MaybeNullWhen(false)] out LegendMark mark) => Marks.Remove(key, out mark);

    /// <summary>
    ///     Removes the specified count from the mark with the specified key
    /// </summary>
    /// <param name="key">
    ///     The key of the mark to look for
    /// </param>
    /// <param name="count">
    ///     The count to remove from the mark if found
    /// </param>
    /// <param name="mark">
    ///     The mark if found
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the mark was found and the count was removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <remarks>
    ///     If the count of the mark is less than or equal to zero after removing the specified count, the mark is removed from
    ///     the legend
    /// </remarks>
    public bool RemoveCount(string key, int count, [MaybeNullWhen(false)] out LegendMark mark)
    {
        if (Marks.TryGetValue(key, out mark))
        {
            mark.Count -= count;

            if (mark.Count <= 0)
                return Marks.Remove(key, out mark);

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Attempts to get the mark with the specified key
    /// </summary>
    /// <param name="key">
    ///     The key of the mark to look for
    /// </param>
    /// <param name="existingMark">
    ///     The mark if found
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the mark was found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out LegendMark existingMark) => Marks.TryGetValue(key, out existingMark);
}