using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.RemoveSpellFromPane" /> packet
/// </summary>
public sealed record RemoveSpellFromPaneArgs : IPacketSerializable
{
    /// <summary>
    ///     The slot of the spell to be removed
    /// </summary>
    public byte Slot { get; set; }
}