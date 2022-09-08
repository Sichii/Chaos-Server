using Chaos.Geometry.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ClientWalkArgs(Direction Direction, byte StepCount) : IReceiveArgs;