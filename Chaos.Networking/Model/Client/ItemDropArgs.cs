using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ItemDropArgs(byte SourceSlot, Point DestinationPoint, int Count) : IReceiveArgs;