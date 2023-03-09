using System.Net.Sockets;
using Chaos.Networking.Abstractions;

namespace Chaos.Services.Factories.Abstractions;

public interface IClientProvider
{
    TClient CreateClient<TClient>(Socket socket) where TClient: ISocketClient;
}