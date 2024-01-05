using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.PursuitRequest" /> packet
/// </summary>
public sealed record PursuitRequestArgs : IPacketSerializable
{
    /// <summary>
    ///     If specified, any lingering arguments that were passed into the dialog, and any input from the player in a text box
    /// </summary>
    public string[]? Args { get; set; }

    /// <summary>
    ///     The id of the entity from which the dialog was generated
    /// </summary>
    public required uint EntityId { get; set; }

    /// <summary>
    ///     The type of the entity from which the dialog was generated
    /// </summary>
    public required EntityType EntityType { get; set; }

    /// <summary>
    ///     The id of the pursuit selected by the client
    /// </summary>
    public required ushort PursuitId { get; set; }

    /// <summary>
    ///     If specified, the slot of the item the client is clicking on in the dialog
    /// </summary>
    public byte? Slot { get; set; }
}