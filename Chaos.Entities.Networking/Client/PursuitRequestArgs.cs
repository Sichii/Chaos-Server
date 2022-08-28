using Chaos.Common.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record PursuitRequestArgs(
    GameObjectType GameObjectType,
    uint ObjectId,
    ushort PursuitId,
    params string[]? Args
) : IReceiveArgs;