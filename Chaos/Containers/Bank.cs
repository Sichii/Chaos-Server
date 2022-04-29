using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chaos.PanelObjects;

namespace Chaos.Containers;

public class Bank : IEnumerable<Item>
{
    public uint Gold { get; set; }
    public Dictionary<string, Item> Items { get; set; }

    public Bank(IEnumerable<Item>? items = null)
    {
        items ??= Enumerable.Empty<Item>();

        Items = items.ToDictionary(item => item.DisplayName, StringComparer.OrdinalIgnoreCase);
    }

    public IEnumerator<Item> GetEnumerator()
    {
        var enumerable = Enumerable.Empty<Item>();

        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}