namespace Chaos.Options;

public record MapTemplateManagerOptions
{
    public string Directory { get; set; } = null!;
    public string MapDataDirectory { get; set; } = null!;
}