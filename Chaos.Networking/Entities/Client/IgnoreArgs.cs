using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Ignore" />
///     packet
/// </summary>
public sealed record IgnoreArgs : IPacketSerializable
{
    /// <summary>
    ///     The action the client is performing in the ignore window
    /// </summary>
    public required IgnoreType IgnoreType { get; set; }

    /// <summary>
    ///     The name of the player the action is targeted at
    /// </summary>
    public string? TargetName { get; set; }
}