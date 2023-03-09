using System.Net;

namespace Chaos.Security.Abstractions;

/// <summary>
///     Defines the methods that are required to determine if an IP address should be allowed to connect
/// </summary>
public interface IAccessManager
{
    /// <summary>
    ///     Determines whether the specified IP address should be allowed to connect
    /// </summary>
    /// <param name="ipAddress">The IP address of the client</param>
    /// <returns><c>true</c> if the IP address should be allowed to connect, otherwise <c>false</c></returns>
    Task<bool> ShouldAllowAsync(IPAddress ipAddress);
}