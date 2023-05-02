// ReSharper disable once CheckNamespace

namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Common.Abstractions">Abstract Common</see> DI extensions
/// </summary>
public static class AbstractCommonExtensions
{ /*
    /// <summary>
    ///     Adds an option object based on a configuration section to the service collection and ensures any directory bound options are based off
    ///     of a staging directory
    /// </summary>
    /// <param name="services">The service collection to add the options object to</param>
    /// <param name="subSection">If the section is not at the root level, supply the subsection here</param>
    /// <param name="optionsSection">If the options section is not the same as the class name, supply the name of that section here</param>
    /// <typeparam name="T">The type of the options object</typeparam>
    public static OptionsBuilder<T> AddDirectoryBoundOptionsFromConfig<T>(
        this IServiceCollection services,
        string? subSection = null,
        string? optionsSection = null
    )
        where T: class, IDirectoryBound
    {
        var path = optionsSection ?? typeof(T).Name;

        if (!string.IsNullOrWhiteSpace(subSection))
            path = $"{subSection}:{path}";

        return services.AddOptions<T>()
                       .Configure<IConfiguration, IStagingDirectory>(
                           (options, config, stagingDir) =>
                           {
                               config.GetRequiredSection(path).Bind(options, binder => binder.ErrorOnUnknownConfiguration = true);
                               options.UseBaseDirectory(stagingDir.StagingDirectory);
                           });
    }*/
}