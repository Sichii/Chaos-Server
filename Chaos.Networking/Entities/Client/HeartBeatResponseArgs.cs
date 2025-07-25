#region
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.HeartBeatResponse" /> packet
/// </summary>
public sealed record HeartBeatResponseArgs : IPacketSerializable
{
    /// <summary>
    ///     The first byte (this should match the second byte of the server's request)
    /// </summary>
    public required byte First { get; set; }

    /// <summary>
    ///     The second byte (this should match the first byte of the server's request)
    /// </summary>
    public required byte Second { get; set; }
}