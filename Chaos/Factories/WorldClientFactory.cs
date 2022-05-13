using System;
using System.Net.Sockets;
using AutoMapper;
using Chaos.Clients;
using Chaos.Cryptography.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Packets.Interfaces;
using Chaos.Servers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class WorldClientFactory : IClientFactory<WorldClient>
{
    private readonly IServiceProvider ServiceProvider;

    public WorldClientFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public WorldClient CreateClient(Socket socket)
    {
        var crypto = ServiceProvider.GetRequiredService<ICryptoClient>();
        var server = ServiceProvider.GetRequiredService<IWorldServer>();
        var mapper = ServiceProvider.GetRequiredService<IMapper>();
        var serializer = ServiceProvider.GetRequiredService<IPacketSerializer>();
        var logger = ServiceProvider.GetRequiredService<ILogger<WorldClient>>();

        return new WorldClient(
            socket,
            mapper,
            crypto,
            server,
            serializer,
            logger);
    }
}