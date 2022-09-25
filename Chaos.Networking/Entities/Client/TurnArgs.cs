using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record TurnArgs(Direction Direction) : IReceiveArgs;