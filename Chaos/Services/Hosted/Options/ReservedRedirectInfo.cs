namespace Chaos.Services.Hosted.Options;

public record ReservedRedirectInfo
{
    public byte Id { get; set; }
    public string Name { get; set; } = null!;

    public override string ToString() => $"Id: {Id}, Name: {Name}";
}