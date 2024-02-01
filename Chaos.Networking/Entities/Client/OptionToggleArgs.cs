using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.OptionToggle" /> packet
/// </summary>
public sealed record OptionToggleArgs : IPacketSerializable
{
    /// <summary>
    ///     The option the client is trying to toggle
    /// </summary>
    public required UserOption UserOption { get; set; }
}