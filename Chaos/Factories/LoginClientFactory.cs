using System.Net.Sockets;
using Chaos.Clients;
using Chaos.Cryptography.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Packets.Interfaces;
using Chaos.Servers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class LoginClientFactory : IClientFactory<LoginClient>
{
    private readonly IServiceProvider ServiceProvider;

    public LoginClientFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public LoginClient CreateClient(Socket socket)
    {
        var crypto = ServiceProvider.GetRequiredService<ICryptoClient>();
        var server = ServiceProvider.GetRequiredService<ILoginServer>();
        var serializer = ServiceProvider.GetRequiredService<IPacketSerializer>();
        var logger = ServiceProvider.GetRequiredService<ILogger<LoginClient>>();

        return new LoginClient(
            socket,
            crypto,
            server,
            serializer,
            logger);
    }
}