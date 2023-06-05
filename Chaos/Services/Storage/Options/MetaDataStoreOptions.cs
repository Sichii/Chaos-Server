using Chaos.Common.Abstractions;
using Chaos.MetaData.Abstractions;
using Chaos.MetaData.ItemMetadata;

namespace Chaos.Services.Storage.Options;

public sealed class MetaDataStoreOptions : IDirectoryBound
{
    /// <summary>
    ///     The path to the event meta file
    /// </summary>
    public string EventMetaPath { get; set; } = null!;
    /// <summary>
    ///     The path to the mundane illustration meta file
    /// </summary>
    public string MundaneIllustrationMetaPath { get; set; } = null!;

    public ICollection<IMetaNodeMutator<ItemMetaNode>> PrefixMutators { get; } = new List<IMetaNodeMutator<ItemMetaNode>>();

    public ICollection<IMetaNodeMutator<ItemMetaNode>> SuffixMutators { get; } = new List<IMetaNodeMutator<ItemMetaNode>>();

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory)
    {
        EventMetaPath = Path.Combine(baseDirectory, EventMetaPath);
        MundaneIllustrationMetaPath = Path.Combine(baseDirectory, MundaneIllustrationMetaPath);
    }
}