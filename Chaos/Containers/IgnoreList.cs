using System;
using System.Collections.Generic;
using Chaos.Core.Collections.Synchronized;

namespace Chaos.Containers;

public class IgnoreList : SynchronizedHashSet<string>
{
    public IgnoreList(IEnumerable<string>? items = null)
        : base(items, StringComparer.OrdinalIgnoreCase) { }

    public override string ToString() => string.Join(Environment.NewLine, this);
}