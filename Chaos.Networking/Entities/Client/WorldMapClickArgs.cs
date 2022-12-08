using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record WorldMapClickArgs(
    ushort UniqueId,
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not needed / Not trusted")]
    ushort MapId,
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not needed / Not trusted")]
    IPoint Point
) : IReceiveArgs;