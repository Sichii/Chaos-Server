using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Configuration;

public sealed class LobbyOptionsConfigurer : RedirectAddressConfigurer, IPostConfigureOptions<LobbyOptions>, IValidateOptions<LobbyOptions>
{
    private readonly ILogger<LobbyOptionsConfigurer> Logger;

    public LobbyOptionsConfigurer(ILogger<LobbyOptionsConfigurer> logger) => Logger = logger;

    /// <inheritdoc />
    public void PostConfigure(string? name, LobbyOptions options)
    {
        base.PostConfigure(name, options);

        foreach (var server in options.Servers)
            base.PostConfigure(name, server);
    }

    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, LobbyOptions options)
    {
        foreach (var server in options.Servers)
        {
            if (server.Description.Length > 18)
            {
                Logger.LogError("Description for \"{ServerName}\" is too long, trimming it to 18 characters", server.Name);
                server.Description = server.Description[..18];
            }

            if (server.Name.Length > 9)
            {
                Logger.LogError("Name for \"{ServerName}\" is too long, trimming it to 9 characters", server.Name);
                server.Name = server.Name[..9];
            }
        }

        return ValidateOptionsResult.Success;
    }
}