using Chaos.Networking.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record MenuArgs : ISendArgs
{
    public string? Args { get; set; }
    public GameObjectType GameObjectType { get; set; }
    public ICollection<ItemInfo>? Items { get; set; }
    public MenuType MenuType { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<ushort>? Options { get; set; }
    public ushort? PursuitId { get; set; }
    public ICollection<SkillInfo>? Skills { get; set; }
    public ICollection<byte>? Slots { get; set; }
    public uint? SourceId { get; set; }
    public ICollection<SpellInfo>? Spells { get; set; }
    public ushort Sprite { get; set; }
    public string Text { get; set; } = null!;
}