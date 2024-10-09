using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.ClientWalk" /> packet
/// </summary>
public sealed record ClientWalkArgs : IPacketSerializable
{
    /// <summary>
    ///     The direction the client is walking
    /// </summary>
    public required Direction Direction { get; set; }

    /// <summary>
    ///     The number of steps taken. this number rolls over when it caps out at 255.
    /// </summary>
    public required byte StepCount { get; set; }
}