using System.Net;
using System.Net.Sockets;
using Chaos.Entities.Networking.Client;
using Chaos.Networking.Options;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Networking.Abstractions;

public abstract class ServerBase : BackgroundService, IServer
{
    protected ClientHandler?[] ClientHandlers { get; }
    protected IPEndPoint EndPoint { get; }
    protected ILogger Logger { get; }
    protected abstract ServerOptions Options { get; }
    protected IPacketSerializer PacketSerializer { get; }
    protected IRedirectManager RedirectManager { get; }
    protected Socket Socket { get; }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected ServerBase(
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IOptionsSnapshot<ServerOptions> options,
        ILogger<ServerBase> logger
    )
    {
        RedirectManager = redirectManager;
        Logger = logger;
        PacketSerializer = packetSerializer;
        ClientHandlers = new ClientHandler?[byte.MaxValue];

        var dnsEntry = Dns.GetHostAddresses(options.Value.HostName, AddressFamily.InterNetwork).First();
        EndPoint = new IPEndPoint(dnsEntry, options.Value.Port);

        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IndexHandlers();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var endPoint = new IPEndPoint(IPAddress.Any, Options.Port);
        Socket.Bind(endPoint);
        Socket.Listen(20);
        Socket.BeginAccept(OnConnection, Socket);
        Logger.LogInformation("Listening on {EndPoint}", endPoint);

        return stoppingToken.WaitTillCanceled();
    }

    protected abstract void OnConnection(IAsyncResult ar);

    protected delegate ValueTask ClientHandler(ISocketClient client, ref ClientPacket packet);

    #region Handlers
    protected virtual void IndexHandlers()
    {
        ClientHandlers[(byte)ClientOpCode.HeartBeat] = OnHeartBeatAsync;
        ClientHandlers[(byte)ClientOpCode.SequenceChange] = OnSequenceChangeAsync;
        ClientHandlers[(byte)ClientOpCode.SynchronizeTicks] = OnSynchronizeTicksAsync;
    }

    public virtual ValueTask HandlePacketAsync(ISocketClient client, ref ClientPacket packet)
    {
        var handler = ClientHandlers[(byte)packet.OpCode];

        return handler?.Invoke(client, ref packet) ?? default;
    }

    public ValueTask OnHeartBeatAsync(ISocketClient client, ref ClientPacket packet)
    {
        (var first, var second) = PacketSerializer.Deserialize<HeartBeatArgs>(ref packet);

        client.SendHeartBeat(second, first);

        return default;
    }

    public ValueTask OnSequenceChangeAsync(ISocketClient client, ref ClientPacket packet)
    {
        client.SetSequence(packet.Sequence);

        return default;
    }

    public ValueTask OnSynchronizeTicksAsync(ISocketClient client, ref ClientPacket packet) =>
        //_ = PacketSerializer.Deserialize<SynchronizeTicksArgs>(ref packet);
        //do nothing
        default;
    #endregion
}