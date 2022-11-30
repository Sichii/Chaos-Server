namespace Chaos.Storage.Abstractions;

public interface IExpiringFileCacheOptions : ISimpleFileCacheOptions
{
    /// <summary>
    ///     The amount of time an entry in the cache will go unused before it is removed
    /// </summary>
    int ExpirationMins { get; init; }
}