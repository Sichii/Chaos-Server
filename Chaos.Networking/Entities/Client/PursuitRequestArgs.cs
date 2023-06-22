using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.PursuitRequest" /> packet
/// </summary>
/// <param name="EntityType">The type of the entity from which the dialog was generated</param>
/// <param name="EntityId">The id of the entity from which the dialog was generated</param>
/// <param name="PursuitId">The id of the pursuit selected by the client</param>
/// <param name="Args">
///     If specified, any lingering arguments that were passed into the dialog, and any input from the
///     player in a text box
/// </param>
public sealed record PursuitRequestArgs(
    EntityType EntityType,
    uint EntityId,
    ushort PursuitId,
    params string[]? Args
) : IReceiveArgs;