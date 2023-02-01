using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.MundaneMetadata;

public sealed class MundaneIllustrationMetaData : MetaDataBase<MundaneIllustrationMetaNode>
{
    private const string MUNDANE_ILLUSTRATION_METAFILE_NAME = "NPCIllust";

    /// <inheritdoc />
    public MundaneIllustrationMetaData()
        : base(MUNDANE_ILLUSTRATION_METAFILE_NAME) { }
}