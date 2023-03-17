using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Microsoft.Extensions.DependencyInjection" /> extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a singleton service that can be retreived via multiple base types
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <typeparam name="TI1">A base type of <typeparamref name="T" /></typeparam>
    /// <typeparam name="TI2">Another base type of <typeparamref name="T" /></typeparam>
    /// <typeparam name="T">An implementation of the previous two types</typeparam>
    public static void AddSingleton<TI1, TI2, T>(this IServiceCollection services) where T: class, TI1, TI2
                                                                                   where TI1: class
                                                                                   where TI2: class
    {
        services.AddSingleton<TI1, T>();
        services.AddSingleton<TI2, T>(p => (T)p.GetRequiredService<TI1>());
    }
}