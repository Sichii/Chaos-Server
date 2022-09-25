using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record AddSkillToPaneArgs : ISendArgs
{
    public SkillInfo Skill { get; set; } = null!;
}