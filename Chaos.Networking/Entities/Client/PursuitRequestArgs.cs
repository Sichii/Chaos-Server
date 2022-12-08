using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record PursuitRequestArgs(
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not needed / Not trusted")]
    EntityType EntityType,
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not needed / Not trusted")]
    uint ObjectId,
    ushort PursuitId,
    params string[]? Args
) : IReceiveArgs;