#region
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class SocketClientBaseTests
{
    [Test]
    public void BeginReceive_WhenSocketConnected_ShouldSetConnectedTrue()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Connected
                  .Should()
                  .BeFalse();

            client.BeginReceive();

            // Connected = true is set synchronously before the first await
            client.Connected
                  .Should()
                  .BeTrue();
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void BeginReceive_WhenSocketNotConnected_ShouldNotSetConnected()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            // Close the server socket so Socket.Connected returns false
            serverSocket.Close();

            client.BeginReceive();

            client.Connected
                  .Should()
                  .BeFalse();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Constructor_ShouldSetProperties()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Id
                  .Should()
                  .BeGreaterThan(0);

            client.Socket
                  .Should()
                  .BeSameAs(serverSocket);

            client.RemoteIp
                  .Should()
                  .NotBe(IPAddress.None);

            client.Connected
                  .Should()
                  .BeFalse();
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Disconnect_ShouldFireOnDisconnectedEvent()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            var eventFired = false;
            client.OnDisconnected += (_, _) => eventFired = true;
            client.Connected = true;

            client.Disconnect();

            eventFired.Should()
                      .BeTrue();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Disconnect_WhenConnected_ShouldSetConnectedFalse()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Connected = true;
            client.Disconnect();

            client.Connected
                  .Should()
                  .BeFalse();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Disconnect_WhenNotConnected_ShouldDoNothing()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            var act = () => client.Disconnect();

            act.Should()
               .NotThrow();

            client.Connected
                  .Should()
                  .BeFalse();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Disconnect_WhenOnDisconnectedHandlerThrows_ShouldNotThrow()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.OnDisconnected += (_, _) => throw new InvalidOperationException("handler error");
            client.Connected = true;

            var act = () => client.Disconnect();

            act.Should()
               .NotThrow();

            client.Connected
                  .Should()
                  .BeFalse();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Dispose_CalledTwice_ShouldNotThrow()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Dispose();

            var act = () => client.Dispose();

            act.Should()
               .NotThrow();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Dispose_ShouldNotThrow()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            var act = () => client.Dispose();

            act.Should()
               .NotThrow();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Receive_WhenHandlePacketThrows_ShouldResetBufferAndContinue()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.ThrowOnHandlePacket = true;
            client.BeginReceive();
            Thread.Sleep(100);

            // Send a valid packet that will trigger an exception in HandlePacketAsync
            clientSocket.Send(
                new byte[]
                {
                    0xAA,
                    0x00,
                    0x01,
                    0x42
                });

            // ReSharper disable once AccessToDisposedClosure
            SpinWait.SpinUntil(() => client.HandlePacketCallCount > 0, TimeSpan.FromSeconds(5));

            client.HandlePacketCallCount
                  .Should()
                  .Be(1);

            // Client should still be connected (error is caught, buffer is reset)
            client.Connected
                  .Should()
                  .BeTrue();

            // Should be able to receive another packet after the error
            client.ThrowOnHandlePacket = false;

            clientSocket.Send(
                new byte[]
                {
                    0xAA,
                    0x00,
                    0x01,
                    0x43
                });

            // ReSharper disable once AccessToDisposedClosure
            SpinWait.SpinUntil(() => client.HandlePacketCallCount > 1, TimeSpan.FromSeconds(5));

            client.HandlePacketCallCount
                  .Should()
                  .Be(2);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Receive_WhenMultiplePacketsReceived_ShouldProcessAll()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.BeginReceive();
            Thread.Sleep(100);

            // Send two valid packets in a single buffer
            clientSocket.Send(
                new byte[]
                {
                    0xAA,
                    0x00,
                    0x01,
                    0x42, // Packet 1
                    0xAA,
                    0x00,
                    0x01,
                    0x43 // Packet 2
                });

            // ReSharper disable once AccessToDisposedClosure
            SpinWait.SpinUntil(() => client.HandlePacketCallCount >= 2, TimeSpan.FromSeconds(5));

            client.HandlePacketCallCount
                  .Should()
                  .BeGreaterThanOrEqualTo(2);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Receive_WhenPartialPacketReceived_ShouldWaitForMoreData()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.BeginReceive();
            Thread.Sleep(100);

            // Send only 2 bytes — not enough for a complete packet (need > 3)
            clientSocket.Send(
                new byte[]
                {
                    0xAA,
                    0x00
                });
            Thread.Sleep(200);

            client.HandlePacketCallCount
                  .Should()
                  .Be(0);

            // Now send the rest of the packet
            clientSocket.Send(
                new byte[]
                {
                    0x01,
                    0x42
                });

            // ReSharper disable once AccessToDisposedClosure
            SpinWait.SpinUntil(() => client.HandlePacketCallCount > 0, TimeSpan.FromSeconds(5));

            client.HandlePacketCallCount
                  .Should()
                  .Be(1);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Receive_WhenValidPacketReceived_ShouldCallHandlePacket()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.BeginReceive();

            // Wait for receive to be set up
            Thread.Sleep(100);

            // Send a valid packet: [0xAA, len_high, len_low, opcode]
            // packetLength = (0 << 8) + 1 + 3 = 4
            clientSocket.Send(
                new byte[]
                {
                    0xAA,
                    0x00,
                    0x01,
                    0x42
                });

            // ReSharper disable once AccessToDisposedClosure
            SpinWait.SpinUntil(() => client.HandlePacketCallCount > 0, TimeSpan.FromSeconds(5));

            client.HandlePacketCallCount
                  .Should()
                  .Be(1);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Receive_WhenZeroBytesReceived_ShouldDisconnect()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.BeginReceive();

            // Wait for receive to be set up
            Thread.Sleep(100);

            client.Connected
                  .Should()
                  .BeTrue();

            // Close the client socket to trigger 0-byte receive on the server side
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            // Wait for disconnection
            SpinWait.SpinUntil(() => !client.Connected, TimeSpan.FromSeconds(5));

            client.Connected
                  .Should()
                  .BeFalse();
        } finally
        {
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Send_MultipleTimes_ShouldReuseArgsWithoutError()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Connected = true;

            var act = () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var packet = new Packet(0x01, MemoryPool<byte>.Shared.Rent(16), 1);

                    // ReSharper disable once AccessToDisposedClosure
                    client.Send(ref packet);
                }
            };

            act.Should()
               .NotThrow();
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Send_WhenArgsQueueExhausted_ShouldCreateNewArgs()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            // Use reflection to drain the pre-populated SocketArgsQueue (5 items)
            var queueField = typeof(SocketClientBase).GetField("SocketArgsQueue", BindingFlags.NonPublic | BindingFlags.Instance)!;

            var queue = (ConcurrentQueue<SocketAsyncEventArgs>)queueField.GetValue(client)!;

            while (queue.TryDequeue(out _)) { }

            // Now Send will call DequeueArgs → TryDequeue fails → CreateArgs
            client.Connected = true;

            var act = () =>
            {
                var packet = new Packet(0x01, MemoryPool<byte>.Shared.Rent(16), 1);

                // ReSharper disable once AccessToDisposedClosure
                client.Send(ref packet);
            };

            act.Should()
               .NotThrow();
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Send_WhenConnected_AndEncrypted_ShouldCallEncryptAndIncrementSequence()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Connected = true;
            client.IsEncryptedResult = true;

            var packet = new Packet(0x01, MemoryPool<byte>.Shared.Rent(16), 1);
            client.Send(ref packet);

            client.EncryptCallCount
                  .Should()
                  .Be(1);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Send_WhenConnected_AndNotEncrypted_ShouldNotCallEncrypt()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Connected = true;
            client.IsEncryptedResult = false;

            var packet = new Packet(0x01, MemoryPool<byte>.Shared.Rent(16), 1);
            client.Send(ref packet);

            client.EncryptCallCount
                  .Should()
                  .Be(0);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Send_WhenConnected_ShouldSendDataOnSocket()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Connected = true;
            var packet = new Packet(0x01, MemoryPool<byte>.Shared.Rent(16), 1);
            client.Send(ref packet);

            clientSocket.ReceiveTimeout = 2000;
            var recvBuf = new byte[128];
            var received = clientSocket.Receive(recvBuf);

            received.Should()
                    .BeGreaterThan(0);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Send_WhenLogRawPacketsEnabled_ShouldNotThrow()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Connected = true;
            client.LogRawPackets = true;

            var act = () =>
            {
                var packet = new Packet(0x01, MemoryPool<byte>.Shared.Rent(16), 1);

                // ReSharper disable once AccessToDisposedClosure
                client.Send(ref packet);
            };

            act.Should()
               .NotThrow();
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Send_WhenLogSendPacketCodeEnabled_ShouldNotThrow()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            client.Connected = true;
            client.LogRawPackets = false;
            client.LogSendPacketCode = true;

            var act = () =>
            {
                var packet = new Packet(0x01, MemoryPool<byte>.Shared.Rent(16), 1);

                // ReSharper disable once AccessToDisposedClosure
                client.Send(ref packet);
            };

            act.Should()
               .NotThrow();
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void Send_WhenNotConnected_ShouldNotThrow()
    {
        var serializer = MockPacketSerializer.Create();

        (var client, var clientSocket, var serverSocket, var listener)
            = MockSocketClientBase.CreateWithSockets(packetSerializer: serializer);

        try
        {
            var act = () =>
            {
                var packet = new Packet(0x01);

                // ReSharper disable once AccessToDisposedClosure
                client.Send(ref packet);
            };

            act.Should()
               .NotThrow();

            serializer.SerializeCallCount
                      .Should()
                      .Be(0);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void SendGeneric_WhenConnected_ShouldSerializeAndSend()
    {
        var serializer = MockPacketSerializer.Create();

        (var client, var clientSocket, var serverSocket, var listener)
            = MockSocketClientBase.CreateWithSockets(packetSerializer: serializer);

        try
        {
            client.Connected = true;

            var args = new AcceptConnectionArgs
            {
                Message = "test"
            };
            client.Send(args);

            serializer.SerializeCallCount
                      .Should()
                      .Be(1);

            serializer.LastSerialized
                      .Should()
                      .BeSameAs(args);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void SendGeneric_WhenNotConnected_ShouldSerializeButNotSendOnSocket()
    {
        var serializer = MockPacketSerializer.Create();

        (var client, var clientSocket, var serverSocket, var listener)
            = MockSocketClientBase.CreateWithSockets(packetSerializer: serializer);

        try
        {
            // Connected is false by default
            var args = new AcceptConnectionArgs
            {
                Message = "test"
            };
            client.Send(args);

            // Serialize is called regardless of connection state
            serializer.SerializeCallCount
                      .Should()
                      .Be(1);

            // But nothing was sent on the socket
            clientSocket.Available
                        .Should()
                        .Be(0);
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }

    [Test]
    public void SetSequence_ShouldNotThrow()
    {
        (var client, var clientSocket, var serverSocket, var listener) = MockSocketClientBase.CreateWithSockets();

        try
        {
            // ReSharper disable once AccessToDisposedClosure
            var act = () => client.SetSequence(42);

            act.Should()
               .NotThrow();
        } finally
        {
            client.Dispose();
            MockSocketPair.Cleanup(clientSocket, serverSocket, listener);
        }
    }
}