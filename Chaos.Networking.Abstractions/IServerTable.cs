namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines the requirements for a server table. The server table is used to display available servers to connect to
///     from the lobby server.
/// </summary>
public interface IServerTable
{
    /// <summary>
    ///     A checksum used to validate the contents of this object so that it doesn't need to be sent every time.
    /// </summary>
    uint CheckSum { get; }

    /// <summary>
    ///     The raw data of this object.
    /// </summary>
    byte[] Data { get; }

    /// <summary>
    ///     Contains information about all available login servers, keyed by their id.
    /// </summary>
    Dictionary<byte, ILoginServerInfo> Servers { get; }
}