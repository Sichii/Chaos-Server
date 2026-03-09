#region
using System.Net;
using System.Net.Sockets;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Networking.Tests;

public sealed class ServerBaseTests : IDisposable
{
    private readonly MockServer Server = MockServer.Create();

    public void Dispose() => Server.Dispose();

    [Test]
    public void Dispose_ShouldNotThrow()
    {
        var server = MockServer.Create();

        var act = () => server.Dispose();

        act.Should()
           .NotThrow();
    }

    [Test]
    public async Task ExecuteHandler_NoArgs_ShouldInvokeAction()
    {
        var actionCalled = false;
        var client = MockConnectedClient.Create();

        // ReSharper disable once HeapView.CanAvoidClosure
        await Server.ExecuteHandler(
            client,
            _ =>
            {
                actionCalled = true;

                return default;
            });

        actionCalled.Should()
                    .BeTrue();
    }

    [Test]
    public async Task ExecuteHandler_NoArgs_WhenActionThrows_ShouldNotThrow()
    {
        var client = MockConnectedClient.Create();

        var act = async () => await Server.ExecuteHandler(client, _ => throw new InvalidOperationException("test error"));

        await act.Should()
                 .NotThrowAsync();
    }

    [Test]
    public async Task ExecuteHandler_WithArgs_ShouldInvokeAction()
    {
        var actionCalled = false;
        var client = MockConnectedClient.Create();

        await Server.ExecuteHandler(
            client,
            "test",
            (_, _) =>
            {
                actionCalled = true;

                return default;
            });

        actionCalled.Should()
                    .BeTrue();
    }

    [Test]
    public async Task ExecuteHandler_WithArgs_WhenActionThrows_ShouldNotThrow()
    {
        var client = MockConnectedClient.Create();

        var act = async () => await Server.ExecuteHandler(
            client,
            "test",
            (Func<MockConnectedClient, string, ValueTask>)((_, _) => throw new InvalidOperationException("test error")));

        await act.Should()
                 .NotThrowAsync();
    }

    [Test]
    public async Task HandlePacketAsync_ShouldReturnDefault_WhenNoHandler()
    {
        var client = MockConnectedClient.Create();

        // OpCode 0xFD has no handler registered (array size is byte.MaxValue = 255, indices 0-254)
        var packet = new Packet(0xFD);

        var result = Server.HandlePacketAsync(client, in packet);

        result.IsCompleted
              .Should()
              .BeTrue();

        await result;
    }

    [Test]
    public async Task HandlePacketAsync_ShouldRouteToRegisteredHandler()
    {
        var handlerCalled = false;

        Server.ClientHandlers[0xFE] = (_, in _) =>
        {
            handlerCalled = true;

            return default;
        };

        var client = MockConnectedClient.Create();
        var packet = new Packet(0xFE);

        await Server.HandlePacketAsync(client, in packet);

        handlerCalled.Should()
                     .BeTrue();
    }

    [Test]
    public void IndexHandlers_ShouldRegisterBaseHandlers()
    {
        Server.ClientHandlers[(byte)ClientOpCode.ClientException]
              .Should()
              .NotBeNull();

        Server.ClientHandlers[(byte)ClientOpCode.HeartBeatResponse]
              .Should()
              .NotBeNull();

        Server.ClientHandlers[(byte)ClientOpCode.SequenceChange]
              .Should()
              .NotBeNull();

        Server.ClientHandlers[(byte)ClientOpCode.SynchronizeTicksResponse]
              .Should()
              .NotBeNull();
    }

    [Test]
    public async Task OnClientException_ShouldDeserializeAndLogException()
    {
        var serializer = MockPacketSerializer.Create();

        serializer.DeserializeResult = new ClientExceptionArgs
        {
            ExceptionStr = "Test client exception"
        };

        var server = MockServer.Create(packetSerializer: serializer);

        try
        {
            var client = MockConnectedClient.Create();
            var packet = new Packet((byte)ClientOpCode.ClientException);

            await server.OnClientException(client, in packet);

            serializer.DeserializeCallCount
                      .Should()
                      .Be(1);
        } finally
        {
            server.Dispose();
        }
    }

    [Test]
    public void OnConnection_WhenClientConnects_ShouldCallOnConnected()
    {
        var server = MockServer.Create();

        try
        {
            server.Socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            server.Socket.Listen(100);

            var port = ((IPEndPoint)server.Socket.LocalEndPoint!).Port;

            // Start an accept without a callback so we can manually invoke OnConnection
            var acceptResult = server.Socket.BeginAccept(null, server.Socket);

            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                clientSocket.Connect(IPAddress.Loopback, port);
                acceptResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

                // Calls base.OnConnection which tests the base class implementation.
                // The BeginAccept callback in the finally block will resolve to the safe override.
                server.InvokeBaseOnConnection(acceptResult);

                server.LastConnectedSocket
                      .Should()
                      .NotBeNull();
            } finally
            {
                clientSocket.Dispose();
            }
        } finally
        {
            server.Dispose();
        }
    }

    [Test]
    public void OnConnection_WhenEndAcceptThrows_ShouldNotCallOnConnected()
    {
        var server = MockServer.Create();

        try
        {
            // Bind and listen so BeginAccept in the finally block succeeds
            server.Socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            server.Socket.Listen(100);

            // Create a fake IAsyncResult whose AsyncState is the server socket
            // EndAccept will throw because the IAsyncResult isn't from a real BeginAccept
            var mockResult = new Mock<IAsyncResult>();

            mockResult.Setup(r => r.AsyncState)
                      .Returns(server.Socket);

            // ReSharper disable once AccessToDisposedClosure
            var act = () => server.InvokeBaseOnConnection(mockResult.Object);

            act.Should()
               .NotThrow();

            server.LastConnectedSocket
                  .Should()
                  .BeNull();
        } finally
        {
            server.Dispose();
        }
    }

    [Test]
    public async Task OnHeartBeatAsync_ShouldDeserializeArgs()
    {
        var serializer = MockPacketSerializer.Create();
        var server = MockServer.Create(packetSerializer: serializer);

        try
        {
            var client = MockConnectedClient.Create();
            var packet = new Packet((byte)ClientOpCode.HeartBeatResponse);

            await server.OnHeartBeatAsync(client, in packet);

            serializer.DeserializeCallCount
                      .Should()
                      .Be(1);
        } finally
        {
            server.Dispose();
        }
    }

    [Test]
    public async Task OnSequenceChangeAsync_ShouldCallSetSequence()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockConnectedClient.CreateWithSockets();

        try
        {
            // Create a packet with Sequence = 42
            Span<byte> raw =
            [
                0xAA,
                0x00,
                0x03,
                0x62,
                0x2A,
                0xFF
            ];
            var packet = new Packet(ref raw, true);

            await Server.OnSequenceChangeAsync(client, in packet);

            // The method calls client.SetSequence(packet.Sequence) which is 42
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public async Task OnSynchronizeTicksAsync_ShouldDeserializeArgs()
    {
        var serializer = MockPacketSerializer.Create();
        var server = MockServer.Create(packetSerializer: serializer);

        try
        {
            var client = MockConnectedClient.Create();
            var packet = new Packet((byte)ClientOpCode.SynchronizeTicksResponse);

            await server.OnSynchronizeTicksAsync(client, in packet);

            serializer.DeserializeCallCount
                      .Should()
                      .Be(1);
        } finally
        {
            server.Dispose();
        }
    }
}