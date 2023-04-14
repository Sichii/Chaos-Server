using Chaos.Geometry;
using Chaos.Geometry.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a world map node in the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.WorldMap" /> packet
/// </summary>
public record WorldMapNodeInfo
{
    /// <summary>
    ///     The destination of the node the character will be teleported to
    /// </summary>
    public Location Destination { get; set; }
    /// <summary>
    ///     The X and Y screen coordinates where the node will be displayed on the world map (the screen is 640x480)
    /// </summary>
    public IPoint ScreenPosition { get; set; } = null!;
    /// <summary>
    ///     The text displayed on the node
    /// </summary>
    public string Text { get; set; } = null!;
    /// <summary>
    ///     The unique id of the node (or checksum, depends how you want to use it)
    /// </summary>
    public ushort UniqueId { get; set; }
}