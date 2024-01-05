using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Profile" />
///     packet
/// </summary>
public sealed record ProfileArgs : IPacketSerializable
{
    /// <summary>
    ///     The data of the client's custom portrait
    /// </summary>
    public required byte[] PortraitData { get; set; }

    /// <summary>
    ///     The text in the client's custom profile
    /// </summary>
    public required string ProfileMessage { get; set; }
}