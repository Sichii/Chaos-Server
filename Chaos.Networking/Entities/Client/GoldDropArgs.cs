using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record GoldDropArgs(int Amount, IPoint DestinationPoint) : IReceiveArgs;