#region
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.WorldMap" /> packet
/// </summary>
public sealed record WorldMapArgs : IPacketSerializable
{
    /// <summary>
    ///     The index of the image to use
    /// </summary>
    public byte FieldIndex { get; set; }

    /// <summary>
    ///     The name of the world map
    /// </summary>
    public string FieldName { get; set; } = null!;

    /// <summary>
    ///     A collection of clickable nodes to display on the world map
    /// </summary>
    public ICollection<WorldMapNodeInfo> Nodes { get; set; } = [];
}