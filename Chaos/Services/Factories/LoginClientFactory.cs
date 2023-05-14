using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Factories;

public sealed class LoginClientFactory : IClientFactory<LoginClient>
{
    private readonly IOptions<ChaosOptions> ChaosOptions;
    private readonly ILogger<LoginClient> ClientLogger;
    private readonly ICryptoFactory CryptoFactory;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<LoginClientFactory> Logger;
    private readonly ILoginServer<ILoginClient> LoginServer;
    private readonly ITypeMapper Mapper;
    private readonly IPacketSerializer PacketSerializer;

    public LoginClientFactory(
        IOptions<ChaosOptions> chaosOptions,
        ICryptoFactory cryptoFactory,
        ILoginServer<ILoginClient> loginServer,
        IPacketSerializer packetSerializer,
        ILoggerFactory loggerFactory,
        ITypeMapper mapper
    )
    {
        ChaosOptions = chaosOptions;
        CryptoFactory = cryptoFactory;
        LoginServer = loginServer;
        PacketSerializer = packetSerializer;
        Mapper = mapper;
        ClientLogger = loggerFactory.CreateLogger<LoginClient>();
        Logger = loggerFactory.CreateLogger<LoginClientFactory>();
    }

    public LoginClient CreateClient(Socket socket) =>
        new(
            socket,
            ChaosOptions,
            CryptoFactory.Create(),
            LoginServer,
            PacketSerializer,
            ClientLogger,
            Mapper);
}