using System.Net.Sockets;
using Chaos.Clients;
using Chaos.Cryptography.Abstractions;
using Chaos.Packets.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class LobbyClientFactory : IClientFactory<LobbyClient>
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