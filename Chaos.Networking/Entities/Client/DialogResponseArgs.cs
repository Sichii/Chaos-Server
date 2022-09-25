using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record DialogResponseArgs(
    GameObjectType GameObjectType,
    uint ObjectId,
    ushort PursuitId,
    ushort DialogId,
    DialogArgsType? DialogArgsType,
    byte? Option,
    params string[]? Args
) : IReceiveArgs;