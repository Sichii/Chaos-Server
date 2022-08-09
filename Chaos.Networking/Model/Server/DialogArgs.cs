using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record DialogArgs : ISendArgs
{
    public ushort DialogId { get; set; }
    public DialogType DialogType { get; set; } = DialogType.CloseDialog;
    public GameObjectType GameObjectType { get; set; }
    public bool HasNextButton { get; set; }
    public bool HasPreviousButton { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<string>? Options { get; set; }
    public ushort? PursuitId { get; set; }
    public uint? SourceId { get; set; }
    public ushort Sprite { get; set; }
    public string Text { get; set; } = null!;
    public ushort? TextBoxLength { get; set; }
}