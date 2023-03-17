using Chaos.Storage;
using Chaos.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class StorageExtensions
{
    public static void AddExpiringCache<T, TSchema, TOptions>(this IServiceCollection services, string? optionsSubsection = null)
        where T: class where TSchema: class where TOptions: class, IExpiringFileCacheOptions
    {
        services.AddDirectoryBoundOptionsFromConfig<TOptions>(optionsSubsection);
        services.AddSingleton<ISimpleCache<T>, ExpiringFileCache<T, TSchema, TOptions>>();
    }

    public static void AddExpiringCacheImpl<T, TImpl, TOptions>(this IServiceCollection services, string? optionsSubsection = null)
        where T: class where TImpl: class, ISimpleCache<T> where TOptions: class, IExpiringFileCacheOptions
    {
        services.AddDirectoryBoundOptionsFromConfig<TOptions>(optionsSubsection);
        services.AddSingleton<ISimpleCache<T>, TImpl>();
    }
}