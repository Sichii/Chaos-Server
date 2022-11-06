using Chaos.Common.Abstractions;

namespace Chaos.Storage.Options;

public sealed record UserSaveManagerOptions : IDirectoryBound
{
    public required string Directory { get; set; }

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory) => Directory = Path.Combine(baseDirectory, Directory);
}