#region
using Chaos.Common.Configuration;
using Chaos.Storage;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     Extensions for <see cref="IServiceCollection" />
/// </summary>
public static class StorageExtensions
{
    /// <summary>
    ///     Adds a singleton instance of <see cref="ExpiringFileCache{T,TSchema,TOptions}" /> to the service collection as an
    ///     implementation of <see cref="ISimpleCache{T}" />.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of object being cached.
    /// </typeparam>
    /// <typeparam name="TSchema">
    ///     The type of schema used to serialize/deserialize the cached object.
    /// </typeparam>
    /// <typeparam name="TOptions">
    ///     The type of options used to configure the cache.
    /// </typeparam>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add the services to.
    /// </param>
    /// <param name="optionsSubsection">
    ///     The optional subsection name of the options to bind.
    /// </param>
    public static void AddExpiringCache<T, TSchema, TOptions>(this IServiceCollection services, string? optionsSubsection = null)
        where T: class
        where TSchema: class
        where TOptions: class, IExpiringFileCacheOptions
    {
        services.AddOptionsFromConfig<TOptions>(optionsSubsection); //bound
        services.ConfigureOptions<DirectoryBoundOptionsConfigurer<TOptions>>();
        services.AddSingleton<ISimpleCache<T>, ExpiringFileCache<T, TSchema, TOptions>>();
    }

    /// <summary>
    ///     Adds an implementation of <see cref="ISimpleCache{T}" /> to the service collection with a specified
    ///     <typeparamref name="TImpl" /> type, configured with <typeparamref name="TOptions" /> options.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of object being cached.
    /// </typeparam>
    /// <typeparam name="TImpl">
    ///     The type of implementation of <see cref="ISimpleCache{T}" /> to add to the service collection.
    /// </typeparam>
    /// <typeparam name="TOptions">
    ///     The type of options used to configure the cache.
    /// </typeparam>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add the services to.
    /// </param>
    /// <param name="optionsSubsection">
    ///     The optional subsection name of the options to bind.
    /// </param>
    public static void AddExpiringCacheImpl<T, TImpl, TOptions>(this IServiceCollection services, string? optionsSubsection = null)
        where T: class
        where TImpl: class, ISimpleCache<T>
        where TOptions: class, IExpiringFileCacheOptions
    {
        services.AddOptionsFromConfig<TOptions>(optionsSubsection); //bound
        services.ConfigureOptions<DirectoryBoundOptionsConfigurer<TOptions>>();
        services.AddSingleton<ISimpleCache<T>, TImpl>();
    }

    /// <summary>
    ///     Adds an implementation of <see cref="IStorageManager" /> to the service collection. Also registers generic
    ///     <see cref="IReadOnlyStorage{T}" /> and <see cref="IStorage{T}" />
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add the services to
    /// </param>
    /// <param name="optionsSubsection">
    ///     The optional subsection name of the options to bind
    /// </param>
    public static void AddLocalStorage(this IServiceCollection services, string? optionsSubsection = null)
    {
        services.AddOptionsFromConfig<LocalStorageOptions>(optionsSubsection); //bound
        services.ConfigureOptions<DirectoryBoundOptionsConfigurer<LocalStorageOptions>>();
        services.AddSingleton<IStorageManager, LocalStorageManager>();
        services.AddSingleton(typeof(IReadOnlyStorage<>), typeof(ReadOnlyStorageObject<>));
        services.AddSingleton(typeof(IStorage<>), typeof(StorageObject<>));
    }
}