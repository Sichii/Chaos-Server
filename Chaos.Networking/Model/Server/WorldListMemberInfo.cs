namespace Chaos.Networking.Model.Server;

public record WorldListMemberInfo
{
    public BaseClass BaseClass { get; set; }
    public WorldListColor Color { get; set; }
    public bool IsMaster { get; set; }
    public string Name { get; set; } = null!;
    public SocialStatus SocialStatus { get; set; }
    public string? Title { get; set; }
}