using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.ItemMetadata;

/// <summary>
///     A node that stores metadata about an item
/// </summary>
/// <param name="Name"></param>
public sealed record ItemMetaNode(string Name) : IMetaNode
{
    /// <summary>
    ///     The category of the item, used for bank sorting
    /// </summary>
    public string Category { get; set; } = "other";
    /// <summary>
    ///     The class that can equip the item
    /// </summary>
    public BaseClass Class { get; set; }
    /// <summary>
    ///     A short description of the item
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    ///     The level required to equip the item
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    ///     The name of the item
    /// </summary>
    public string Name { get; init; } = Name;
    /// <summary>
    ///     The weight of the item
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    ///     The length of the serialized data
    /// </summary>
    public int Length => 20 + Name.Length + Category.Length + Description.Length;

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name); // 1 byte for length + length of string
        writer.WriteUInt16(5); // 2 bytes for property count

        writer.WriteString16(Level.ToString()); // 2 bytes for length + 2 bytes for length of string
        writer.WriteString16(((int)Class).ToString()); // 2 bytes for length + 1 byte for length of string
        writer.WriteString16(Weight.ToString()); // 2 bytes for length + 3 bytes for length of string
        writer.WriteString16(Category); // 2 bytes for length + length of string
        writer.WriteString16(Description); // 2 bytes for length + length of string
    }
}