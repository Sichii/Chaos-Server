using System.Net.Sockets;
using Chaos.Networking.Interfaces;

namespace Chaos.Factories.Interfaces;

public interface IClientFactory<out T> where T: ISocketClient
{
    T CreateClient(Socket socket);
}