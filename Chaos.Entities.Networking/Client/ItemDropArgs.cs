using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ItemDropArgs(byte SourceSlot, IPoint DestinationPoint, int Count) : IReceiveArgs;