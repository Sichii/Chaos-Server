using Chaos.Geometry.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ClientWalkArgs(Direction Direction, byte StepCount) : IReceiveArgs;