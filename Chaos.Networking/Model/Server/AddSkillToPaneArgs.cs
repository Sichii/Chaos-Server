using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record AddSkillToPaneArgs : ISendArgs
{
    public SkillArg Skill { get; set; } = null!;
}