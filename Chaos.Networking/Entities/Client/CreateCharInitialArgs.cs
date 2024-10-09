using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.CreateCharInitial" /> packet
/// </summary>
public sealed record CreateCharInitialArgs : IPacketSerializable
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