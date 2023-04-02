using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.NationMetaData;

/// <summary>
///     Represents a compressible collection of <see cref="NationDescriptionMetaNode" />s
/// </summary>
public sealed class NationDescriptionMetaData : MetaDataBase<NationDescriptionMetaNode>
{
    private const string NATION_DESCRIPTION_METAFILE_NAME = "NationDesc";

    /// <inheritdoc />
    public NationDescriptionMetaData()
        : base(NATION_DESCRIPTION_METAFILE_NAME) { }
}