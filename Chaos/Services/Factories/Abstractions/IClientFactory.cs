using System.Net.Sockets;
using Chaos.Networking.Abstractions;

namespace Chaos.Services.Factories.Abstractions;

public interface IClientFactory<out TClient> where TClient: ISocketClient
{
    TClient CreateClient(Socket socket);
}