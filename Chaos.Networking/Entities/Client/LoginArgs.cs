#region
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.Login" /> packet
/// </summary>
public sealed record LoginArgs : IPacketSerializable
{
    /// <summary>
    ///     The CRC16 checksum of <see cref="ClientId1" /> (little-endian bytes)
    /// </summary>
    public required ushort Checksum1 { get; set; }

    /// <summary>
    ///     A unique identifier for the client that is unique to the installation
    /// </summary>
    public required uint ClientId1 { get; set; }

    /// <summary>
    ///     A unique identifier for the client that is randomly generated each login
    /// </summary>
    public required uint ClientId2 { get; set; }

    /// <summary>
    ///     A CRC16 integrity check computed over the 12 encrypted payload bytes (key1 + encodedKey2 + encryptedClientId1 +
    ///     encryptedChecksum1 + encryptedClientId2)
    /// </summary>
    public required ushort IntegrityCrc { get; set; }

    /// <summary>
    ///     The name of the aisling the client is trying to log in as
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     The password of the aisling the client is trying to log in as
    /// </summary>
    public required string Password { get; set; }
}