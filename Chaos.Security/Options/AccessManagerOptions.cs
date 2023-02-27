using Chaos.Common.Abstractions;
using Chaos.Security.Definitions;

namespace Chaos.Security.Options;

public sealed class AccessManagerOptions : IDirectoryBound
{
    /// <summary>
    ///     The relative directory where the IP manager will store its files
    /// </summary>
    public string Directory { get; set; } = null!;
    /// <summary>
    ///     The mode in which that the ip manager operates
    /// </summary>
    public IpAccessMode Mode { get; set; }

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory) => Directory = Path.Combine(baseDirectory, Directory);
}