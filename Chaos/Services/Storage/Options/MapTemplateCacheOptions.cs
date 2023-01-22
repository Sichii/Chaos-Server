using Chaos.Storage;

namespace Chaos.Services.Storage.Options;

public sealed class MapTemplateCacheOptions : ExpiringFileCacheOptions
{
    public string MapDataDirectory { get; set; } = null!;

    /// <inheritdoc />
    public override void UseBaseDirectory(string rootDirectory)
    {
        base.UseBaseDirectory(rootDirectory);
        MapDataDirectory = Path.Combine(rootDirectory, MapDataDirectory);
    }
}