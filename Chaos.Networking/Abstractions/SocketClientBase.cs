using System.Net;
using System.Net.Sockets;
using Chaos.Core.Identity;
using Chaos.Core.Synchronization;
using Chaos.Cryptography.Interfaces;
using Chaos.IO.Memory;
using Chaos.Networking.Extensions;
using Chaos.Networking.Interfaces;
using Chaos.Networking.Model;
using Chaos.Networking.Model.Server;
using Chaos.Packets;
using Chaos.Packets.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Networking.Abstractions;

public abstract class SocketClientBase : ISocketClient, IDisposable
{
    private readonly byte[] Buffer;
    private readonly Memory<byte> MemoryBuffer;
    public FifoSemaphoreSlim ReceiveSync { get; }
    private readonly Socket Socket;
    private readonly ConcurrentQueue<SocketAsyncEventArgs> SocketArgsQueue;
    private int Count;
    private int Sequence;
    public bool Connected { get; set; }
    public ICryptoClient CryptoClient { get; set; }

    public event EventHandler? OnDisconnected;
    public uint Id { get; }
    protected ILogger Logger { get; }
    protected IPacketSerializer PacketSerializer { get; }
    protected IServer Server { get; }

    protected SocketClientBase(
        Socket socket,
        ICryptoClient cryptoClient,
        IServer server,
        IPacketSerializer packetSerializer,
        ILogger<SocketClientBase> logger
    )
    {
        Id = ClientId.NextId;
        ReceiveSync = new FifoSemaphoreSlim(1, 1);
        Socket = socket;
        CryptoClient = cryptoClient;
        Server = server;
        Buffer = new byte[short.MaxValue];
        MemoryBuffer = new Memory<byte>(Buffer);
        Logger = logger;
        PacketSerializer = packetSerializer;

        var initialArgs = Enumerable.Range(0, 5).Select(_ => CreateArgs());
        SocketArgsQueue = new ConcurrentQueue<SocketAsyncEventArgs>(initialArgs);
        Connected = false;
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        Socket.Dispose();
    }

    protected ValueTask HandlePacketAsync(Span<byte> span)
    {
        var opCode = span[3];
        var isEncrypted = CryptoClient.ShouldBeEncrypted(opCode);
        var packet = new ClientPacket(ref span, isEncrypted);

        if (isEncrypted)
            CryptoClient.Decrypt(ref packet);

        Logger.LogTrace("[Rcv] {Packet}", packet.GetHexString());

        return Server.HandlePacketAsync(this, ref packet);
    }

    #region Actions
    public void SendRedirect(Redirect redirect)
    {
        var args = new RedirectArgs(redirect);

        Send(args);
    }

    public void SetSequence(byte newSequence) => Sequence = newSequence;

    public void SendHeartBeat(byte first, byte second)
    {
        var args = new HeartBeatResponseArgs
        {
            First = first,
            Second = second
        };

        Send(args);
    }

    public virtual void SendAcceptConnection()
    {
        var packet = new ServerPacket(ServerOpCode.AcceptConnection);
        var writer = new SpanWriter(PacketSerializer.Encoding);

        writer.WriteByte(27);
        writer.WriteString("CONNECTED SERVER", true);
        packet.Buffer = writer.ToSpan();

        Send(ref packet);
    }
    #endregion

    #region Networking
    public async void BeginReceive()
    {
        Connected = true;
        await Task.Yield();

        var args = new SocketAsyncEventArgs();
        args.SetBuffer(MemoryBuffer);
        args.Completed += ReceiveEventHandler;
        Socket.ReceiveAndForget(args, ReceiveEventHandler);
    }

    public bool IsLoopback()
    {
        if (Socket.RemoteEndPoint is IPEndPoint ipEndPoint)
            return IPAddress.IsLoopback(ipEndPoint.Address);

        return false;
    }

    private async void ReceiveEventHandler(object? sender, SocketAsyncEventArgs e)
    {
        await ReceiveSync.WaitAsync().ConfigureAwait(false);

        try
        {
            var count = e.BytesTransferred;

            //if we received a length of 0, the client is forcing a disconnection
            if (count == 0)
            {
                Disconnect();

                return;
            }

            Count += count;
            var offset = 0;

            var endPosition = Count;

            //if there's less than 4 bytes in the buffer
            //there isnt enough data to make a packet
            while (Count > 3)
            {
                var packetLength = (Buffer[offset + 1] << 8) + Buffer[offset + 2] + 3;

                try
                {
                    await HandlePacketAsync(MemoryBuffer.Span.Slice(offset, packetLength)).ConfigureAwait(false);
                } catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception while handling a packet");
                }

                Count -= packetLength;
                offset += packetLength;
            }

            //if we received the first few bytes of a new packet, they wont be at the beginning of the buffer
            //copy those couple bytes to the beginning of the buffer
            if (Count > 0)
                MemoryBuffer.Slice(endPosition - Count, Count).CopyTo(MemoryBuffer);

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

    public void Send<T>(T obj) where T: ISendArgs
    {
        var packet = PacketSerializer.Serialize(obj);
        Send(ref packet);
    }

    public void Send(ref ServerPacket packet)
    {
        if (!Connected)
            return;

        Logger.LogTrace("[Snd] {Packet}", packet.GetHexString());

        packet.ShouldEncrypt = CryptoClient.ShouldEncrypt((byte)packet.OpCode);

        if (packet.ShouldEncrypt)
        {
            packet.Sequence = (byte)Interlocked.Increment(ref Sequence);

            CryptoClient.Encrypt(ref packet);
        }

        var args = DequeueArgs(packet.ToMemory());
        Socket.SendAndForget(args, ReuseSocketAsyncEventArgs);
    }

    public virtual void Disconnect()
    {
        if (!Connected)
            return;

        Connected = false;

        try
        {
            Socket.Disconnect(false);
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

        Dispose();
    }
    #endregion

    #region Utility
    private void ReuseSocketAsyncEventArgs(object? sender, SocketAsyncEventArgs e) =>
        SocketArgsQueue.Enqueue(e);

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