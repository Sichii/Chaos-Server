using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record WorldMapClickArgs(ushort NodeCheckSum, ushort MapId, Point Point) : IReceiveArgs;