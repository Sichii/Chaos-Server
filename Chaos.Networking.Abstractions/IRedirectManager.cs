using System.Diagnostics.CodeAnalysis;

namespace Chaos.Networking.Abstractions;

/// <summary>
///     Defines the properties and methods for managing redirects
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
    bool TryGetRemove(uint id, [MaybeNullWhen(false)] out IRedirect redirect);
}