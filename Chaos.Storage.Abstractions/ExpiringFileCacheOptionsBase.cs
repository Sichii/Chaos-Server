namespace Chaos.Storage.Abstractions;

public abstract class ExpiringFileCacheOptionsBase : SimpleFileCacheOptionsBase, IExpiringFileCacheOptions
{
    /// <inheritdoc />
    public int ExpirationMins { get; init; }
}