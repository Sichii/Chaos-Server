using Chaos.Common.Abstractions;

namespace Chaos.Storage.Abstractions;

public enum SearchType
{
    Files = 0,
    Directories = 1
}

public interface IFileCacheOptions : IDirectoryBound
{
    string Directory { get; set; }
    string? FilePattern { get; init; }
    bool Recursive { get; init; }
    SearchType SearchType { get; init; }
}