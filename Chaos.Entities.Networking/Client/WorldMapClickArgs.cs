using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record WorldMapClickArgs(ushort NodeCheckSum, ushort MapId, IPoint Point) : IReceiveArgs;