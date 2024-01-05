using System.Net;
using System.Net.Sockets;
using System.Text;
using Chaos.Common.Identity;
using Chaos.Common.Synchronization;
using Chaos.Cryptography.Abstractions;
using Chaos.Extensions.Networking;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;
using Microsoft.Extensions.Logging;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Provides the ability to send and receive packets to and from a client over a socket
/// </summary>
public abstract class SocketClientBase : ISocketClient, IDisposable
{
    private readonly Memory<byte> MemoryBuffer;
    private readonly ConcurrentQueue<SocketAsyncEventArgs> SocketArgsQueue;
    private int Count;
    private int Sequence;

    /// <inheritdoc />
    public bool Connected { get; set; }

    /// <inheritdoc />
    public ICrypto Crypto { get; set; }

    /// <summary>
    ///     Whether or not to log raw packet data to Trace
    /// </summary>
    public bool LogRawPackets { get; set; }

    /// <inheritdoc />
    public uint Id { get; }

    /// <summary>
    ///     The logger for logging client-related events
    /// </summary>
    protected ILogger<SocketClientBase> Logger { get; }

    /// <summary>
    ///     The packet serializer for serializing and deserializing packets
    /// </summary>
    protected IPacketSerializer PacketSerializer { get; }

    /// <inheritdoc />
    public FifoSemaphoreSlim ReceiveSync { get; }

    /// <inheritdoc />
    public IPAddress RemoteIp { get; }

    /// <inheritdoc />
    public Socket Socket { get; }

    private Span<byte> Buffer => MemoryBuffer.Span;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SocketClientBase" /> class.
    /// </summary>
    /// <param name="socket">
    /// </param>
    /// <param name="crypto">
    /// </param>
    /// <param name="packetSerializer">
    /// </param>
    /// <param name="logger">
    /// </param>
    protected SocketClientBase(
        Socket socket,
        ICrypto crypto,
        IPacketSerializer packetSerializer,
        ILogger<SocketClientBase> logger)
    {
        Id = SequentialIdGenerator<uint>.Shared.NextId;
        ReceiveSync = new FifoSemaphoreSlim(1, 1);
        Socket = socket;
        RemoteIp = (socket.RemoteEndPoint as IPEndPoint)?.Address!;
        Crypto = crypto;
        var buffer = new byte[ushort.MaxValue];
        MemoryBuffer = new Memory<byte>(buffer);
        Logger = logger;
        PacketSerializer = packetSerializer;

        var initialArgs = Enumerable.Range(0, 5)
                                    .Select(_ => CreateArgs());
        SocketArgsQueue = new ConcurrentQueue<SocketAsyncEventArgs>(initialArgs);
        Connected = false;
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);

