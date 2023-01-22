namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines the properties required to configure an expiring simple cache
/// </summary>
public interface IExpiringFileCacheOptions : ISimpleFileCacheOptions
{
    /// <summary>
    ///     The amount of time an entry in the cache will go unused before it is removed
    /// </summary>
    int ExpirationMins { get; init; }
}