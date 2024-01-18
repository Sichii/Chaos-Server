namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines the properties required to configure an expiring simple cache
/// </summary>
public interface IExpiringFileCacheOptions : ISimpleFileCacheOptions
{
    /// <summary>
    ///     Default null. The amount of time an entry in the cache will go unused before it is removed. (Only applies if
    ///     Expires is set to true)
    /// </summary>
    int? ExpirationMins { get; set; }

    /// <summary>
    ///     Whether or not entities added to the cache will ever expire
    /// </summary>
    bool Expires { get; set; }
}