using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record WorldMapClickArgs(ushort NodeCheckSum, ushort MapId, IPoint Point) : IReceiveArgs;