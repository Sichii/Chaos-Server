using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record ItemDropArgs(byte SourceSlot, IPoint DestinationPoint, int Count) : IReceiveArgs;