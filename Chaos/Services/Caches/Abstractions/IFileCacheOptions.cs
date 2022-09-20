namespace Chaos.Services.Caches.Abstractions;

public enum SearchResultType
{
    Files = 0,
    Directories = 1
}

public interface IFileCacheOptions
{
    string Directory { get; set; }
    string? FilePattern { get; init; }
    SearchResultType SearchResultType { get; init; }

    public void UseRootDirectory(string rootDirectory);
}