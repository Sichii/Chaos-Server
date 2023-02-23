using System.Text.RegularExpressions;
using Chaos.Common.Abstractions;

namespace Chaos.Security.Options;

/// <summary>
///     Configuration options for <see cref="SimpleCredentialManager" />
/// </summary>
public sealed record SimpleCredentialManagerOptions : IDirectoryBound
{
    /// <summary>
    ///     The relative directory where the credentials are stored
    /// </summary>
    public string Directory { get; set; } = null!;

    /// <summary>
    ///     The name of the hash algorithm used to hash passwords
    /// </summary>
    public string HashAlgorithmName { get; set; } = null!;

    /// <summary>
    ///     The maximum number of characters allowed in a password
    /// </summary>
    public int MaxPasswordLength { get; set; }

    /// <summary>
    ///     The maximum number of characters allowed in a username
    /// </summary>
    public int MaxUsernameLength { get; set; }

    /// <summary>
    ///     The minimum number of characters allowed in a password
    /// </summary>
    public int MinPasswordLength { get; set; }

    /// <summary>
    ///     The minimum number of characters allowed in a username
    /// </summary>
    public int MinUsernameLength { get; set; }

    /// <summary>
    ///     A list of phrases/words that a username can not contains
    /// </summary>
    public string[] PhraseFilter { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A list of usernames that are not allowed to be used
    /// </summary>
    public string[] ReservedUsernames { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A regular expression used to validate that a username only contains valid characters
    /// </summary>
    public string ValidCharactersPattern { get; set; } = null!;

    /// <summary>
    ///     A compiled regular expression used to validate that a username only contains valid characters
    /// </summary>
    public Regex ValidCharactersRegex { get; set; } = null!;

    /// <summary>
    ///     A regular expression used to validate that a username is in the correct format
    /// </summary>
    public string ValidFormatPattern { get; set; } = null!;

    /// <summary>
    ///     A compiled regular expression used to validate that a username is in the correct format
    /// </summary>
    public Regex ValidFormatRegex { get; set; } = null!;

    /// <inheritdoc />
    public void UseBaseDirectory(string baseDirectory) => Directory = Path.Combine(baseDirectory, Directory);
}