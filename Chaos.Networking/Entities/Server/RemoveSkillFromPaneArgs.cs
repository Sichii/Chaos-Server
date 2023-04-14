using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.RemoveSkillFromPane" /> packet
/// </summary>
public sealed record RemoveSkillFromPaneArgs : ISendArgs
{
    /// <summary>
    ///     The slot of the skill to be removed
    /// </summary>
    public byte Slot { get; set; }
}