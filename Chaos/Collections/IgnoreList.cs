#region
using Chaos.Collections.Synchronized;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents the in-game ignore list
/// </summary>
/// <param name="items">
/// </param>
public sealed class IgnoreList(IEnumerable<string>? items = null) : SynchronizedHashSet<string>(items, StringComparer.OrdinalIgnoreCase)
{
    /// <inheritdoc />
    public override string ToString() => string.Join(Environment.NewLine, this);
}