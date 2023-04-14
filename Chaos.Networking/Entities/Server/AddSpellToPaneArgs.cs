using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.AddSpellToPane" /> packet
/// </summary>
public sealed record AddSpellToPaneArgs : ISendArgs
{
    /// <summary>
    ///     Details about the spell being added to the pane
    /// </summary>
    public SpellInfo Spell { get; set; } = null!;
}