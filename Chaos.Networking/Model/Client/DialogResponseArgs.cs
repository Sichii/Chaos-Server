using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record DialogResponseArgs(
    GameObjectType GameObjectType,
    uint ObjectId,
    ushort PursuitId,
    ushort DialogId,
    DialogArgsType? DialogArgsType,
    byte? Option,
    params string[]? Args
) : IReceiveArgs;