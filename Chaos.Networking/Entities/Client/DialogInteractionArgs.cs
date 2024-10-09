using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.DialogInteraction" /> packet
/// </summary>
public sealed record DialogInteractionArgs : IPacketSerializable
{
    /// <summary>
    ///     The extra string args contained in this response, if any
    /// </summary>
    public List<string>? Args { get; set; }

    /// <summary>
    ///     The type of args contained in this dialog
    /// </summary>
    public DialogArgsType DialogArgsType { get; set; }

    /// <summary>
    ///     The id of the dialog being responded to. This id is offset depending on the action taken.
    ///     <br />
    ///     +1 if the Next button was pressed, an Option was selected, or the dialog was otherwise progressed
    ///     <br />
    ///     0 if the Close button was pressed
    ///     <br />
    ///     -1 if the Previous button was pressed
    /// </summary>
    public required ushort DialogId { get; set; }

    /// <summary>
    ///     The id of the entity this dialog was generated from
    /// </summary>
    public required uint EntityId { get; set; }

    /// <summary>
    ///     The type of the entity this dialog was generated from
    /// </summary>
    public required EntityType EntityType { get; set; }

    /// <summary>
    ///     The 1-based index of the option selected, if any
    /// </summary>
    public byte? Option { get; set; }

    /// <summary>
    ///     The id of the pursuit this dialog is part of
    /// </summary>
    public required ushort PursuitId { get; set; }
}