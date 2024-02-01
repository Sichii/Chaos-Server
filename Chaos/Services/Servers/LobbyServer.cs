using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Chaos.Common.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Identity;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Entities;
using Chaos.Networking.Entities.Client;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Services.Servers.Options;
using Microsoft.Extensions.Options;

namespace Chaos.Services.Servers;

public sealed class LobbyServer : ServerBase<ILobbyClient>, ILobbyServer<ILobbyClient>
{
    private readonly IFactory<ILobbyClient> ClientFactory;
    private readonly ServerTable ServerTable;
    private new LobbyOptions Options { get; }

    public LobbyServer(
        IClientRegistry<ILobbyClient> clientRegistry,
        IFactory<ILobbyClient> clientFactory,
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IOptions<LobbyOptions> options,
        ILogger<LobbyServer> logger)
        : base(
            redirectManager,
            packetSerializer,
            clientRegistry,
            options,
            logger)
    {
        Options = options.Value;
        ClientFactory = clientFactory;
        ServerTable = new ServerTable(Options.Servers);

        IndexHandlers();
    }

    #region OnHandlers
    public ValueTask OnVersion(ILobbyClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<VersionArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnVersion);

        ValueTask InnerOnVersion(ILobbyClient localClient, VersionArgs localArgs)
        {
            if (localArgs.Version != 741)
                return default;

            localClient.SendConnectionInfo(ServerTable.CheckSum);

            return default;
        }
    }

    public ValueTask OnServerTableRequest(ILobbyClient client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ServerTableRequestArgs>(in packet);

        return ExecuteHandler(client, args, InnerOnServerTableRequest);

        ValueTask InnerOnServerTableRequest(ILobbyClient localClient, ServerTableRequestArgs localArgs)
        {
            switch (localArgs.ServerTableRequestType)
            {
                case ServerTableRequestType.ServerId:
                    if (ServerTable.Servers.TryGetValue(localArgs.ServerId!.Value, out var serverInfo))
                    {
                        var redirect = new Redirect(
                            EphemeralRandomIdGenerator<uint>.Shared.NextId,
                            serverInfo,
                            ServerType.Login,
                            Encoding.ASCII.GetString(client.Crypto.Key),
                            client.Crypto.Seed);

                        RedirectManager.Add(redirect);

                        Logger.WithTopics(Topics.Servers.LobbyServer, Topics.Entities.Client, Topics.Actions.Redirect)
                              .LogDebug("Redirecting {@ClientIp} to {@ServerIp}", client.RemoteIp, serverInfo.Address.ToString());

                        client.SendRedirect(redirect);
                    } else
                        throw new InvalidOperationException($"Server id \"{localArgs.ServerId}\" requested, but does not exist.");

                    break;
                case ServerTableRequestType.RequestTable:
                    client.SendServerTableResponse(ServerTable);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return default;
        }
    }
    #endregion

    #region Connection / Handler
    public override ValueTask HandlePacketAsync(ILobbyClient client, in Packet packet)
    {
        var opCode = packet.OpCode;
        var handler = ClientHandlers[opCode];

        if (handler is not null)
            Logger.WithTopics(Topics.Servers.LobbyServer, Topics.Entities.Packet, Topics.Actions.Processing)
                  .WithProperty(client)
                  .LogTrace("Processing message with code {@OpCode} from {@ClientIp}", opCode, client.RemoteIp);
        else
            Logger.WithTopics(
                      Topics.Servers.LobbyServer,
                      Topics.Entities.Packet,
                      Topics.Actions.Processing,
                      Topics.Qualifiers.Cheating)
                  .WithProperty(client)
                  .WithProperty(packet.ToString(), "HexData")
                  .LogWarning("Unknown message with code {@OpCode} from {@ClientIp}", opCode, client.RemoteIp);

        return handler?.Invoke(client, in packet) ?? default;
    }

    protected override void IndexHandlers()
    {
        if (ClientHandlers == null!)
            return;

        base.IndexHandlers();

        ClientHandlers[(byte)ClientOpCode.Version] = OnVersion;
        ClientHandlers[(byte)ClientOpCode.ServerTableRequest] = OnServerTableRequest;
    }

    protected override void OnConnected(Socket clientSocket)
    {
        var ip = clientSocket.RemoteEndPoint as IPEndPoint;

        Logger.WithTopics(Topics.Servers.LobbyServer, Topics.Entities.Client, Topics.Actions.Connect)
              .LogDebug("Incoming connection from {@ClientIp}", ip!.Address);

        var client = ClientFactory.Create(clientSocket);

        Logger.WithTopics(Topics.Servers.LobbyServer, Topics.Entities.Client, Topics.Actions.Connect)
              .WithProperty(client)
              .LogInformation("Connection established with {@ClientIp}", client.RemoteIp);

        if (!ClientRegistry.TryAdd(client))
        {
            var stackTrace = new StackTrace(true).ToString();

            Logger.WithTopics(Topics.Servers.LobbyServer, Topics.Entities.Client, Topics.Actions.Connect)
                  .WithProperty(client.Id)
                  .WithProperty(stackTrace)
                  .LogError("Somehow, two clients got the same id");

            client.Disconnect();

            return;
        }

        client.OnDisconnected += OnDisconnect;

        client.BeginReceive();
        client.SendAcceptConnection("CONNECTED SERVER");
    }

    private void OnDisconnect(object? sender, EventArgs e)
    {
        var client = (ILobbyClient)sender!;
        ClientRegistry.TryRemove(client.Id, out _);
    }
    #endregion
}