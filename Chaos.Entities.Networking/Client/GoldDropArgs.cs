using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record GoldDropArgs(int Amount, IPoint DestinationPoint) : IReceiveArgs;