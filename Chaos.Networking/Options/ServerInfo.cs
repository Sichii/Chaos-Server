namespace Chaos.Networking.Options;

public record ServerInfo : RedirectInfo
{
    public string Description { get; set; } = null!;
    public byte Id { get; set; } = 0;
    public string Name { get; set; } = null!;
}