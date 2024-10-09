using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.SelfProfileRequest" /> packet
/// </summary>
public sealed record SelfProfileRequestArgs : IPacketSerializable;