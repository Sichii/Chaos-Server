using Chaos.Common.Definitions;

namespace Chaos.Networking.Entities.Server;

public sealed record WorldListMemberInfo
{
    public BaseClass BaseClass { get; set; }
    public WorldListColor Color { get; set; }
    public bool IsMaster { get; set; }
    public string Name { get; set; } = null!;
    public SocialStatus SocialStatus { get; set; }
    public string? Title { get; set; }
}