using System.Configuration;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Options;

namespace Chaos.Configuration;

public sealed class LoginOptionsConfigurer : RedirectAddressConfigurer, IPostConfigureOptions<LoginOptions>
{
    /// <inheritdoc />
    public void PostConfigure(string? name, LoginOptions options)
    {
        base.PostConfigure(name, options);
        base.PostConfigure(name, options.WorldConnection);

        if (Point.TryParse(options.StartingPointStr, out var point))
            options.StartingPoint = point;
        else
            throw new ConfigurationErrorsException($"Unable to parse starting point from config ({options.StartingPointStr})");
    }
}