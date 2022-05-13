using System;
using System.Net.Sockets;
using Chaos.Clients;
using Chaos.Cryptography.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Packets.Interfaces;
using Chaos.Servers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class LobbyClientFactory : IClientFactory<LobbyClient>
{
    private readonly IServiceProvider ServiceProvider;

    public LobbyClientFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public LobbyClient CreateClient(Socket socket)
    {
        var crypto = ServiceProvider.GetRequiredService<ICryptoClient>();
        var server = ServiceProvider.GetRequiredService<ILobbyServer>();
        var serializer = ServiceProvider.GetRequiredService<IPacketSerializer>();
        var logger = ServiceProvider.GetRequiredService<ILogger<LobbyClient>>();

        return new LobbyClient(
            socket,
            crypto,
            server,
            serializer,
            logger);
    }
}