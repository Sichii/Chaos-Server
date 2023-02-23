using System.Text.RegularExpressions;
using Chaos.Security;
using Chaos.Security.Abstractions;
using Chaos.Security.Options;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Security" /> DI extensions
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    ///     Adds <see cref="SimpleCredentialManager" /> as an implementation of
    ///     <see cref="Chaos.Security.Abstractions.ICredentialManager" /> utilizing
    ///     <see cref="SimpleCredentialManagerOptions" /> for configuration
    /// </summary>
    /// <param name="services">The service collection to add the service to</param>
    /// <param name="subSection">
    ///     The section where the <see cref="SimpleCredentialManagerOptions" /> can be located
    ///     in the config
    /// </param>
    public static void AddSecurity(this IServiceCollection services, string subSection)
    {
        services.AddDirectoryBoundOptionsFromConfig<SimpleCredentialManagerOptions>(subSection)
                .PostConfigure(
                    o =>
                    {
                        o.ValidCharactersRegex = new Regex(o.ValidCharactersPattern, RegexOptions.Compiled);
                        o.ValidFormatRegex = new Regex(o.ValidFormatPattern, RegexOptions.Compiled);
                    });

        services.AddDirectoryBoundOptionsFromConfig<IpManagerOptions>(subSection);

        services.AddSingleton<ICredentialManager, SimpleCredentialManager>();
        services.AddSingleton<IIpManager, IpManager>();
    }
}