using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.ToggleGroup" /> packet
/// </summary>
public sealed record ToggleGroupArgs : IPacketSerializable;