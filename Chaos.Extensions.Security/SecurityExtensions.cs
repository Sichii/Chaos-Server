using System.Text.RegularExpressions;
using Chaos.Security;
using Chaos.Security.Abstractions;
using Chaos.Security.Options;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class SecurityExtensions
{
    public static void AddSecurity(this IServiceCollection services, string subSection)
    {
        services.AddDirectoryBoundOptionsFromConfig<ActiveDirectoryCredentialManagerOptions>(subSection)
                .PostConfigure(
                    o =>
                    {
                        o.ValidCharactersRegex = new Regex(o.ValidCharactersPattern, RegexOptions.Compiled);
                        o.ValidFormatRegex = new Regex(o.ValidFormatPattern, RegexOptions.Compiled);
                    });

        services.AddSingleton<ICredentialManager, ActiveDirectoryCredentialManager>();
    }
}