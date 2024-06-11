using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.LightMetaData;

/// <summary>
/// </summary>
public sealed class LightMetaData : MetaDataBase<IMetaNode>
{
    /// <inheritdoc />
    public LightMetaData()
        : base("Light") { }
}