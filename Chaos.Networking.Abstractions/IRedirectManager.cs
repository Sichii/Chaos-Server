namespace Chaos.Networking.Abstractions;

/// <summary>
///     An object used to manage redirects
/// </summary>
public interface IRedirectManager
{
    /// <summary>
    ///     Adds a redirect that is in progress
    /// </summary>
    void Add(IRedirect redirect);

    /// <summary>
    ///     Tries to remove a redirect that should be in progress
    /// </summary>
    bool TryGetRemove(uint id, out IRedirect redirect);
}