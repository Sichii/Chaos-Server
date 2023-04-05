using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.ClientWalk" /> packet
/// </summary>
/// <param name="Direction">The direction the client is walking</param>
/// <param name="StepCount">The number of steps taken. this number rolls over when it caps out at 255.</param>
public sealed record ClientWalkArgs(Direction Direction, byte StepCount) : IReceiveArgs;