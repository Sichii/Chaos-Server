using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record NotepadArgs : ISendArgs
{
    public byte Height { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotepadType NotepadType { get; set; }
    public byte Slot { get; set; }
    public byte Width { get; set; }
}