using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.SynchronizeTicks" /> packet
/// </summary>
public sealed record SynchronizeTicksResponseArgs : IPacketSerializable
{
    /// <summary>
    ///     The Environment.TickCount of the server
    /// </summary>
    public int Ticks { get; set; }
}