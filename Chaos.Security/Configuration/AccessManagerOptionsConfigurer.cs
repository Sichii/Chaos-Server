using System.Text.RegularExpressions;
using Chaos.Common.Abstractions;
using Chaos.Common.Configuration;
using Chaos.Security.Options;

namespace Chaos.Security.Configuration;

/// <summary>
///     Configures the <see cref="AccessManagerOptions" />
/// </summary>
public sealed class AccessManagerOptionsConfigurer : DirectoryBoundOptionsConfigurer<AccessManagerOptions>
{
    /// <inheritdoc />
    public AccessManagerOptionsConfigurer(IStagingDirectory stagingDirectory)
        : base(stagingDirectory) { }

    /// <inheritdoc />
    public override void PostConfigure(string? name, AccessManagerOptions options)
    {
        options.ValidCharactersRegex = new Regex(options.ValidCharactersPattern, RegexOptions.Compiled);
        options.ValidFormatRegex = new Regex(options.ValidFormatPattern, RegexOptions.Compiled);
        base.PostConfigure(name, options);
    }
}