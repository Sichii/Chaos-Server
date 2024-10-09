using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a spell in the <see cref="ServerOpCode.AddSpellToPane" /> and
///     <see cref="ServerOpCode.DisplayMenu" /> packets
/// </summary>
public sealed record SpellInfo
{
    /// <summary>
    ///     The number of castlines the spell has
    /// </summary>
    public byte CastLines { get; set; }

    /// <summary>
    ///     The name of the spell
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The text that appears when you hover this spell on the spell panel
    /// </summary>
    public string PanelName { get; set; } = null!;

    /// <summary>
    ///     If the spell has a prompt, this is that prompt
    /// </summary>
    public string Prompt { get; set; } = null!;

    /// <summary>
    ///     The slot the spell is in
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    ///     The type of spell
    /// </summary>
    public SpellType SpellType { get; set; }

    /// <summary>
    ///     The sprite of the spell icon
    /// </summary>
    public ushort Sprite { get; set; }
}