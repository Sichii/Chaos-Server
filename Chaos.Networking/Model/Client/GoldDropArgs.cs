using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record GoldDropArgs(int Amount, IPoint DestinationPoint) : IReceiveArgs;