using System.Net.Sockets;
using Chaos.Networking.Abstractions;

namespace Chaos.Factories.Abstractions;

public interface IClientFactory<out TClient> where TClient: ISocketClient
{
    TClient CreateClient(Socket socket);
}