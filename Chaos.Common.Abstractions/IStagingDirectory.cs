namespace Chaos.Common.Abstractions;

/// <summary>
///     Literally just a staging directory that can be more easily pulled out of a container
/// </summary>
public interface IStagingDirectory
{
    string StagingDirectory { get; }
}