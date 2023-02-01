using Chaos.Common.Abstractions;
using Chaos.Networking.Metadata.ItemMetadata;

namespace Chaos.Services.Storage.Abstractions;

public interface IMetaDataCacheOptions : IDirectoryBound
{
    string EventMetaPath { get; set; }
    string MundaneIllustrationMetaPath { get; set; }

    ICollection<IMetaNodeMutator<ItemMetaNode>> PrefixMutators { get; }
    ICollection<IMetaNodeMutator<ItemMetaNode>> SuffixMutators { get; }
}