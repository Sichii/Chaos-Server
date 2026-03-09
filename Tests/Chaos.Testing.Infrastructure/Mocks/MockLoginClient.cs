#region
using System.Net.Sockets;
using Chaos.Cryptography.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockLoginClient : LoginClientBase
{
    public List<IPacketSerializable> SentArgs { get; } = [];

    private MockLoginClient(
        Socket socket,
        ICrypto crypto,
        IPacketSerializer packetSerializer,
        ILogger<MockLoginClient> logger)
        : base(
            socket,
            crypto,
            packetSerializer,
            logger) { }

    public static MockLoginClient Create(Socket? socket = null, ICrypto? crypto = null, IPacketSerializer? packetSerializer = null)
    {
        if (socket is null)
        {
            (_, var serverSocket, _) = MockSocketPair.Create();
            socket = serverSocket;
        }

        return new MockLoginClient(
            socket,
            crypto ?? MockCrypto.Create(),
            packetSerializer ?? MockPacketSerializer.Create(),
            NullLogger<MockLoginClient>.Instance);
    }

    protected override ValueTask HandlePacketAsync(Span<byte> span) => default;

    public override void Send<T>(T obj) => SentArgs.Add(obj);
}