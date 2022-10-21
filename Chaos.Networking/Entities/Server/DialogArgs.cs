using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record DialogArgs : ISendArgs
{
    public required ushort DialogId { get; set; }
    public required DialogType DialogType { get; set; }
    public required EntityType EntityType { get; set; }
    public required bool HasNextButton { get; set; }
    public required bool HasPreviousButton { get; set; }
    public required string Name { get; set; } = null!;
    public ICollection<string>? Options { get; set; }
    public ushort? PursuitId { get; set; }
    public uint? SourceId { get; set; }
    public required ushort Sprite { get; set; }
    public required string Text { get; set; } = null!;
    public ushort? TextBoxLength { get; set; }
}