using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.ClientRedirected" /> packet
/// </summary>
/// <param name="Seed">The new encryption seed to be used</param>
/// <param name="Key">The new encryption key to be used</param>
/// <param name="Name">The name associated with the redirection. Can be a player name or something else</param>
/// <param name="Id">The id of the redirect</param>
public sealed record ClientRedirectedArgs(
    byte Seed,
    byte[] Key,
    string Name,
    uint Id
) : IReceiveArgs;