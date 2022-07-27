namespace Chaos.Services.Caches.Options;

public record MapTemplateCacheOptions
{
    public string Directory { get; set; } = null!;
    public string MapDataDirectory { get; set; } = null!;
}