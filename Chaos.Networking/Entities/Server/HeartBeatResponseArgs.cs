using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.HeartBeatResponse" /> packet
/// </summary>
public sealed record HeartBeatResponseArgs : ISendArgs
{
    /// <summary>
    ///     The first byte of the heartbeat response. This should be the secone byte of the heartbeat request
    /// </summary>
    public byte First { get; set; }
    /// <summary>
    ///     The second byte of the heartbeat response. This should be the first byte of the heartbeat request
    /// </summary>
    public byte Second { get; set; }
}