namespace Chaos.Security.Definitions;

/// <summary>
///     The mode in which that the <see cref="Chaos.Security.Abstractions.IIpManager" /> operates
/// </summary>
public enum IpManagerMode
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