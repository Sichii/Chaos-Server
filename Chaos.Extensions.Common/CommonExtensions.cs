using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Common" /> DI extensions
/// </summary>
public static class CommonExtensions
{
    /// <summary>
    ///     Adds an option object based on a configuration section to the service collection
    /// </summary>
    /// <param name="services">The service collection to add the options object to</param>
    /// <param name="subSection">If the section is not at the root level, supply the subsection here</param>
    /// <param name="optionsSection">If the options section is not the same as the class name, supply the name of that section here</param>
    /// <typeparam name="T">The type of the options object</typeparam>
    public static OptionsBuilder<T> AddOptionsFromConfig<T>(
        this IServiceCollection services,
        string? subSection = null,
        string? optionsSection = null
    ) where T: class
    {
        var path = optionsSection ?? typeof(T).Name;

        if (!string.IsNullOrWhiteSpace(subSection))
            path = $"{subSection}:{path}";

        return services.AddOptions<T>()
                       .Configure<IConfiguration>(
                           (o, c) => c.GetRequiredSection(path).Bind(o, options => options.ErrorOnUnknownConfiguration = true));
    }
}