using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.ForceClientPacket" /> packet
/// </summary>
public sealed record ForceClientPacketArgs : IPacketSerializable
{
    /// <summary>
    ///     The opcode of the packet the server is forcing the client to send back to it
    /// </summary>
    public ClientOpCode ClientOpCode { get; set; }

    /// <summary>
    ///     The data of the packet the server is forcing the client to send back to it
    /// </summary>
    public byte[] Data { get; set; } = [];
}