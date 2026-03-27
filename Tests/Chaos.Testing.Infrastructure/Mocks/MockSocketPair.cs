#region
using System.Net;
using System.Net.Sockets;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockSocketPair
{
    public static void Cleanup(Socket clientSocket, Socket serverSocket, TcpListener listener)
    {
        try
        {
            clientSocket.Dispose();
        } catch
        {
            // ignored
        }

        try
        {
            serverSocket.Dispose();
        } catch
        {
            // ignored
        }

        try
        {
            listener.Stop();
        } catch
        {
            // ignored
        }
    }

    public static (Socket ClientSocket, Socket ServerSocket, TcpListener Listener) Create()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(IPAddress.Loopback, port);

        var serverSocket = listener.AcceptSocket();

        return (clientSocket, serverSocket, listener);
    }
}