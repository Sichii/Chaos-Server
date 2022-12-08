using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record DialogResponseArgs(
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not needed / Not trusted")]
    EntityType EntityType,
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not needed / Not trusted")]
    uint ObjectId,
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not needed / Not trusted")]
    ushort PursuitId,
    ushort DialogId,
    [SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Not needed / Not trusted")]
    DialogArgsType DialogArgsType,
    byte? Option,
    params string[]? Args
) : IReceiveArgs;