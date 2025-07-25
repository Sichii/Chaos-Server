#region
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.SynchronizeTicks" /> packet
/// </summary>
public sealed record SynchronizeTicksArgs : IPacketSerializable
{
    /// <summary>
    ///     The Environment.TickCount of the server
    /// </summary>
    public uint Ticks { get; set; }
}