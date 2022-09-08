using System.IO;
using Chaos.Services.Caches.Abstractions;

namespace Chaos.Services.Caches.Options;

public class MapTemplateCacheOptions : FileCacheOptionsBase
{
    public string MapDataDirectory { get; set; } = null!;

    /// <inheritdoc />
    public override void UseRootDirectory(string rootDirectory)
    {
        base.UseRootDirectory(rootDirectory);
        MapDataDirectory = Path.Combine(rootDirectory, MapDataDirectory);
    }
}