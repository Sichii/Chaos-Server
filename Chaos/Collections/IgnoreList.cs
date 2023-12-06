using Chaos.Common.Collections.Synchronized;

namespace Chaos.Collections;

public sealed class IgnoreList(IEnumerable<string>? items = null) : SynchronizedHashSet<string>(items, StringComparer.OrdinalIgnoreCase)
{
    public override string ToString() => string.Join(Environment.NewLine, this);
}