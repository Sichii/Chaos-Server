using Chaos.Storage.Abstractions;

namespace Chaos.Storage.Options;

public sealed class MapTemplateCacheOptions : SimpleFileCacheOptionsBase
{
    public string MapDataDirectory { get; set; } = null!;

    /// <inheritdoc />
    public override void UseBaseDirectory(string rootDirectory)
    {
        base.UseBaseDirectory(rootDirectory);
        MapDataDirectory = Path.Combine(rootDirectory, MapDataDirectory);
    }
}

public sealed class ExpiringMapTemplateCacheOptions : ExpiringFileCacheOptionsBase
{
    public string MapDataDirectory { get; set; } = null!;

    /// <inheritdoc />
    public override void UseBaseDirectory(string rootDirectory)
    {
        base.UseBaseDirectory(rootDirectory);
        MapDataDirectory = Path.Combine(rootDirectory, MapDataDirectory);
    }
}