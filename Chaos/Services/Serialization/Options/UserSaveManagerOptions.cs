namespace Chaos.Services.Serialization.Options;

public record UserSaveManagerOptions
{
    public string Directory { get; set; } = null!;
}