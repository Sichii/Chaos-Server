#region
using System.Net.Sockets;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Networking;

internal static class SocketExtensions
{
    extension(Socket socket)
    {
        internal bool IsDisposed()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (socket == null)
                return true;

            if ((socket.Handle == nint.Zero) || (socket.Handle == new nint(-1)))
                return true;

            return false;
        }

        internal void ReceiveAndForget(SocketAsyncEventArgs args, EventHandler<SocketAsyncEventArgs> completedEvent)
        {
            //fast path but not threadsafe
            if (socket.IsDisposed())
                return;

            try
            {
                //if we receive true, it means the io operation is pending, and the completion will be raised on the args completed event
                var completedSynchronously = !socket.ReceiveAsync(args);

                //if we receive false, it means the io operation completed synchronously, and the completed event will not be raised
                if (completedSynchronously)
                    completedEvent(socket, args);
            } catch (ObjectDisposedException)
            {
                //ignored
                //trying to send while socket is disposed
                //expected error that occurs when logging out or disconnecting
            }
        }

        internal void SendAndForget(SocketAsyncEventArgs args, EventHandler<SocketAsyncEventArgs> completedEvent)
        {
            //fast path but not threadsafe
            if (socket.IsDisposed())
            {
                completedEvent(socket, args);

                return;
            }

            try
            {
                var completedSynchronously = !socket.SendAsync(args);

                if (completedSynchronously)
                    completedEvent(socket, args);
            } catch (Exception ex) when (ex is ObjectDisposedException or SocketException)
            {
                //trying to send while socket is disposed or faulted
                //expected error that occurs when logging out or disconnecting
                //invoke the completion handler to clean up the args
                completedEvent(socket, args);
            }
        }
    }
}