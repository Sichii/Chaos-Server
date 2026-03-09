#region
using System.Net.Sockets;
using Chaos.Extensions.Networking;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class SocketExtensionsTests
{
    [Test]
    public void IsDisposed_ShouldNotThrow_ForDisposedSocket()
    {
        (var clientSocket, var serverSocket, var listener) = MockSocketPair.Create();
        serverSocket.Close();
        serverSocket.Dispose();

        try
        {
            // IsDisposed should not throw for a disposed socket, regardless of platform behavior
            var act = () => serverSocket.IsDisposed();

            act.Should()
               .NotThrow();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void IsDisposed_ShouldReturnFalse_ForActiveSocket()
    {
        (var clientSocket, var serverSocket, var listener) = MockSocketPair.Create();

        try
        {
            serverSocket.IsDisposed()
                        .Should()
                        .BeFalse();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void ReceiveAndForget_ShouldNotThrow_WhenSocketDisposed()
    {
        (var clientSocket, var serverSocket, var listener) = MockSocketPair.Create();
        serverSocket.Dispose();

        try
        {
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(new byte[128]);

            var act = () => serverSocket.ReceiveAndForget(args, static (_, _) => { });

            act.Should()
               .NotThrow();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void ReceiveAndForget_WhenDataAvailable_ShouldReceiveData()
    {
        (var clientSocket, var serverSocket, var listener) = MockSocketPair.Create();

        try
        {
            // Send data first so the kernel buffer has data
            clientSocket.Send(
                new byte[]
                {
                    0x01,
                    0x02,
                    0x03
                });
            Thread.Sleep(100);

            var handlerCalled = false;
            var bytesReceived = 0;
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(new byte[128]);

            serverSocket.ReceiveAndForget(
                args,
                (_, e) =>
                {
                    bytesReceived = e.BytesTransferred;
                    handlerCalled = true;
                });

            // If the operation completed asynchronously, wait for the Completed event
            if (!handlerCalled)
            {
                var completed = new ManualResetEventSlim(false);
                args.Completed += (_, _) => completed.Set();
                completed.Wait(TimeSpan.FromSeconds(5));
            }

            bytesReceived.Should()
                         .Be(3);
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void SendAndForget_ShouldNotThrow_WhenSocketDisposed()
    {
        (var clientSocket, var serverSocket, var listener) = MockSocketPair.Create();
        serverSocket.Dispose();

        try
        {
            var args = new SocketAsyncEventArgs();

            args.SetBuffer(
                new byte[]
                {
                    1,
                    2,
                    3
                });

            var act = () => serverSocket.SendAndForget(args, static (_, _) => { });

            act.Should()
               .NotThrow();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void SendAndForget_ShouldSendData_WhenSocketConnected()
    {
        (var clientSocket, var serverSocket, var listener) = MockSocketPair.Create();

        try
        {
            var completed = new ManualResetEventSlim(false);
            var args = new SocketAsyncEventArgs();

            args.SetBuffer(
                new byte[]
                {
                    0xAA,
                    0xBB,
                    0xCC
                });
            args.Completed += (_, _) => completed.Set();

            serverSocket.SendAndForget(args, (_, _) => completed.Set());

            completed.Wait(TimeSpan.FromSeconds(5));

            var recvBuffer = new byte[3];
            clientSocket.Receive(recvBuffer);

            recvBuffer.Should()
                      .Equal(0xAA, 0xBB, 0xCC);
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }
}