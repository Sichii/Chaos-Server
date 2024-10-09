using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.Notepad" /> packet
/// </summary>
public sealed record NotepadArgs : IPacketSerializable
{
    /// <summary>
    ///     The height of the notepad
    /// </summary>
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
    public byte Width { get; set; }
}