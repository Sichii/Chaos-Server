using Chaos.IO.Memory;
using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata;

public sealed record MetaNode(string Name) : MetaNodeBase(Name)
{
    public ICollection<string> Properties { get; } = new List<string>();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteUInt16((ushort)Properties.Count);

        foreach (var property in Properties)
            writer.WriteString16(property);
    }
}