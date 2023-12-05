using System.Net.Sockets;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Networking;

internal static class SocketExtensions
{
    internal static void ReceiveAndForget(this Socket socket, SocketAsyncEventArgs args, EventHandler<SocketAsyncEventArgs> completedEvent)
    {
        //if we receive true, it means the io operation is pending, and the completion will be raised on the args completed event
        var completedSynchronously = !socket.ReceiveAsync(args);

        //if we receive false, it means the io operation completed synchronously, and the completed event will not be raised
        if (completedSynchronously)
            completedEvent(socket, args);
    }

    internal static void SendAndForget(this Socket socket, SocketAsyncEventArgs args, EventHandler<SocketAsyncEventArgs> completedEvent)
    {
        var completedSynchronously = !socket.SendAsync(args);

        if (completedSynchronously)
            completedEvent(socket, args);
    }
}