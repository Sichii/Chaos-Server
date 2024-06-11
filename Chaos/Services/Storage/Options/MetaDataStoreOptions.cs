using Chaos.Common.Abstractions;
using Chaos.MetaData.Abstractions;

namespace Chaos.Services.Storage.Options;

public sealed class MetaDataStoreOptions : IDirectoryBound
{
    /// <summary>
    ///     The path to the event meta file
    /// </summary>
    public string EventMetaPath { get; set; } = null!;

    /// <summary>
    ///     The path to the light meta file
    /// </summary>
    public string LightMetaPath { get; set; } = null!;

    /// <summary>
    ///     The path to the mundane illustration meta file
    /// </summary>
    public string MundaneIllustrationMetaPath { get; set; } = null!;

    public ICollection<IItemMetaNodeMutator> PrefixMutators { get; } = new List<IItemMetaNodeMutator>();

    public ICollection<IItemMetaNodeMutator> SuffixMutators { get; } = new List<IItemMetaNodeMutator>();

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory)
    {
        EventMetaPath = Path.Combine(baseDirectory, EventMetaPath);
        LightMetaPath = Path.Combine(baseDirectory, LightMetaPath);
        MundaneIllustrationMetaPath = Path.Combine(baseDirectory, MundaneIllustrationMetaPath);
    }
}