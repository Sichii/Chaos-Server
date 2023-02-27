using Chaos.Security.Abstractions;

namespace Chaos.Security.Definitions;

/// <summary>
///     The mode in which that the <see cref="IAccessManager" /> operates
/// </summary>
public enum IpAccessMode
{
    /// <summary>
    ///     Allows all connections except those that are blacklisted
    /// </summary>
    Blacklist,
    /// <summary>
    ///     Disallows all connections except those that are whitelisted
    /// </summary>
    Whitelist
}