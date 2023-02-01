using Chaos.Networking.Metadata.ItemMetadata;
using Chaos.Services.Storage.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class MetaDataCacheOptions : IMetaDataCacheOptions
{
    /// <inheritdoc />
    public string EventMetaPath { get; set; } = null!;
    /// <inheritdoc />
    public string MundaneIllustrationMetaPath { get; set; } = null!;
    /// <inheritdoc />
    public ICollection<IMetaNodeMutator<ItemMetaNode>> PrefixMutators { get; } = new List<IMetaNodeMutator<ItemMetaNode>>();
    /// <inheritdoc />
    public ICollection<IMetaNodeMutator<ItemMetaNode>> SuffixMutators { get; } = new List<IMetaNodeMutator<ItemMetaNode>>();

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory)
    {
        EventMetaPath = Path.Combine(baseDirectory, EventMetaPath);
        MundaneIllustrationMetaPath = Path.Combine(baseDirectory, MundaneIllustrationMetaPath);
    }
}