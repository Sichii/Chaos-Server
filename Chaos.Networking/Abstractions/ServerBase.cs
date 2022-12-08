using System.Net;
using System.Net.Sockets;
using Chaos.Extensions.Common;
using Chaos.Networking.Entities.Client;
using Chaos.Networking.Options;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Networking.Abstractions;

public abstract class ServerBase<T> : BackgroundService, IServer<T> where T: ISocketClient
{
    public delegate ValueTask ClientHandler(T client, in ClientPacket packet);

    protected ClientHandler?[] ClientHandlers { get; }
    protected IClientRegistry<T> ClientRegistry { get; }
    protected ILogger<ServerBase<T>> Logger { get; }
    protected ServerOptions Options { get; }
    protected IPacketSerializer PacketSerializer { get; }
    protected IRedirectManager RedirectManager { get; }
    protected Socket Socket { get; }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected ServerBase(
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IClientRegistry<T> clientRegistry,
        IOptions<ServerOptions> options,
        ILogger<ServerBase<T>> logger
    )
    {
        Options = options.Value;
        RedirectManager = redirectManager;
        Logger = logger;
        ClientRegistry = clientRegistry;
        PacketSerializer = packetSerializer;
        ClientHandlers = new ClientHandler?[byte.MaxValue];
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IndexHandlers();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var endPoint = new IPEndPoint(IPAddress.Any, Options.Port);
        Socket.Bind(endPoint);
        Socket.Listen(20);
        Socket.BeginAccept(OnConnection, Socket);
        Logger.LogInformation("Listening on {EndPoint}", endPoint);

        await stoppingToken.WaitTillCanceled();
    }

    protected abstract void OnConnection(IAsyncResult ar);

    #region Handlers
    protected virtual void IndexHandlers()
    {
        ClientHandlers[(byte)ClientOpCode.HeartBeat] = OnHeartBeatAsync;
        ClientHandlers[(byte)ClientOpCode.SequenceChange] = OnSequenceChangeAsync;
        ClientHandlers[(byte)ClientOpCode.SynchronizeTicks] = OnSynchronizeTicksAsync;
    }

    public virtual ValueTask HandlePacketAsync(T client, in ClientPacket packet)
    {
        var handler = ClientHandlers[(byte)packet.OpCode];

        return handler?.Invoke(client, in packet) ?? default;
    }

    public ValueTask OnHeartBeatAsync(T client, in ClientPacket packet)
    {
        (var first, var second) = PacketSerializer.Deserialize<HeartBeatArgs>(in packet);

        client.SendHeartBeat(second, first);

        return default;
    }

    public ValueTask OnSequenceChangeAsync(T client, in ClientPacket packet)
    {
        client.SetSequence(packet.Sequence);

        return default;
    }

    public ValueTask OnSynchronizeTicksAsync(T client, in ClientPacket packet) =>
        //_ = PacketSerializer.Deserialize<SynchronizeTicksArgs>(ref packet);
        //do nothing
        default;
    #endregion
}