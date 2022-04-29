using Chaos.Core.Geometry;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record GoldDropArgs(int Amount, Point DestinationPoint) : IReceiveArgs;