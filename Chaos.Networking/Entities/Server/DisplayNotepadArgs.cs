#region
using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.DisplayNotepad" /> packet
/// </summary>
public sealed record DisplayNotepadArgs : IPacketSerializable
{
    /// <summary>
    ///     The height of the notepad
    /// </summary>
    /// <remarks>
    ///     In game, the notepad will display with a line height close to (1.4 * thisValue) with midpoint rouding
    /// </remarks>
    public byte Height { get; set; }

    /// <summary>
    ///     The message of the notepad
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    ///     The display type of the notepad
    /// </summary>
    public NotepadType NotepadType { get; set; }

    /// <summary>
    ///     The slot of the object that the notepad is attached to
    /// </summary>
    public byte Slot { get; set; }

    /// <summary>
    ///     The width of the notepad
    /// </summary>
    /// <remarks>
    ///     In game, the notepad will display with a character width close to (2.5 * thisValue) with midpoint rounding
    /// </remarks>
    public byte Width { get; set; }
}