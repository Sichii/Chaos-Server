using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record PursuitRequestArgs(
    EntityType EntityType,
    uint ObjectId,
    ushort PursuitId,
    params string[]? Args
) : IReceiveArgs;