using System.Net.Sockets;
using Chaos.Networking.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Services.Factories;

public class ClientProvider : IClientProvider
{
    private readonly IServiceProvider ServiceProvider;

    public ClientProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    /// <inheritdoc />
    public TClient CreateClient<TClient>(Socket socket) where TClient: ISocketClient
    {
        var factory = ServiceProvider.GetRequiredService<IClientFactory<TClient>>();

        return factory.CreateClient(socket);
    }
}