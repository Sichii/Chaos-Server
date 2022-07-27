using System.Net.Sockets;
using Chaos.Clients;
using Chaos.Cryptography.Interfaces;
using Chaos.Packets.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Hosted.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

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