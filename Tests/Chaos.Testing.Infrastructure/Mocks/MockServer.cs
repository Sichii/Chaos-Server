#region
using System.Net.Sockets;
using Chaos.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Options;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public sealed class MockServer : ServerBase<MockConnectedClient>
{
    public Socket? LastConnectedSocket { get; private set; }

    public new ClientHandler?[] ClientHandlers => base.ClientHandlers;

    public new Socket Socket => base.Socket;

    private MockServer(
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IClientRegistry<MockConnectedClient> clientRegistry,
        IOptions<ServerOptions> options,
        ILogger<MockServer> logger)
        : base(
            redirectManager,
            packetSerializer,
            clientRegistry,
            options,
            logger) { }

    public static MockServer Create(
        IRedirectManager? redirectManager = null,
        IPacketSerializer? packetSerializer = null,
        IClientRegistry<MockConnectedClient>? clientRegistry = null,
        ServerOptions? serverOptions = null)
    {
        redirectManager ??= new Mock<IRedirectManager>().Object;
        packetSerializer ??= MockPacketSerializer.Create();
        clientRegistry ??= new ClientRegistry<MockConnectedClient>();

        serverOptions ??= new ServerOptions
        {
            Port = 0
        };

        var options = Microsoft.Extensions.Options.Options.Create(serverOptions);

        return new MockServer(
            redirectManager,
            packetSerializer,
            clientRegistry,
            options,
            NullLogger<MockServer>.Instance);
    }

    /// <summary>
    ///     Calls the base class <see cref="ServerBase{T}.OnConnection" /> for testing the base implementation. The base code's
    ///     BeginAccept callback resolves to the safe override via virtual dispatch.
    /// </summary>
    public void InvokeBaseOnConnection(IAsyncResult ar) => base.OnConnection(ar);

    // ReSharper disable once AsyncMethodWithoutAwait
    protected override async Task OnConnected(Socket clientSocket) => LastConnectedSocket = clientSocket;

    /// <summary>
    ///     Safe override that wraps BeginAccept in the finally block with a try/catch, preventing unhandled
    ///     ObjectDisposedException during test cleanup.
    /// </summary>
    protected override void OnConnection(IAsyncResult ar)
    {
        var serverSocket = (Socket)ar.AsyncState!;
        Socket? clientSocket = null;

        try
        {
            clientSocket = serverSocket.EndAccept(ar);
        } catch
        {
            //ignored
        } finally
        {
            try
            {
                serverSocket.BeginAccept(OnConnection, serverSocket);
            } catch
            {
                // Socket disposed during shutdown — safe to ignore in tests
            }
        }

        if (clientSocket is not null && clientSocket.Connected)
        {
            clientSocket.NoDelay = true;

            _ = OnConnected(clientSocket);
        }
    }
}