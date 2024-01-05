using System.Diagnostics.CodeAnalysis;
using Chaos.Security;
using Chaos.Security.Abstractions;
using Chaos.Security.Configuration;
using Chaos.Security.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Security" /> DI extensions
/// </summary>
[ExcludeFromCodeCoverage]
public static class SecurityExtensions
{
    /// <summary>
    ///     Adds the <see cref="IAccessManager" /> to the service collection
    /// </summary>
    /// <param name="services">
    ///     The service collection to add the service to
    /// </param>
    /// <param name="subSection">
    ///     The section where the <see cref="AccessManagerOptions" /> can be located in the config
    /// </param>
    public static void AddSecurity(this IServiceCollection services, string subSection)
    {
        services.AddOptionsFromConfig<AccessManagerOptions>(subSection); //bound
        services.ConfigureOptions<AccessManagerOptionsConfigurer>();
        services.AddSingleton<IAccessManager, IHostedService, AccessManager>();
    }
}