using Chaos.IO.Memory;
using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.LightMetaData;

/// <summary>
///     A node that stores metadata about what light type is used on a map.
/// </summary>
public class MapLightMetaNode(short mapId, string lightTypeName) : IMetaNode
{
    /// <summary>
    ///     The type of light used on the map
    /// </summary>
    public string LightType { get; init; } = lightTypeName;

    /// <summary>
    ///     The map id
    /// </summary>
    public string Name { get; init; } = mapId.ToString();

    /// <inheritdoc />
    public void Serialize(ref SpanWriter writer)
    {
        writer.WriteString8(Name);
        writer.WriteInt16(1);

        writer.WriteString16(LightType);
    }
}