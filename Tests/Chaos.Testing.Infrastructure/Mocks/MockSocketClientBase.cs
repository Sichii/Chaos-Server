#region
using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockSocketClientBase : SocketClientBase
{
    public int EncryptCallCount { get; private set; }
    public int HandlePacketCallCount { get; private set; }
    public bool IsEncryptedResult { get; set; }
    public bool ThrowOnHandlePacket { get; set; }

    private MockSocketClientBase(
        Socket socket,
        ICrypto crypto,
        IPacketSerializer packetSerializer,
        ILogger<MockSocketClientBase> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger) { }

    public static MockSocketClientBase Create(
        Socket? socket = null,
        ICrypto? crypto = null,
        IPacketSerializer? packetSerializer = null,
        ILogger<MockSocketClientBase>? logger = null)
    {
        if (socket is null)
        {
            (_, var serverSocket, _) = MockSocketPair.Create();
            socket = serverSocket;
        }

        return new MockSocketClientBase(
            socket,
            crypto ?? MockCrypto.Create(),
            packetSerializer ?? MockPacketSerializer.Create(),
            logger ?? NullLogger<MockSocketClientBase>.Instance);
    }

    public static (MockSocketClientBase Client, Socket ClientSocket, Socket ServerSocket, TcpListener Listener) CreateWithSockets(
        ICrypto? crypto = null,
        IPacketSerializer? packetSerializer = null,
        ILogger<MockSocketClientBase>? logger = null)
    {
        (var clientSocket, var serverSocket, var listener) = MockSocketPair.Create();

        var client = new MockSocketClientBase(
            serverSocket,
            crypto ?? MockCrypto.Create(),
            packetSerializer ?? MockPacketSerializer.Create(),
            logger ?? NullLogger<MockSocketClientBase>.Instance);

        return (client, clientSocket, serverSocket, listener);
    }

    public override void Encrypt(ref Packet packet) => EncryptCallCount++;

    protected override ValueTask HandlePacketAsync(Span<byte> span)
    {
        HandlePacketCallCount++;

        if (ThrowOnHandlePacket)
            throw new InvalidOperationException("Test exception from HandlePacket");

        return default;
    }

    public override bool IsEncrypted(byte opCode) => IsEncryptedResult;
}