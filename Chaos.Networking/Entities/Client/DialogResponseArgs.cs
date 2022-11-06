using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record DialogResponseArgs(
    EntityType EntityType,
    uint ObjectId,
    ushort PursuitId,
    ushort DialogId,
    DialogArgsType DialogArgsType,
    byte? Option,
    params string[]? Args
) : IReceiveArgs;