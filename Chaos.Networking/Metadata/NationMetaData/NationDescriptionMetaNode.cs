using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.NationMetaData;

/// <summary>
///     A metafile node that represents a mundane illustration.
/// </summary>
public sealed record NationDescriptionMetaNode : MetaNodeBase
{
    public string Description { get; }

    /// <inheritdoc />
    public NationDescriptionMetaNode(Nation nation)
        : base($"nation_{(int)nation}") =>
        Description = nation.ToString();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteUInt16(1); //1 property

        writer.WriteString16(Description);
    }
}