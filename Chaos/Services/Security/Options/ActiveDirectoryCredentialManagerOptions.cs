using System.Text.RegularExpressions;

namespace Chaos.Services.Security.Options;

public record ActiveDirectoryCredentialManagerOptions
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

    public static void PostConfigure(ActiveDirectoryCredentialManagerOptions options)
    {
        options.ValidCharactersRegex = new Regex(options.ValidCharactersPattern, RegexOptions.Compiled);
        options.ValidFormatRegex = new Regex(options.ValidFormatPattern, RegexOptions.Compiled);
    }
}