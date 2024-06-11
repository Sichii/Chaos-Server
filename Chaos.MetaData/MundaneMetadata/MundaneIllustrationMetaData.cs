using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.MundaneMetaData;

/// <summary>
///     Represents a compressible collection of <see cref="MundaneIllustrationMetaNode" />s
/// </summary>
public sealed class MundaneIllustrationMetaData : MetaDataBase<MundaneIllustrationMetaNode>
{
    private const string MUNDANE_ILLUSTRATION_METADATA_NAME = "NPCIllust";

    /// <inheritdoc />
    public MundaneIllustrationMetaData()
        : base(MUNDANE_ILLUSTRATION_METADATA_NAME) { }
}