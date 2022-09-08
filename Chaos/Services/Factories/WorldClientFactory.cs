using System.Net.Sockets;
using Chaos.Clients;
using Chaos.Cryptography.Abstractions;
using Chaos.Packets.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Hosted.Abstractions;
using Chaos.Services.Mappers.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class WorldClientFactory : IClientFactory<WorldClient>
{
    private readonly IServiceProvider ServiceProvider;

    public WorldClientFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public WorldClient CreateClient(Socket socket)
    {
        var typeMapper = ServiceProvider.GetRequiredService<ITypeMapper>();
        var crypto = ServiceProvider.GetRequiredService<ICryptoClient>();
        var server = ServiceProvider.GetRequiredService<IWorldServer>();
        var serializer = ServiceProvider.GetRequiredService<IPacketSerializer>();
        var logger = ServiceProvider.GetRequiredService<ILogger<WorldClient>>();

        return new WorldClient(
            socket,
            typeMapper,
            crypto,
            server,
            serializer,
            logger);
    }
}