using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.NationMetaData;

public sealed class NationDescriptionMetaData : MetaDataBase<NationDescriptionMetaNode>
{
    private const string NATION_DESCRIPTION_METAFILE_NAME = "NationDesc";

    /// <inheritdoc />
    public NationDescriptionMetaData()
        : base(NATION_DESCRIPTION_METAFILE_NAME) { }
}