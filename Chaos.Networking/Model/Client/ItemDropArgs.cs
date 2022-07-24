using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ItemDropArgs(byte SourceSlot, IPoint DestinationPoint, int Count) : IReceiveArgs;