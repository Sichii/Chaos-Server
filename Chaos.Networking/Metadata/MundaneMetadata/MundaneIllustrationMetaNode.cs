using Chaos.IO.Memory;
using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.MundaneMetadata;

/// <summary>
///     A metafile node that represents a mundane illustration.
/// </summary>
public sealed record MundaneIllustrationMetaNode : MetaNodeBase
{
    public string ImageName { get; }

    /// <inheritdoc />
    public MundaneIllustrationMetaNode(string name, string imageName)
        : base(name) => ImageName = imageName;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteUInt16(1); //1 property

        writer.WriteString16(ImageName);
    }
}