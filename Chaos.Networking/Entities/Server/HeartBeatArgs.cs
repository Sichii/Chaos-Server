#region
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.HeartBeat" /> packet
/// </summary>
public sealed record HeartBeatArgs : IPacketSerializable
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