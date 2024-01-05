using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Login" /> packet
/// </summary>
public sealed record LoginArgs : IPacketSerializable
{
    /// <summary>
    ///     The name of the aisling the client is trying to log in as
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     The password of the aisling the client is trying to log in as
    /// </summary>
    public required string Password { get; set; }
}