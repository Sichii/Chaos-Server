using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Common" /> DI extensions
/// </summary>
[ExcludeFromCodeCoverage]
public static class CommonExtensions
{
    /// <summary>
    ///     Adds an option object based on a configuration section to the service collection
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the options object to
    /// </param>
    /// <param name="subSection">
    ///     If the section is not at the root level, supply the subsection here
    /// </param>
    /// <param name="optionsSection">
    ///     If the options section is not the same as the class name, supply the name of that section here
    /// </param>
    /// <typeparam name="T">
    ///     The type of the options object
    /// </typeparam>
    public static OptionsBuilder<T> AddOptionsFromConfig<T>(
        this IServiceCollection services,
        string? subSection = null,
        string? optionsSection = null) where T: class
    {
        var path = optionsSection ?? typeof(T).Name;

        if (!string.IsNullOrWhiteSpace(subSection))
            path = $"{subSection}:{path}";

        return services.AddOptions<T>()
                       .BindConfiguration(path, o => o.ErrorOnUnknownConfiguration = true);
    }

    /// <summary>
    ///     Adds a singleton service that can be retreived via multiple base types
    /// </summary>
    /// <param name="services">
    ///     The service collection to add to
    /// </param>
    /// <typeparam name="TI1">
    ///     A base type of <typeparamref name="T" />
    /// </typeparam>
    /// <typeparam name="TI2">
    ///     Another base type of <typeparamref name="T" />
    /// </typeparam>
    /// <typeparam name="T">
    ///     An implementation of the previous two types
    /// </typeparam>
    public static void AddSingleton<TI1, TI2, T>(this IServiceCollection services) where T: class, TI1, TI2
                                                                                   where TI1: class
                                                                                   where TI2: class
    {
        services.AddSingleton<TI1, T>();
        services.AddSingleton<TI2, T>(p => (T)p.GetRequiredService<TI1>());
    }
}