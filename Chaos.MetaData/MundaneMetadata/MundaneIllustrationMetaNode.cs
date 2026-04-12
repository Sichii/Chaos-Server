#region
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;
#endregion

namespace Chaos.MetaData.MundaneMetaData;

/// <summary>
///     A metadata node that represents a mundane illustration. <paramref name="ImageNames" /> is the ordered list of
///     SPF filename variants the client will register for <paramref name="Name" /> — the first entry is variant 0,
///     the second is variant 1, and so on. Almost every NPC only needs a single filename; multi-variant entries are
///     rare.
/// </summary>
public sealed record MundaneIllustrationMetaNode(string Name, IReadOnlyList<string> ImageNames) : IMetaNode
{
    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteUInt16((ushort)ImageNames.Count);

        foreach (var imageName in ImageNames)
            writer.WriteString16(imageName);
    }
}