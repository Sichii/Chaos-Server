using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a world map node in the <see cref="ServerOpCode.WorldMap" /> packet
/// </summary>
public sealed record WorldMapNodeInfo
{
    /// <summary>
    ///     The unique id of the node (or checksum, depends how you want to use it)
    /// </summary>
    public ushort CheckSum { get; set; }

    /// <summary>
    ///     The destination point on the map
    /// </summary>
    public Point DestinationPoint { get; set; }

    /// <summary>
    ///     The destination map id
    /// </summary>
    public ushort MapId { get; set; }

    /// <summary>
    ///     The X and Y screen coordinates where the node will be displayed on the world map (the screen is 640x480)
    /// </summary>
    public IPoint ScreenPosition { get; set; } = null!;

    /// <summary>
    ///     The text displayed on the node
    /// </summary>
    public string Text { get; set; } = null!;
}