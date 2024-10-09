using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.MapInfo" /> packet
/// </summary>
public sealed record MapInfoArgs : IPacketSerializable
{
    /// <summary>
    ///     A checksum of the map data. If this value does not match what the client has, it will request the map data
    /// </summary>
    public ushort CheckSum { get; set; }

    /// <summary>
    ///     Flags that change how the map looks, and various other details
    /// </summary>
    public byte Flags { get; set; }

    /// <summary>
    ///     The height of the map
    /// </summary>
    public byte Height { get; set; }

    /// <summary>
    ///     The id of the map. The client will use this to store the map data. (e.g. if id was 500, it would be stored as
    ///     lod500.map)
    /// </summary>
    public short MapId { get; set; }

    /// <summary>
    ///     The name of the map
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The width of the map
    /// </summary>
    public byte Width { get; set; }
}