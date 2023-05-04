using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Factories;

public sealed class LobbyClientFactory : IClientFactory<LobbyClient>
{
    private readonly IOptions<ChaosOptions> ChaosOptions;
    private readonly ILogger<LobbyClient> ClientLogger;
    private readonly ICryptoFactory CryptoFactory;
    private readonly ILobbyServer<ILobbyClient> LobbyServer;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<LobbyClientFactory> Logger;
    private readonly IPacketSerializer PacketSerializer;

    public LobbyClientFactory(
        IOptions<ChaosOptions> chaosOptions,
        ICryptoFactory cryptoFactory,
        ILobbyServer<ILobbyClient> lobbyServer,
        IPacketSerializer packetSerializer,
        ILoggerFactory loggerFactory
    )
    {
        ChaosOptions = chaosOptions;
        CryptoFactory = cryptoFactory;
        LobbyServer = lobbyServer;
        PacketSerializer = packetSerializer;
        ClientLogger = loggerFactory.CreateLogger<LobbyClient>();
        Logger = loggerFactory.CreateLogger<LobbyClientFactory>();
    }

    public LobbyClient CreateClient(Socket socket) =>
        new(
            socket,
            ChaosOptions,
            CryptoFactory.Create(),
            LobbyServer,
            PacketSerializer,
            ClientLogger);
}