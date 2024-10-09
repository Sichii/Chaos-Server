using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.ExitResponse" /> packet
/// </summary>
public sealed record ExitResponseArgs : IPacketSerializable
{
    /// <summary>
    ///     Whether or not the server is confirming that the client can exit
    /// </summary>
    public bool ExitConfirmed { get; set; }
}