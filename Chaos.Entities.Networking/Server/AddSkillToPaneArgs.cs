using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Server;

public record AddSkillToPaneArgs : ISendArgs
{
    public SkillInfo Skill { get; set; } = null!;
}