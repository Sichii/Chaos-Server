using Chaos.Common.Abstractions;

namespace Chaos.Storage.Abstractions;

public enum SearchResultType
{
    Files = 0,
    Directories = 1
}

public interface IFileCacheOptions : IDirectoryBound
{
    string Directory { get; set; }
    string? FilePattern { get; init; }
    SearchResultType SearchResultType { get; init; }
}