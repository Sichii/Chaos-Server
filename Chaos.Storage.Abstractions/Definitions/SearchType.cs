namespace Chaos.Storage.Abstractions.Definitions;

/// <summary>
///     The storage interface to look for
/// </summary>
public enum SearchType
{
    /// <summary>
    ///     The search will return paths to files
    /// </summary>
    Files = 0,
    /// <summary>
    ///     The search will return directory paths
    /// </summary>
    Directories = 1
}