using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.RemoveSpellFromPane" /> packet
/// </summary>
public sealed record RemoveSpellFromPaneArgs : ISendArgs
{
    /// <summary>
    ///     The slot of the spell to be removed
    /// </summary>
    public byte Slot { get; set; }
}