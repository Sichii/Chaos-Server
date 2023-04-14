using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.ConfirmClientWalk" /> packet
/// </summary>
public sealed record ConfirmClientWalkArgs : ISendArgs
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