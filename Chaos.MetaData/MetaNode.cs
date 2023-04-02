using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData;

/// <inheritdoc />
public sealed record MetaNode(string Name) : IMetaNode
{
    /// <summary>
    ///     The properties of the meta node
    /// </summary>
    public ICollection<string> Properties { get; } = new List<string>();

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteUInt16((ushort)Properties.Count);

        foreach (var property in Properties)
            writer.WriteString16(property);
    }
}