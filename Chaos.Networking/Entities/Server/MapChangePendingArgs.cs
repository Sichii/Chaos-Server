using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.MapChangePending" /> packet
/// </summary>
public sealed record MapChangePendingArgs : IPacketSerializable;