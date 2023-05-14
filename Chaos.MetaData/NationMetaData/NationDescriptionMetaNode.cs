using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.NationMetaData;

/// <summary>
///     A metadata node that represents a mundane illustration.
/// </summary>
public sealed record NationDescriptionMetaNode : IMetaNode
{
    /// <summary>
    ///     The text that shows next to the nation heraldry in the profile
    /// </summary>
    public string Description { get; }
    /// <summary>
    ///     The name of the nation
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NationDescriptionMetaNode" /> class.
    /// </summary>
    public NationDescriptionMetaNode(Nation nation)
    {
        Name = $"nation_{(int)nation}";
        Description = nation.ToString();
    }

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteUInt16(1); //1 property

        writer.WriteString16(Description);
    }
}