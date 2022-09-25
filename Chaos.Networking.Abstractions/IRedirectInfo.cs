using System.Net;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents a configuration object to set up redirects that will be added to the login notice
/// </summary>
public interface IRedirectInfo
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