using System.IO;

namespace Chaos.Services.Caches.Abstractions;

public abstract class FileCacheOptionsBase : IFileCacheOptions
{
    /// <inheritdoc />
    public string Directory { get; set; } = null!;
    /// <inheritdoc />
    public required SearchResultType SearchResultType { get; init; }
    /// <inheritdoc />
    public required string? FilePattern { get; init; }

    /// <inheritdoc />
    public virtual void UseRootDirectory(string rootDirectory) => Directory = Path.Combine(rootDirectory, Directory);
}