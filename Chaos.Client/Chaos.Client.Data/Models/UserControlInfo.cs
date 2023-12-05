using System.Collections.ObjectModel;
using Chaos.Client.Data.Utilities;
using DALib.Data;

namespace Chaos.Client.Data.Models;

public sealed class UserControlInfo(string name) : KeyedCollection<string, ControlInfo>(StringComparer.OrdinalIgnoreCase)
{
    public string Name { get; set; } = name;

    public static Dictionary<string, UserControlInfo> FromArchive(DataArchive setoaArchive)
    {
        var controlLookup = new Dictionary<string, UserControlInfo>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in setoaArchive.GetEntries(".txt"))
        {
            var controlCollection = FromEntry(entry);
            controlLookup.Add(controlCollection.Name, controlCollection);
        }

        return controlLookup;
    }

    public static UserControlInfo FromEntry(DataArchiveEntry entry)
    {
        using var segment = entry.ToStreamSegment();

        var name = Path.GetFileNameWithoutExtension(entry.EntryName);
        var controlParser = new ControlInfoParser();

        return controlParser.Parse(name, segment);
    }

    /// <inheritdoc />
    protected override string GetKeyForItem(ControlInfo item) => item.Name;
}