using Chaos.Common.Abstractions;
using Chaos.MetaData.ItemMetadata;

namespace Chaos.Services.Storage.Abstractions;

public interface IMetaDataCacheOptions : IDirectoryBound
{
    /// <summary>
    ///     The path to the event meta file
    /// </summary>
    string EventMetaPath { get; set; }
    /// <summary>
    ///     The path to the mundane illustration meta file
    /// </summary>
    string MundaneIllustrationMetaPath { get; set; }

    ICollection<IMetaNodeMutator<ItemMetaNode>> PrefixMutators { get; }
    ICollection<IMetaNodeMutator<ItemMetaNode>> SuffixMutators { get; }
}