using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.MundaneMetaData;

/// <summary>
///     A metadata node that represents a mundane illustration.
/// </summary>
public sealed record MundaneIllustrationMetaNode(string Name, string ImageName) : IMetaNode
{
    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteUInt16(1); //1 property

        writer.WriteString16(ImageName);
    }
}