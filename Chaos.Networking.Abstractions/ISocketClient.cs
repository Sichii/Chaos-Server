using System.Net;
using System.Net.Sockets;
using Chaos.Common.Synchronization;
using Chaos.Cryptography.Abstractions;
using Chaos.Packets;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a pattern for an object that has the ability to send and receive packets over a socket connection
/// </summary>
public interface ISocketClient
{
    /// <summary>
    ///     Handles encryption and decryption of packets
    /// </summary>
    ICryptoClient CryptoClient { get; set; }

    /// <summary>
    ///     An event that is called when a client disconnects
    /// </summary>
    event EventHandler? OnDisconnected;

    /// <summary>
    ///     Whether or not the client is connected
    /// </summary>
    bool Connected { get; }

    /// <summary>
    ///     A unique id specific to this client
    /// </summary>
    uint Id { get; }

    /// <summary>
    ///     A semaphoreslim that obeys FIFO rules
    /// </summary>
    FifoSemaphoreSlim ReceiveSync { get; }

    /// <summary>
    ///     The remote endpoint of the client
    /// </summary>
    IPAddress? RemoteIp { get; }
    /// <summary>
    ///     The socket that the client is connected to
    /// </summary>
    Socket Socket { get; }

    /// <summary>
    ///     Begins an operation that receives data from the socket
    /// </summary>
    void BeginReceive();

    /// <summary>
    ///     Disconnects the client from the server and calls the <see cref="Chaos.Networking.Abstractions.ISocketClient.OnDisconnected" /> event
    /// </summary>
    void Disconnect();

    /// <summary>
    ///     Determins whether or not the client is a loopback client or not (client is on same server as ip)
    /// </summary>
    bool IsLoopback();

    /// <summary>
    ///     Serializes an object and sends it to the client
    /// </summary>
    /// <param name="obj">The object to be serialized and sent</param>
    /// <typeparam name="T">
    ///     The type must inherit <see cref="Chaos.Packets.Abstractions.ISendArgs" />
    ///     and have a <see cref="Chaos.Packets.Abstractions.IServerPacketSerializer{T}" /> created for it
    /// </typeparam>
    void Send<T>(T obj) where T: ISendArgs;

    /// <summary>
    ///     Sends a packet to the client
    /// </summary>
    void Send(ref ServerPacket packet);

    /// <summary>
    ///     Used when a client connects to response with a string
    /// </summary>
    void SendAcceptConnection();

    /// <summary>
    ///     Used to response to a client heart beat
    /// </summary>
    void SendHeartBeat(byte first, byte second);

    /// <summary>
    ///     Used to redirect the client to another server
    /// </summary>
    void SendRedirect(IRedirect redirect);

    /// <summary>
    ///     Used when a client requests to change the packet sequence
    /// </summary>
    /// <param name="newSequence"></param>
    void SetSequence(byte newSequence);
}