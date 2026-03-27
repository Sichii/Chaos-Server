#region
using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockConnectedClient : ConnectedClientBase
{
    public int HandlePacketCallCount { get; private set; }
    public List<IPacketSerializable> SentArgs { get; } = [];

    private MockConnectedClient(
        Socket socket,
        ICrypto crypto,
        IPacketSerializer packetSerializer,
        ILogger<MockConnectedClient> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger) { }

    public static MockConnectedClient Create(
        Socket? socket = null,
        ICrypto? crypto = null,
        IPacketSerializer? packetSerializer = null,
        ILogger<MockConnectedClient>? logger = null)
    {
        if (socket is null)
        {
            (_, var serverSocket, _) = MockSocketPair.Create();
            socket = serverSocket;
        }

        return new MockConnectedClient(
            socket,
            crypto ?? MockCrypto.Create(),
            packetSerializer ?? MockPacketSerializer.Create(),
            logger ?? NullLogger<MockConnectedClient>.Instance);
    }

    public static (MockConnectedClient Client, Socket ClientSocket, Socket ServerSocket, TcpListener Listener) CreateWithSockets(
        ICrypto? crypto = null,
        IPacketSerializer? packetSerializer = null,
        ILogger<MockConnectedClient>? logger = null)
    {
        (var clientSocket, var serverSocket, var listener) = MockSocketPair.Create();

        var client = new MockConnectedClient(
            serverSocket,
            crypto ?? MockCrypto.Create(),
            packetSerializer ?? MockPacketSerializer.Create(),
            logger ?? NullLogger<MockConnectedClient>.Instance);

        return (client, clientSocket, serverSocket, listener);
    }

    protected override ValueTask HandlePacketAsync(Span<byte> span)
    {
        HandlePacketCallCount++;

        return default;
    }

    public override void Send<T>(T obj) => SentArgs.Add(obj);
}