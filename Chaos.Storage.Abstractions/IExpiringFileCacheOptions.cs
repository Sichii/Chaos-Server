namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines the properties required to configure a <see cref="Chaos.Storage.Abstractions.ExpiringFileCacheBase{T,TSchema}" />
/// </summary>
public interface IExpiringFileCacheOptions : ISimpleFileCacheOptions
{
    /// <summary>
    ///     The amount of time an entry in the cache will go unused before it is removed
    /// </summary>
    int ExpirationMins { get; init; }
}