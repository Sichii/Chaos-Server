using Chaos.Storage.Abstractions.Definitions;

namespace Chaos.Storage.Abstractions;

/// <inheritdoc />
public abstract class SimpleFileCacheOptionsBase : ISimpleFileCacheOptions
{
    /// <inheritdoc />
    public required string Directory { get; set; }
    /// <inheritdoc />
    public string? FilePattern { get; init; }
    /// <inheritdoc />
    public bool Recursive { get; init; }
    /// <inheritdoc />
    public required SearchType SearchType { get; init; }

    /// <inheritdoc />
    public virtual void UseBaseDirectory(string rootDirectory) => Directory = Path.Combine(rootDirectory, Directory);
}