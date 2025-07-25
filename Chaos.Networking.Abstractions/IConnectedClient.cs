namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a pattern for an object that represents a client connected to an <see cref="IServer{T}" />. This interface
///     contains the definitions used to communicate with the client
/// </summary>
public interface IConnectedClient : ISocketClient
{
    /// <summary>
    ///     Used when a client connects to respond with a string
    /// </summary>
    void SendAcceptConnection(string message);

    /// <summary>
    ///     Used to ping the client and check if it is still connected.
    /// </summary>
    void SendHeartBeat(byte first, byte second);

    /// <summary>
    ///     Used to redirect the client to another server
    /// </summary>
    void SendRedirect(IRedirect redirect);

    /// <summary>
    ///     Used to check synchronization ticks between server and client.
    /// </summary>
    void SendSynchronizeTicks();
}