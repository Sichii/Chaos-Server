using System.Net.Sockets;
using Chaos.Networking.Interfaces;

namespace Chaos.Factories.Interfaces;

public interface IClientFactory<out TClient> where TClient: ISocketClient
{
    TClient CreateClient(Socket socket);
}