using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record ItemDropArgs(byte SourceSlot, IPoint DestinationPoint, int Count) : IReceiveArgs;