using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.ClientRedirected" /> packet
/// </summary>
public sealed record ClientRedirectedArgs : IPacketSerializable
{
    /// <summary>
    ///     The id of the redirect
    /// </summary>
    public required uint Id { get; set; }

    /// <summary>
    ///     The new encryption key to be used
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    ///     The name associated with the redirection. Can be a player name or something else
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     The new encryption seed to be used
    /// </summary>
    public required byte Seed { get; set; }
}