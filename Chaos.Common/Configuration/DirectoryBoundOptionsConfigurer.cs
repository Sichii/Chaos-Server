using Chaos.Common.Abstractions;
using Microsoft.Extensions.Options;

namespace Chaos.Common.Configuration;

/// <summary>
///     Configures an options object that implements <see cref="IDirectoryBound" /> to use the staging directory
/// </summary>
/// <typeparam name="T">
///     The type to configure
/// </typeparam>
public class DirectoryBoundOptionsConfigurer<T> : IPostConfigureOptions<T> where T: class, IDirectoryBound
{
    /// <summary>
    ///     The staging directory
    /// </summary>
    protected IStagingDirectory StagingDirectory { get; }

    /// <summary>
    ///     Creates a new <see cref="DirectoryBoundOptionsConfigurer{T}" />
    /// </summary>
    public DirectoryBoundOptionsConfigurer(IStagingDirectory stagingDirectory) => StagingDirectory = stagingDirectory;

    /// <inheritdoc />
    public virtual void PostConfigure(string? name, T options) => options.UseBaseDirectory(StagingDirectory.StagingDirectory);
}