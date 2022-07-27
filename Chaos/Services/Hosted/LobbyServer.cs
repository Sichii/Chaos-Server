using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chaos.Clients.Interfaces;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Networking.Model.Client;
using Chaos.Objects;
using Chaos.Packets;
using Chaos.Packets.Definitions;
using Chaos.Packets.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Hosted.Interfaces;
using Chaos.Services.Hosted.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Hosted;

public class LobbyServer : ServerBase, ILobbyServer
{
    private readonly IClientFactory<ILobbyClient> ClientFactory;
    private readonly ServerTable ServerTable;
    public ConcurrentDictionary<uint, ILobbyClient> Clients { get; }
    protected new LobbyClientHandler?[] ClientHandlers { get; }
    protected override LobbyOptions Options { get; }

    public LobbyServer(
        IClientFactory<ILobbyClient> clientFactory,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IOptionsSnapshot<LobbyOptions> options,
        ILogger<LobbyServer> logger
    )
        : base(
            redirectManager,
            packetSerializer,
            options,
            logger)
    {
        var opts = options.Value;
        
        ClientFactory = clientFactory;
        ServerTable = new ServerTable(options.Value.Servers);
        Clients = new ConcurrentDictionary<uint, ILobbyClient>();
        ClientHandlers = new LobbyClientHandler?[byte.MaxValue];
        Options = opts;

        IndexHandlers();
    }

    #region OnHandlers
    public ValueTask OnConnectionInfoRequest(ILobbyClient client, ref ClientPacket _)
    {
        client.SendConnectionInfo(ServerTable.CheckSum);

        return default;
    }

    public ValueTask OnServerTableRequest(ILobbyClient client, ref ClientPacket packet)
    {
        (var serverTableRequestType, var serverId) = PacketSerializer.Deserialize<ServerTableRequestArgs>(ref packet);

        switch (serverTableRequestType)
        {
            case ServerTableRequestType.ServerId:
                if (ServerTable.Servers.TryGetValue(serverId!.Value, out var serverInfo))
                {
                    var redirect = new Redirect(client.CryptoClient, serverInfo, ServerType.Login);
                    RedirectManager.Add(redirect);

                    Logger.LogDebug(
                        "Redirecting lobby client to server {ServerName} with id {ServerId} at {ServerAddress}:{ServerPort}",
                        serverInfo.Name,
                        serverInfo.Id,
                        serverInfo.Address,
                        serverInfo.Port);

                    client.SendRedirect(redirect);
                } else
                    throw new InvalidOperationException($"Server id \"{serverId}\" requested, but does not exist.");

                break;
            case ServerTableRequestType.RequestTable:
                client.SendServerTable(ServerTable);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return default;
    }
    #endregion

    #region Connection / Handler
    protected delegate ValueTask LobbyClientHandler(ILobbyClient client, ref ClientPacket packet);

    public override ValueTask HandlePacketAsync(ISocketClient client, ref ClientPacket packet)
    {
        if (client is ILobbyClient lobbyClient)
            return HandlePacketAsync(lobbyClient, ref packet);
        
        return base.HandlePacketAsync(client, ref packet);
    }

    private ValueTask HandlePacketAsync(ILobbyClient client, ref ClientPacket packet)
    {
        var handler = ClientHandlers[(byte)packet.OpCode];

        return handler?.Invoke(client, ref packet) ?? default;
    }

    protected sealed override void IndexHandlers()
    {
        if (ClientHandlers == null!)
            return;

        base.IndexHandlers();
        var oldHandlers = base.ClientHandlers;

        for (var i = 0; i < byte.MaxValue; i++)
        {
            var old = oldHandlers[i];

            if (old == null)
                continue;

            ClientHandlers[i] = new LobbyClientHandler(old);
        }

        ClientHandlers[(byte)ClientOpCode.ConnectionInfoRequest] = OnConnectionInfoRequest;
        ClientHandlers[(byte)ClientOpCode.ServerTableRequest] = OnServerTableRequest;
    }

    protected override void OnConnection(IAsyncResult ar)
    {
        var serverSocket = (Socket)ar.AsyncState!;
        var clientSocket = serverSocket.EndAccept(ar);

        var ip = clientSocket.RemoteEndPoint as IPEndPoint;
        Logger.LogDebug("Incoming connection from {Ip}", ip);

        serverSocket.BeginAccept(OnConnection, serverSocket);

        var client = ClientFactory.CreateClient(clientSocket);

        if (!Clients.TryAdd(client.Id, client))
        {
            Logger.LogError("Somehow, two clients got the same id? (ID: {Id})", client.Id);
            client.Disconnect();

            return;
        }

        client.OnDisconnected += OnDisconnect;

        client.BeginReceive();
        client.SendAcceptConnection();
    }

    private void OnDisconnect(object? sender, EventArgs e)
    {
        var client = (ILobbyClient)sender!;
        Clients.TryRemove(client.Id, out _);
    }
    #endregion
}