namespace Chaos.Storage.Abstractions;

/// <inheritdoc cref="IExpiringFileCacheOptions" />
public abstract class ExpiringFileCacheOptionsBase : SimpleFileCacheOptionsBase, IExpiringFileCacheOptions
{
    /// <inheritdoc />
    public int ExpirationMins { get; init; }
}