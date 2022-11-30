using Chaos.Common.Abstractions;
using Chaos.Storage.Abstractions.Definitions;

namespace Chaos.Storage.Abstractions;

/// <summary>
///     Defines the properties required to configure a <see cref="Chaos.Storage.Abstractions.SimpleFileCacheBase{T,TSchema,TOptions}" />
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