namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a pattern for an object that has the ability to send and receive packets over a socket connection. Defines
///     methods used by all types of server clients
/// </summary>
public interface IServerClient : ISocketClient
{
    /// <summary>
    ///     Used when a client connects to respond with a string
    /// </summary>
    void SendAcceptConnection(string message);

    /// <summary>
    ///     Used to respond to a client heart beat
    /// </summary>
    void SendHeartBeat(byte first, byte second);

    /// <summary>
    ///     Used to redirect the client to another server
    /// </summary>
    void SendRedirect(IRedirect redirect);
}