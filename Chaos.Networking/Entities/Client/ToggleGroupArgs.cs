using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.ToggleGroup" />
///     packet
/// </summary>
public sealed record ToggleGroupArgs : IPacketSerializable;