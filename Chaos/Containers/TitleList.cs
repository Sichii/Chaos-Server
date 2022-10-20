using Chaos.Common.Collections.Synchronized;

namespace Chaos.Containers;

public sealed class TitleList : SynchronizedHashSet<string>
{
    public TitleList(IEnumerable<string>? items = null)
        : base(items, StringComparer.OrdinalIgnoreCase) { }

    public override string ToString() => string.Join(Environment.NewLine, this);
}