using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.ClientWalkResponse" /> packet
/// </summary>
public sealed record ClientWalkResponseArgs : IPacketSerializable
{
    /// <summary>
    ///     The direction for the client to walk
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    ///     The point the client is supposed to be walking from
    /// </summary>
    public IPoint OldPoint { get; set; } = null!;
}