using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record WorldMapClickArgs(ushort NodeCheckSum, ushort MapId, IPoint Point) : IReceiveArgs;