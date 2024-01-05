using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.CreateCharRequest" /> packet
/// </summary>
public sealed record CreateCharRequestArgs : IPacketSerializable
{
    /// <summary>
    ///     The name of the character
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     The password of the character
    /// </summary>
    public required string Password { get; set; }
}