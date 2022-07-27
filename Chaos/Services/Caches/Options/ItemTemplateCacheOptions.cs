namespace Chaos.Services.Caches.Options;

public record ItemTemplateCacheOptions
{
    public string Directory { get; set; } = null!;
}