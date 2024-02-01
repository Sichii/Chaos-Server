using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.HomepageRequest" /> packet
/// </summary>
public sealed record HomepageRequestArgs : IPacketSerializable;