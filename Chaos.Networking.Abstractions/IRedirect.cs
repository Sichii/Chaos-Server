#region
using System.Net;
using Chaos.DarkAges.Definitions;
#endregion

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Represents an object used to instruct and track a client during a redirect operation
/// </summary>
public interface IRedirect
{
    /// <summary>
    ///     The time the redirect was created
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    ///     The endpoint to redirect the client to
    /// </summary>
    IPEndPoint EndPoint { get; }

    /// <summary>
    ///     A unique id specific to this redirect
    /// </summary>
    uint Id { get; }

    /// <summary>
    ///     The cryptographic key used by the client
    /// </summary>
    string Key { get; }

    /// <summary>
    ///     The client side id of the client
    /// </summary>
    uint? LoginId1 { get; }

    /// <summary>
    ///     The client side id of the client (2)
    /// </summary>
    ushort? LoginId2 { get; }

    /// <summary>
    ///     The name of the client
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The cryptographic seed used by the client
    /// </summary>
    byte Seed { get; }

    /// <summary>
    ///     The type of server the client is being redirected to
    /// </summary>
    ServerType Type { get; }
}