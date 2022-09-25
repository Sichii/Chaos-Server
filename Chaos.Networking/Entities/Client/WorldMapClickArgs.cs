using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record WorldMapClickArgs(ushort NodeCheckSum, ushort MapId, IPoint Point) : IReceiveArgs;