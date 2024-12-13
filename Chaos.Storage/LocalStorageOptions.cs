#region
using Chaos.Common.Abstractions;
#endregion

namespace Chaos.Storage;

/// <summary>
///     Options for local storage
/// </summary>
public class LocalStorageOptions : IDirectoryBound
{
    /// <summary>
    ///     The directory to store objects in
    /// </summary>
    public string Directory { get; set; } = null!;

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory) => Directory = Path.Combine(baseDirectory, Directory);
}