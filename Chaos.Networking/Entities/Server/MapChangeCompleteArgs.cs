using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.MapChangeComplete" /> packet
/// </summary>
public sealed record MapChangeCompleteArgs : IPacketSerializable;