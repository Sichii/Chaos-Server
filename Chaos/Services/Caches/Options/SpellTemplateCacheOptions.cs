namespace Chaos.Services.Caches.Options;

public record SpellTemplateCacheOptions
{
    public string Directory { get; set; } = null!;
}