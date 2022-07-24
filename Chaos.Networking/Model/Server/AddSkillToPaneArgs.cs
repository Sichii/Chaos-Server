using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record AddSkillToPaneArgs : ISendArgs
{
    public SkillInfo Skill { get; set; } = null!;
}