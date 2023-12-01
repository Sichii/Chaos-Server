using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Abstractions;
using Chaos.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a simple factory wrapper for <typeparamref name="T" /> to the service collection.
    /// </summary>
    /// <param name="services">This service collection</param>
    /// <param name="argTypes">A collection of types required to construct an instance</param>
    /// <typeparam name="T">The type to create</typeparam>
    [ExcludeFromCodeCoverage(Justification = "Nothing to test, just a shorthand")]
    public static IServiceCollection AddSimpleFactory<T>(this IServiceCollection services, params Type[] argTypes)
        where T: class
    {
        var runtimeFactory = ActivatorUtilities.CreateFactory(typeof(T), argTypes);
        services.AddSingleton<IFactory<T>, Factory<T>>(sp => new Factory<T>(sp, runtimeFactory));

        return services;
    }

    /// <summary>
    ///     Adds a simple factory wrapper for <typeparamref name="T" /> to the service collection.
    /// </summary>
    /// <param name="services">This service collection</param>
    /// <param name="argTypes">A collection of types required to construct an instance</param>
    /// <typeparam name="T">The type of the service to return</typeparam>
    /// <typeparam name="TImpl">The implementation type of the service to create</typeparam>
    [ExcludeFromCodeCoverage(Justification = "Nothing to test, just a shorthand")]
    public static IServiceCollection AddSimpleFactory<T, TImpl>(this IServiceCollection services, params Type[] argTypes)
        where T: class
    {
        var runtimeFactory = ActivatorUtilities.CreateFactory(typeof(TImpl), argTypes);
        services.AddSingleton<IFactory<T>, Factory<T>>(sp => new Factory<T>(sp, runtimeFactory));

        return services;
    }
}