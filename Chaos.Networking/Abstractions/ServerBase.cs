#region
using System.Net;
using System.Net.Sockets;
using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Networking.Options;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents a base class for server implementations.
/// </summary>
/// <typeparam name="T">
///     The type of the socket client.
/// </typeparam>
public abstract class ServerBase<T> : BackgroundService, IServer<T> where T: IConnectedClient
{
    /// <summary>
    ///     Delegate for handling client packets.
    /// </summary>
    /// <param name="client">
    ///     The client sending the packet.
    /// </param>
    /// <param name="packet">
    ///     The client packet received.
    /// </param>
    /// <returns>
    ///     A ValueTask representing the asynchronous operation.
    /// </returns>
    public delegate ValueTask ClientHandler(T client, in Packet packet);

    /// <summary>
    ///     An array of client handlers for handling incoming client packets.
    /// </summary>
    protected ClientHandler?[] ClientHandlers { get; }

    /// <summary>
    ///     The client registry for managing connected clients.
    /// </summary>
    protected IClientRegistry<T> ClientRegistry { get; }

    /// <summary>
    ///     The logger for logging server-related events.
    /// </summary>
    protected ILogger<ServerBase<T>> Logger { get; }

    /// <summary>
    ///     The server options for configuring the server instance.
    /// </summary>
    protected ServerOptions Options { get; }

    /// <summary>
    ///     The packet serializer for serializing and deserializing packets.
    /// </summary>
    protected IPacketSerializer PacketSerializer { get; }

    /// <summary>
    ///     The redirect manager for handling client redirects.
    /// </summary>
    protected IRedirectManager RedirectManager { get; }

    /// <summary>
    ///     The socket used for handling incoming connections.
    /// </summary>
    protected Socket Socket { get; }

    /// <summary>
    ///     A semaphore for synchronizing access to the server.
    /// </summary>
    protected FifoAutoReleasingSemaphoreSlim Sync { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServerBase{T}" /> class.
    /// </summary>
    /// <param name="redirectManager">
    ///     An instance of a redirect manager.
    /// </param>
    /// <param name="packetSerializer">
    ///     An instance of a packet serializer.
    /// </param>
    /// <param name="clientRegistry">
    ///     An instance of a client registry.
    /// </param>
    /// <param name="options">
    ///     Configuration options for the server.
    /// </param>
    /// <param name="logger">
    ///     A logger for the server.
    /// </param>
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected ServerBase(
        IRedirectManager redirectManager,
        IPacketSerializer packetSerializer,
        IClientRegistry<T> clientRegistry,
        IOptions<ServerOptions> options,
        ILogger<ServerBase<T>> logger)
    {
        Options = options.Value;
        RedirectManager = redirectManager;
        Logger = logger;
        ClientRegistry = clientRegistry;
        PacketSerializer = packetSerializer;
        ClientHandlers = new ClientHandler?[byte.MaxValue];
        Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Sync = new FifoAutoReleasingSemaphoreSlim(1, 1, $"{GetType().Name}");
        IndexHandlers();
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        GC.SuppressFinalize(this);

        try
        {
            Socket.Close();
        } catch
        {
            //ignored
        }

        base.Dispose();
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var endPoint = new IPEndPoint(IPAddress.Any, Options.Port);
        Socket.Bind(endPoint);
        Socket.Listen(100);

        Logger.WithTopics(Topics.Actions.Listening)
              .LogInformation("Listening on {@EndPoint}", endPoint.ToString());

        Socket.BeginAccept(OnConnection, Socket);

        await stoppingToken.WaitTillCanceled();

        try
        {
            Socket.Shutdown(SocketShutdown.Receive);
        } catch
        {
            //ignored
        }

        await Parallel.ForEachAsync(
            ClientRegistry,
            (client, _) =>
            {
                try
                {
                    client.Disconnect();
                } catch
                {
                    //ignored
                }

                return default;
            });

        Dispose();
    }

    /// <summary>
    ///     Called when a new connection is accepted by the server.
    /// </summary>
    /// <param name="clientSocket">
    ///     The socket that connected to the server
    /// </param>
    protected abstract void OnConnected(Socket clientSocket);

    /// <summary>
    ///     Called when a new connection is accepted by the server.
    /// </summary>
    /// <param name="ar">
    ///     The result of the asynchronous connection operation
    /// </param>
    protected virtual void OnConnection(IAsyncResult ar)
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
            serverSocket.BeginAccept(OnConnection, serverSocket);
        }

