using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ClientWalkArgs(Direction Direction, byte StepCount) : IReceiveArgs;