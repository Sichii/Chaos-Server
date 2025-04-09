#region
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;
#endregion

namespace Chaos.MetaData.ItemMetaData;

/// <summary>
///     A node that stores metadata about an item
/// </summary>
public sealed record ItemMetaNode : IMetaNode
{
    /// <summary>
    ///     The category of the item, used for bank sorting
    /// </summary>
    public string Category
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(field))
                Length -= Encoding.GetByteCount(field);

            field = value;

            Length += Encoding.GetByteCount(value);
        }
    }

    /// <summary>
    ///     The class that can equip the item
    /// </summary>
    public BaseClass Class { get; set; }

    /// <summary>
    ///     A short description of the item
    /// </summary>
    public string Description
    {
        get;

        set
        {
            if (!string.IsNullOrEmpty(field))
                Length -= Encoding.GetByteCount(field);

            field = value;

            Length += Encoding.GetByteCount(value);
        }
    }

    /// <summary>
    ///     The length of the serialized data
    /// </summary>
    public int Length { get; private set; }

    /// <summary>
    ///     The level required to equip the item
    /// </summary>
    public int Level
    {
        get;

        set
        {
            Length -= field.ToString()
                           .Length;

            field = value;

            Length += field.ToString()
                           .Length;
        }
    }

    /// <summary>
    ///     The name of the item
    /// </summary>
    public string Name
    {
        get;

        init
        {
            if (!string.IsNullOrEmpty(field))
                Length -= Encoding.GetByteCount(field);

            field = value;

            Length += Encoding.GetByteCount(value);
        }
    }

    /// <summary>
    ///     The weight of the item
    /// </summary>
    public int Weight
    {
        get;

        set
        {
            Length -= field.ToString()
                           .Length;

            field = value;

            Length += field.ToString()
                           .Length;
        }
    }

    private static Encoding Encoding { get; } = Encoding.GetEncoding(949);

    /// <summary>
    ///     A node that stores metadata about an item
    /// </summary>
    /// <param name="name">
    /// </param>
    public ItemMetaNode(string name)
    {
        Length = 16;
        Category = "other";
        Description = string.Empty;
        Name = name;
    }

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name); // 1 byte for length + length of string
        writer.WriteUInt16(5); // 2 bytes for property count

        writer.WriteString16(Level.ToString()); // 2 bytes for length + length of string
        writer.WriteString16(((int)Class).ToString()); // 2 bytes for length + 1 byte for length of string
        writer.WriteString16(Weight.ToString()); // 2 bytes for length + length of string
        writer.WriteString16(Category); // 2 bytes for length + length of string
        writer.WriteString16(Description); // 2 bytes for length + length of string
    }

    /// <summary>
    ///     Deconstructs the <see cref="ItemMetaNode" /> into its components
    /// </summary>
    /// <param name="name">
    /// </param>
    public void Deconstruct(out string name) => name = Name;
}