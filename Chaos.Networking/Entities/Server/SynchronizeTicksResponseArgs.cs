using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.SynchronizeTicksResponse" /> packet
/// </summary>
public sealed record SynchronizeTicksResponseArgs : IPacketSerializable
{
    /// <summary>
    ///     The Environment.TickCount of the server
    /// </summary>
    public int Ticks { get; set; }
}