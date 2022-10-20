using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record WorldMapClickArgs(ushort UniqueId, ushort MapId, IPoint Point) : IReceiveArgs;