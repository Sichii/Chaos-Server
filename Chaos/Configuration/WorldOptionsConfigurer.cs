using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Options;

namespace Chaos.Configuration;

public sealed class WorldOptionsConfigurer : RedirectAddressConfigurer, IPostConfigureOptions<WorldOptions>
{
    /// <inheritdoc />
    public void PostConfigure(string? name, WorldOptions options)
    {
        base.PostConfigure(name, options);
        base.PostConfigure(name, options.LoginRedirect);

        WorldOptions.Instance = options;
    }
}