namespace Chaos.Storage;

/// <summary>
///     Options for an <see cref="EntityRepository" />.
/// </summary>
public sealed class EntityRepositoryOptions
{
    /// <summary>
    ///     Whether or not to use atomic saves. This will produce a .bak file in the same directory as the file being saved
    /// </summary>
    public bool SafeSaves { get; set; }
}