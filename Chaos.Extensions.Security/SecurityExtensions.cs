using System.Text.RegularExpressions;
using Chaos.Security;
using Chaos.Security.Abstractions;
using Chaos.Security.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Security" /> DI extensions
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    ///     Adds the <see cref="IAccessManager" /> to the service collection
    /// </summary>
    /// <param name="services">The service collection to add the service to</param>
    /// <param name="subSection">
    ///     The section where the <see cref="AccessManagerOptions" /> can be located
    ///     in the config
    /// </param>
    public static void AddSecurity(this IServiceCollection services, string subSection)
    {
        services.AddDirectoryBoundOptionsFromConfig<AccessManagerOptions>(subSection)
                .PostConfigure(
                    o =>
                    {
                        o.ValidCharactersRegex = new Regex(o.ValidCharactersPattern, RegexOptions.Compiled);
                        o.ValidFormatRegex = new Regex(o.ValidFormatPattern, RegexOptions.Compiled);
                    });

        services.AddSingleton<IAccessManager, IHostedService, AccessManager>();
    }
}