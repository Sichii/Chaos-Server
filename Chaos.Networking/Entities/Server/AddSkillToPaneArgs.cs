using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record AddSkillToPaneArgs : ISendArgs
{
    public SkillInfo Skill { get; set; } = null!;
}