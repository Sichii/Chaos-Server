using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record PursuitRequestArgs(
    GameObjectType GameObjectType,
    uint ObjectId,
    ushort PursuitId,
    params string[]? Args
) : IReceiveArgs;