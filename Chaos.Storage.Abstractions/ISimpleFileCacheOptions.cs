using Chaos.Common.Abstractions;

namespace Chaos.Storage.Abstractions;

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

/// <summary>
///     Defines the properties required to configure a <see cref="SimpleFileCacheBase{T,TSchema,TOptions}"/>
/// </summary>
public interface ISimpleFileCacheOptions : IDirectoryBound
{
    /// <summary>
    ///     The directory to search relative to the staging directory
    /// </summary>
    string Directory { get; set; }
    
    /// <summary>
    ///     The file pattern to use when searching
    /// </summary>
    string? FilePattern { get; init; }
    
    /// <summary>
    ///     Whether or not the search is recursive. A recursive search will search all subdirectories
    /// </summary>
    bool Recursive { get; init; }
    
    /// <summary>
    ///     The type of search to perform. This determins what kind of paths are returned
    /// </summary>
    SearchType SearchType { get; init; }
}

/* If i decide to use IMemoryCache
public interface IFileCacheOptions : ISimpleFileCacheOptions
{
    /// <summary>
    ///     The amount of time an entry in the cache will go unused before it is removed
    /// </summary>
    int ExpirationMins { get; init; }
}*/