        try
        {
            Socket.Dispose();
        } catch
        {
            //ignored
        }
    }

    /// <inheritdoc />
    public event EventHandler? OnDisconnected;

    /// <summary>
    ///     Asynchronously handles a span buffer as a packet
    /// </summary>
    /// <param name="span">
    /// </param>
    /// <returns>
    /// </returns>
    protected abstract ValueTask HandlePacketAsync(Span<byte> span);

    #region Actions
    /// <inheritdoc />
    public virtual void SendRedirect(IRedirect redirect)
    {
        var args = new RedirectArgs
        {
            EndPoint = redirect.EndPoint,
            Seed = redirect.Seed,
            Key = redirect.Key,
            Name = redirect.Name,
            Id = redirect.Id
        };

        Send(args);
    }

    /// <inheritdoc />
    public virtual void SetSequence(byte newSequence) => Sequence = newSequence;

    /// <inheritdoc />
    public virtual void SendHeartBeat(byte first, byte second)
    {
        var args = new HeartBeatResponseArgs
        {
            First = first,
            Second = second
        };

        Send(args);
    }

    /// <inheritdoc />
    public virtual void SendAcceptConnection()
    {
        var packet = new Packet(ServerOpCode.AcceptConnection);
        var writer = new SpanWriter(PacketSerializer.Encoding);

        writer.WriteByte(27);
        writer.WriteString("CONNECTED SERVER", true);
        packet.Buffer = writer.ToSpan();

        Send(ref packet);
    }
    #endregion

    #region Networking
    /// <inheritdoc />
    public virtual async void BeginReceive()
    {
        Connected = true;
        await Task.Yield();

        var args = new SocketAsyncEventArgs();
        args.SetBuffer(MemoryBuffer);
        args.Completed += ReceiveEventHandler;
        Socket.ReceiveAndForget(args, ReceiveEventHandler);
    }

    /// <inheritdoc />
    public virtual bool IsLoopback()
    {
        if (Socket.RemoteEndPoint is IPEndPoint ipEndPoint)
            return IPAddress.IsLoopback(ipEndPoint.Address);

        return false;
    }

    private async void ReceiveEventHandler(object? sender, SocketAsyncEventArgs e)
    {
        await ReceiveSync.WaitAsync()
                         .ConfigureAwait(false);

        try
        {
            var shouldReset = false;
            var count = e.BytesTransferred;

            //if we received a length of 0, the client is forcing a disconnection
            if (count == 0)
            {
                Disconnect();

                return;
            }

            Count += count;
            var offset = 0;

            //if there's less than 4 bytes in the buffer
            //there isnt enough data to make a packet
            while (Count > 3)
            {
                var packetLength = (Buffer[offset + 1] << 8) + Buffer[offset + 2] + 3;

                //if we havent received the whole packet yet, break
                if (Count < packetLength)
                    break;

                if (Count < 4)
                    break;

                try
                {
                    await HandlePacketAsync(MemoryBuffer.Span.Slice(offset, packetLength));
                } catch (Exception ex)
                {
                    void InnerCatch()
                    {
                        var buffer = MemoryBuffer.Span.TrimEnd((byte)0);

                        var hex = BitConverter.ToString(buffer.ToArray())
                                              .Replace("-", " ");
                        var ascii = Encoding.ASCII.GetString(buffer);

                        Logger.WithTopics(Topics.Entities.Client, Topics.Entities.Packet, Topics.Actions.Processing)
                              .WithProperty(this)
                              .LogError(
                                  ex,
                                  "Exception while handling a packet for {@ClientType}. (Count: {Count}, Offset: {Offset}, BufferHex: {BufferHex}, BufferAscii: {BufferAscii})",
                                  GetType()
                                      .Name,
                                  Count,
                                  offset,
                                  hex,
                                  ascii);
                    }

                    InnerCatch();
                    shouldReset = true;
                }

                Count -= packetLength;
                offset += packetLength;
            }

            //if an error occurs which causes shouldReset to be set to true
            //set the Count to 0, effectively clearing the buffer
            if (shouldReset)
                Count = 0;

            //if we received the first few bytes of a new packet, they wont be at the beginning of the buffer
            //copy those couple bytes to the beginning of the buffer
            if (Count > 0)
                MemoryBuffer.Slice(offset, Count)
                            .CopyTo(MemoryBuffer);

            e.SetBuffer(MemoryBuffer[Count..]);
            Socket.ReceiveAndForget(e, ReceiveEventHandler);
        } catch (Exception)
        {
            //ignored
            Disconnect();
        } finally
        {
            if (Connected)
                ReceiveSync.Release();
        }
    }

    /// <inheritdoc />
    public virtual void Send<T>(T obj) where T: IPacketSerializable
    {
        var packet = PacketSerializer.Serialize(obj);
        Send(ref packet);
    }

    /// <inheritdoc />
    public virtual void Send(ref Packet packet)
    {
        if (!Connected)
            return;

        //no way to pass the packet in because its a ref struct
        //but we still want to avoid serializing the packet to a string if we aren't actually going to log it
        if (LogRawPackets)
            Logger.WithTopics(
                      Topics.Qualifiers.Raw,
                      Topics.Entities.Client,
                      Topics.Entities.Packet,
                      Topics.Actions.Send)
                  .WithProperty(this)
                  .LogTrace("[Snd] {Packet}", packet.ToString());

        packet.IsEncrypted = Crypto.IsServerEncrypted(packet.OpCode);

        if (packet.IsEncrypted)
        {
            packet.Sequence = (byte)Interlocked.Increment(ref Sequence);

            Crypto.Encrypt(ref packet);
        }

        var args = DequeueArgs(packet.ToMemory());
        Socket.SendAndForget(args, ReuseSocketAsyncEventArgs);
    }

    /// <inheritdoc />
    public virtual void Disconnect()
    {
        if (!Connected)
            return;

        Connected = false;

        try
        {
            //shutdown the socket so that we dont receive any more data
            //allow sending incase OnDisconnected sends something
            Socket.Shutdown(SocketShutdown.Receive);
        } catch
        {
            //ignored
        }

        try
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        } catch
        {
            //ignored
        }

        //will close/dispose the socket
        Dispose();
    }
    #endregion

    #region Utility
    private void ReuseSocketAsyncEventArgs(object? sender, SocketAsyncEventArgs e) => SocketArgsQueue.Enqueue(e);

    private SocketAsyncEventArgs CreateArgs()
    {
        var args = new SocketAsyncEventArgs();
        args.Completed += ReuseSocketAsyncEventArgs;

        return args;
    }

    private SocketAsyncEventArgs DequeueArgs(Memory<byte> buffer)
    {
        if (!SocketArgsQueue.TryDequeue(out var args))
            args = CreateArgs();

        args.SetBuffer(buffer);

        return args;
    }
    #endregion
}