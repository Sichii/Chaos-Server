using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Factories;

public sealed class WorldClientFactory : IClientFactory<WorldClient>
{
    private readonly IOptions<ChaosOptions> ChaosOptions;
    private readonly ILogger<WorldClient> ClientLogger;
    private readonly ICryptoFactory CryptoFactory;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<WorldClientFactory> Logger;
    private readonly IPacketSerializer PacketSerializer;
    private readonly ITypeMapper TypeMapper;
    private readonly IWorldServer<IWorldClient> WorldServer;

    public WorldClientFactory(
        IOptions<ChaosOptions> chaosOptions,
        ICryptoFactory cryptoFactory,
        ITypeMapper typeMapper,
        IWorldServer<IWorldClient> worldServer,
        IPacketSerializer packetSerializer,
        ILoggerFactory loggerFactory
    )
    {
        ChaosOptions = chaosOptions;
        CryptoFactory = cryptoFactory;
        TypeMapper = typeMapper;
        WorldServer = worldServer;
        PacketSerializer = packetSerializer;
        ClientLogger = loggerFactory.CreateLogger<WorldClient>();
        Logger = loggerFactory.CreateLogger<WorldClientFactory>();
    }

    public WorldClient CreateClient(Socket socket) =>
        new(
            socket,
            ChaosOptions,
            TypeMapper,
            CryptoFactory.Create(),
            WorldServer,
            PacketSerializer,
            ClientLogger);
}