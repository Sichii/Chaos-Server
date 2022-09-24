using System.Text.RegularExpressions;
using Chaos.Common.Abstractions;

namespace Chaos.Security.Options;

public record ActiveDirectoryCredentialManagerOptions : IDirectoryBound
{
    public string Directory { get; set; } = null!;
    public string HashAlgorithmName { get; set; } = null!;
    public int MaxPasswordLength { get; set; }
    public int MaxUsernameLength { get; set; }
    public int MinPasswordLength { get; set; }
    public int MinUsernameLength { get; set; }
    public string[] PhraseFilter { get; set; } = Array.Empty<string>();
    public string[] ReservedUsernames { get; set; } = Array.Empty<string>();
    public string ValidCharactersPattern { get; set; } = null!;
    public Regex ValidCharactersRegex { get; set; } = null!;
    public string ValidFormatPattern { get; set; } = null!;
    public Regex ValidFormatRegex { get; set; } = null!;

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory) => Directory = Path.Combine(baseDirectory, Directory);
}