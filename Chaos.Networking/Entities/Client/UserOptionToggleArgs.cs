using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.UserOptionToggle" /> packet
/// </summary>
public sealed record UserOptionToggleArgs : IPacketSerializable
{
    /// <summary>
    ///     The option the client is trying to toggle
    /// </summary>
    public required UserOption UserOption { get; set; }
}