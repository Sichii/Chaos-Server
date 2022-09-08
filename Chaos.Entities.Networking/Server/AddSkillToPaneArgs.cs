using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record AddSkillToPaneArgs : ISendArgs
{
    public SkillInfo Skill { get; set; } = null!;
}