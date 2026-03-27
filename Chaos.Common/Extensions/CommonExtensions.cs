#region
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Common" /> DI extensions
/// </summary>
[ExcludeFromCodeCoverage]
public static class CommonExtensions
{
    /// <param name="services">
    ///     The service collection to add the options object to
    /// </param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Adds an option object based on a configuration section to the service collection
        /// </summary>
        /// <param name="subSection">
        ///     If the section is not at the root level, supply the subsection here
        /// </param>
        /// <param name="optionsSection">
        ///     If the options section is not the same as the class name, supply the name of that section here
        /// </param>
        /// <typeparam name="T">
        ///     The type of the options object
        /// </typeparam>
        public OptionsBuilder<T> AddOptionsFromConfig<T>(string? subSection = null, string? optionsSection = null) where T: class
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
        /// <typeparam name="TI1">
        ///     A base type of <typeparamref name="T" />
        /// </typeparam>
        /// <typeparam name="TI2">
        ///     Another base type of <typeparamref name="T" />
        /// </typeparam>
        /// <typeparam name="T">
        ///     An implementation of the previous two types
        /// </typeparam>
        public void AddSingleton<TI1, TI2, T>() where T: class, TI1, TI2
                                                where TI1: class
                                                where TI2: class
        {
            services.AddSingleton<TI1, T>();
            services.AddSingleton<TI2, T>(p => (T)p.GetRequiredService<TI1>());
        }
    }
}