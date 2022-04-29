using System;
using Chaos.Networking.Options;
using Microsoft.Extensions.Logging;

namespace Chaos.Options;

public record LobbyOptions : ServerOptions
{
    public ServerInfo[] Servers { get; set; } = Array.Empty<ServerInfo>();

    public static void PostConfigure(LobbyOptions options)
    {
        foreach (var server in options.Servers)
            server.PopulateAddress();
    }

    public static bool Validate(LobbyOptions options, ILogger<LobbyOptions> logger)
    {
        foreach (var server in options.Servers)
        {
            if (server.Description.Length > 18)
            {
                logger.LogError("Description for \"{ServerName}\" is too long, trimming it to 18 characters", server.Name);
                server.Description = server.Description[..18];
            }

            if (server.Name.Length > 9)
            {
                logger.LogError("Name for \"{ServerName}\" is too long, trimming it to 9 characters", server.Name);
                server.Name = server.Name[..9];
            }
        }

        return true;
    }
}