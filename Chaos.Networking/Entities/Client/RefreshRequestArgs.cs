using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.RefreshRequest" /> packet
/// </summary>
public sealed record RefreshRequestArgs : IPacketSerializable;