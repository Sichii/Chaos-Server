using System.Net;
using System.Net.Sockets;
using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Factories.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities.Client;
using Chaos.Networking.Options;
using Chaos.Objects;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Servers.Abstractions;
using Chaos.Servers.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Servers;

public sealed class LobbyServer : ServerBase<ILobbyClient>, ILobbyServer
{
    private readonly IClientFactory<ILobbyClient> ClientFactory;
    private readonly ServerTable ServerTable;
    public ConcurrentDictionary<uint, ILobbyClient> Clients { get; }
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
        Options = opts;

        IndexHandlers();
    }

    #region OnHandlers
    public ValueTask OnConnectionInfoRequest(ILobbyClient client, in ClientPacket _)
    {
        client.SendConnectionInfo(ServerTable.CheckSum);

        return default;
    }

    public ValueTask OnServerTableRequest(ILobbyClient client, in ClientPacket packet)
    {
        (var serverTableRequestType, var serverId) = PacketSerializer.Deserialize<ServerTableRequestArgs>(in packet);

        switch (serverTableRequestType)
        {
            case ServerTableRequestType.ServerId:
                if (ServerTable.Servers.TryGetValue(serverId!.Value, out var serverInfo))
                {
                    var redirect = new Redirect(
                        ClientId.NextId,
                        serverInfo,
                        ServerType.Login,
                        client.CryptoClient.Key,
                        client.CryptoClient.Seed);

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
    public override ValueTask HandlePacketAsync(ILobbyClient client, in ClientPacket packet)
    {
        var handler = ClientHandlers[(byte)packet.OpCode];

        return handler?.Invoke(client, in packet) ?? default;
    }

    protected override void IndexHandlers()
    {
        if (ClientHandlers == null!)
            return;

        base.IndexHandlers();

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