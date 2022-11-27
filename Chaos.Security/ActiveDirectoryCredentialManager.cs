using System.Security.Cryptography;
using System.Text;
using Chaos.Common.Synchronization;
using Chaos.Extensions.Common;
using Chaos.Security.Abstractions;
using Chaos.Security.Exceptions;
using Chaos.Security.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NameReasonType = Chaos.Security.Exceptions.UsernameCredentialException.ReasonType;
using PasswordReasonType = Chaos.Security.Exceptions.PasswordCredentialException.ReasonType;

namespace Chaos.Security;

/// <summary>
///     A <see cref="Chaos.Security.Abstractions.ICredentialManager" /> that uses a folder to represent a user
/// </summary>
public sealed class ActiveDirectoryCredentialManager : ICredentialManager
{
    private readonly ILogger Logger;
    private readonly ActiveDirectoryCredentialManagerOptions Options;
    private readonly AutoReleasingSemaphoreSlim Sync;

    public ActiveDirectoryCredentialManager(
        IOptionsSnapshot<ActiveDirectoryCredentialManagerOptions> options,
        ILogger<ActiveDirectoryCredentialManager> logger
    )
    {
        Options = options.Value;
        Logger = logger;
        Sync = new AutoReleasingSemaphoreSlim(1, 1);

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(string name, string oldPassword, string newPassword)
    {
        await using var sync = await Sync.WaitAsync();

        Logger.LogDebug("Changing password for {Name}", name);

        if (!await InnerValidateCredentialsAsync(name, oldPassword))
            throw new PasswordCredentialException(PasswordReasonType.WrongPassword);

        ValidatePasswordRules(newPassword);
        Logger.LogTrace("Validated password rules for {Name}", name);

        var newHash = ComputeHash(newPassword);
        var passwordPath = Path.Combine(Options.Directory, name, "password.txt");

        await File.WriteAllTextAsync(passwordPath, newHash);
        Logger.LogTrace("Changed password for {Name}", name);
    }

    /// <summary>
    ///     Computes a hash for the given password
    /// </summary>
    /// <param name="password">The password to hash</param>
    private string ComputeHash(string password)
    {
        #pragma warning disable SYSLIB0045
        using var algorithm = HashAlgorithm.Create(Options.HashAlgorithmName);
        #pragma warning restore SYSLIB0045

        var buffer = Encoding.UTF8.GetBytes(password);
        var hash = algorithm!.ComputeHash(buffer);

        return Convert.ToHexString(hash);
    }

    /// <summary>
    ///     Validates a username and password combination
    /// </summary>
    /// <param name="name">The name to validate</param>
    /// <param name="password">The password to validate</param>
    /// <returns><c>true</c> if the username exists, and the password specified matches the record, otherwise <c>false</c></returns>
    /// <exception cref="UsernameCredentialException">Username doesn't exist</exception>
    private async Task<bool> InnerValidateCredentialsAsync(string name, string password)
    {
        var characterDir = Path.Combine(Options.Directory, name);
        var passwordPath = Path.Combine(characterDir, "password.txt");

        if (!Directory.Exists(characterDir))
            throw new UsernameCredentialException(name, NameReasonType.DoesntExist);

        var givenHash = ComputeHash(password);
        var actualHash = await File.ReadAllTextAsync(passwordPath);

        return givenHash.EqualsI(actualHash);
    }

    /// <inheritdoc />
    public async Task SaveNewCredentialsAsync(string name, string password)
    {
        await using var sync = await Sync.WaitAsync();

        ValidateUserNameRules(name);
        Logger.LogTrace("Validated username rules for {Name}", name);
        ValidatePasswordRules(password);
        Logger.LogTrace("Validated password rules for {Name}", name);

        var characterDir = Path.Combine(Options.Directory, name);

        if (Directory.Exists(characterDir))
            throw new UsernameCredentialException(name, NameReasonType.AlreadyExists);

        Directory.CreateDirectory(characterDir);
        var hash = ComputeHash(password);

        var passwordPath = Path.Combine(characterDir, "password.txt");
        await File.WriteAllTextAsync(passwordPath, hash);
        Logger.LogTrace("Saved new credentials for {Name}", name);
    }

    /// <inheritdoc />
    public async Task<bool> ValidateCredentialsAsync(string name, string password)
    {
        await using var sync = await Sync.WaitAsync();

        var ret = await InnerValidateCredentialsAsync(name, password);
        Logger.LogTrace("Validated credentials for {Name}", name);

        return ret;
    }

    /// <summary>
    ///     Validates a password against the rules specified in the configuration
    /// </summary>
    /// <param name="password">The password to validate</param>
    /// <exception cref="PasswordCredentialException">Invalid password</exception>
    private void ValidatePasswordRules(string password)
    {
        if (password.Length < Options.MinPasswordLength)
            throw new PasswordCredentialException(PasswordReasonType.TooShort);

        if (password.Length > Options.MaxPasswordLength)
            throw new PasswordCredentialException(PasswordReasonType.TooLong);
    }

    /// <summary>
    ///     Validates a username against the rules specified in the configuration
    /// </summary>
    /// <param name="name">The name to validate</param>
    /// <exception cref="UsernameCredentialException">Invalid username</exception>
    private void ValidateUserNameRules(string name)
    {
        if (!Options.ValidCharactersRegex.IsMatch(name))
            throw new UsernameCredentialException(name, NameReasonType.InvalidCharacters);

        if (name.Length > Options.MaxUsernameLength)
            throw new UsernameCredentialException(name, NameReasonType.TooLong);

        if (name.Length < Options.MinUsernameLength)
            throw new UsernameCredentialException(name, NameReasonType.TooShort);

        if (!Options.ValidCharactersRegex.IsMatch(name))
            throw new UsernameCredentialException(name, NameReasonType.InvalidFormat);

        if (Options.ReservedUsernames.ContainsI(name))
            throw new UsernameCredentialException(name, NameReasonType.Reserved);

        foreach (var filtered in Options.PhraseFilter)
            if (name.ContainsI(filtered))
                throw new UsernameCredentialException(name, NameReasonType.NotAllowed);
    }
}