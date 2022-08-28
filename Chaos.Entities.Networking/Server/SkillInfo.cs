namespace Chaos.Entities.Networking.Server;

public record SkillInfo
{
    public string Name { get; set; } = null!;
    public byte Slot { get; set; }
    public ushort Sprite { get; set; }
}