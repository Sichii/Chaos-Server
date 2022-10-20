using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record ClientWalkArgs(Direction Direction, byte StepCount) : IReceiveArgs;