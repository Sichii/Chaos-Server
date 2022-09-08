using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record GoldDropArgs(int Amount, IPoint DestinationPoint) : IReceiveArgs;