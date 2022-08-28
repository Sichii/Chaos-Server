using Chaos.Geometry.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record TurnArgs(Direction Direction) : IReceiveArgs;