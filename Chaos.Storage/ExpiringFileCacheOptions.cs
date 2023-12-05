using Chaos.Storage.Abstractions;
using Chaos.Storage.Abstractions.Definitions;

namespace Chaos.Storage;

/// <inheritdoc cref="IExpiringFileCacheOptions" />
public class ExpiringFileCacheOptions : IExpiringFileCacheOptions
{
    /// <inheritdoc />
    public string Directory { get; set; } = null!;

    /// <inheritdoc />
    public int ExpirationMins { get; init; }

    /// <inheritdoc />
    public string? FilePattern { get; init; }

    /// <inheritdoc />
    public bool Recursive { get; init; }

    /// <inheritdoc />
    public SearchType SearchType { get; init; }

    /// <inheritdoc />
    public virtual void UseBaseDirectory(string rootDirectory) => Directory = Path.Combine(rootDirectory, Directory);
}