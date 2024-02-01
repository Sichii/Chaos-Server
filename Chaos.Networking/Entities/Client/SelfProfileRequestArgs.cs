using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.SelfProfileRequest" /> packet
/// </summary>
public sealed record SelfProfileRequestArgs : IPacketSerializable;