#region
using Chaos.Collections.Synchronized;
using Chaos.Extensions.Common;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a collection of titles
/// </summary>
/// <param name="items">
///     Titles to populate the collection with
/// </param>
public sealed class TitleList(IEnumerable<string>? items = null)
    : SynchronizedList<string>(items?.Distinct(StringComparer.OrdinalIgnoreCase) ?? [])
{
    /// <inheritdoc />
    public override void Add(string item)
    {
        if (List.ContainsI(item))
            return;

        base.Add(item);
    }

    /// <inheritdoc />
    public override void Insert(int index, string item)
    {
        if (List.ContainsI(item))
            return;

        base.Insert(index, item);
    }

    public override string ToString() => string.Join(Environment.NewLine, this);
}