namespace Chaos.Networking.Entities.Server;

public sealed record SkillInfo
{
    public string Name { get; set; } = null!;
    public byte Slot { get; set; }
    public ushort Sprite { get; set; }
}