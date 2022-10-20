using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record MenuArgs : ISendArgs
{
    public string? Args { get; set; }
    public required EntityType EntityType { get; set; }
    public ICollection<ItemInfo>? Items { get; set; }
    public required MenuType MenuType { get; set; }
    public required string Name { get; set; }
    public ICollection<string>? Options { get; set; }
    public ushort? PursuitId { get; set; }
    public ICollection<SkillInfo>? Skills { get; set; }
    public ICollection<byte>? Slots { get; set; }
    public uint? SourceId { get; set; }
    public ICollection<SpellInfo>? Spells { get; set; }
    public required ushort Sprite { get; set; }
    public required string Text { get; set; }
}