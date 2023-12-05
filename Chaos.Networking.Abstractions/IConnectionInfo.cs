using System.Net;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines a configuration object thats contains connection information for a server
/// </summary>
public interface IConnectionInfo
{
    /// <summary>
    ///     The ip address to redirect to
    /// </summary>
    IPAddress Address { get; set; }

    /// <summary>
    ///     The hostname to redirect to
    /// </summary>
    string HostName { get; set; }

    /// <summary>
    ///     The port to redirect to
    /// </summary>
    int Port { get; set; }
}