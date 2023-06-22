using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.HeartBeat" />
///     packet
/// </summary>
/// <param name="First">The first byte (the client expects these bytes in reverse for it's response)</param>
/// <param name="Second">The second byte (the client expects these bytes in reverse for it's response)</param>
public sealed record HeartBeatArgs(byte First, byte Second) : IReceiveArgs;