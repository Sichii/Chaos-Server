using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.SynchronizeTicks" /> packet
/// </summary>
public sealed record SynchronizeTicksArgs : IPacketSerializable
{
    /// <summary>
    ///     The ticks the client is using
    /// </summary>
    public required uint ClientTicks { get; set; }

    /// <summary>
    ///     The ticks the client thinks the server is using
    /// </summary>
    public required uint ServerTicks { get; set; }
}