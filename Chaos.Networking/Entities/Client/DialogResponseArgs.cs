using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.DialogResponse" /> packet
/// </summary>
/// <param name="EntityType">The type of the entity this dialog was generated from</param>
/// <param name="EntityId">The id of the entity this dialog was generated from</param>
/// <param name="PursuitId">The id of the pursuit this dialog is part of</param>
/// <param name="DialogId">
///     The id of the dialog being responded to. This id is offset depending on the action taken. <br />
///     +1 if the Next button was pressed, an Option was selected, or the dialog was otherwise progressed <br />
///     0 if the Close button was pressed <br />
///     -1 if the Previous button was pressed
/// </param>
/// <param name="DialogArgsType">The type of args contained in this dialog</param>
/// <param name="Option">The 1-based index of the option selected, if any</param>
/// <param name="Args">The extra string args contained in this response, if any</param>
public sealed record DialogResponseArgs(
    EntityType EntityType,
    uint EntityId,
    ushort PursuitId,
    ushort DialogId,
    DialogArgsType DialogArgsType,
    byte? Option,
    List<string>? Args
) : IReceiveArgs;