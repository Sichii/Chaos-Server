using Chaos.Geometry.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record TurnArgs(Direction Direction) : IReceiveArgs;