        if (clientSocket is not null && clientSocket.Connected)
        {
            clientSocket.NoDelay = true;

            OnConnected(clientSocket);
        }
    }

    #region Handlers
    /// <summary>
    ///     Initializes the client handlers for the server.
    /// </summary>
    protected virtual void IndexHandlers()
    {
        ClientHandlers[(byte)ClientOpCode.ClientException] = OnClientException;
        ClientHandlers[(byte)ClientOpCode.HeartBeatResponse] = OnHeartBeatAsync;
        ClientHandlers[(byte)ClientOpCode.SequenceChange] = OnSequenceChangeAsync;
        ClientHandlers[(byte)ClientOpCode.SynchronizeTicksResponse] = OnSynchronizeTicksAsync;
    }

    /// <inheritdoc />
    public virtual ValueTask HandlePacketAsync(T client, in Packet packet)
    {
        var handler = ClientHandlers[packet.OpCode];

        if (handler is null)
            return default;

        return handler(client, in packet);
    }

    /// <summary>
    ///     Executes an asynchronous action for a client within a sychronized context
    /// </summary>
    /// <param name="client">
    ///     The client to execute the action against
    /// </param>
    /// <param name="args">
    ///     The args deserialized from the packet
    /// </param>
    /// <param name="action">
    ///     The action that uses the args
    /// </param>
    /// <typeparam name="TArgs">
    ///     The type of the args that were deserialized
    /// </typeparam>
    public virtual async ValueTask ExecuteHandler<TArgs>(T client, TArgs args, Func<T, TArgs, ValueTask> action)
    {
        await using var @lock = await Sync.WaitAsync(TimeSpan.FromSeconds(1));

        try
        {
            await action(client, args);
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.Packet, Topics.Actions.Processing)
                  .WithProperty(client)
                  .LogError(
                      e,
                      "{@ClientType} failed to execute inner handler with args type {@ArgsType} ({@Args})",
                      client.GetType()
                            .Name,
                      args!.GetType()
                           .Name,
                      args);
        }
    }

    /// <summary>
    ///     Executes an asynchronous action for a client within a sychronized context
    /// </summary>
    /// <param name="client">
    ///     The client to execute the action against
    /// </param>
    /// <param name="action">
    ///     The action to be executed
    /// </param>
    public virtual async ValueTask ExecuteHandler(T client, Func<T, ValueTask> action)
    {
        await using var @lock = await Sync.WaitAsync();

        try
        {
            await action(client);
        } catch (Exception e)
        {
            Logger.WithTopics(Topics.Entities.Packet, Topics.Actions.Processing)
                  .WithProperty(client)
                  .LogError(
                      e,
                      "{@ClientType} failed to execute inner handler",
                      client.GetType()
                            .Name);
        }
    }

    /// <inheritdoc />
    public virtual ValueTask OnHeartBeatAsync(T client, in Packet packet)
    {
        _ = PacketSerializer.Deserialize<HeartBeatResponseArgs>(in packet);

        //do nothing

        return default;
    }

    /// <inheritdoc />
    public ValueTask OnSequenceChangeAsync(T client, in Packet packet)
    {
        client.SetSequence(packet.Sequence);

        return default;
    }

    /// <inheritdoc />
    public virtual ValueTask OnClientException(T client, in Packet packet)
    {
        var args = PacketSerializer.Deserialize<ClientExceptionArgs>(in packet);

        Logger.WithTopics(Topics.Entities.Packet, Topics.Actions.Processing)
              .WithProperty(client)
              .LogError(
                  "{@ClientType} encountered an exception: {@Exception}",
                  client.GetType()
                        .Name,
                  args.ExceptionStr);

        return default;
    }

    /// <inheritdoc />
    public virtual ValueTask OnSynchronizeTicksAsync(T client, in Packet packet)
    {
        _ = PacketSerializer.Deserialize<SynchronizeTicksResponseArgs>(in packet);

        //do nothing

        return default;
    }
    #endregion
}