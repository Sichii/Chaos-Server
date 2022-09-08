using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Chaos.Core.Synchronization;
using Chaos.Exceptions;
using Chaos.Services.Security.Abstractions;
using Chaos.Services.Security.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NameReasonType = Chaos.Exceptions.UsernameCredentialException.ReasonType;
using PasswordReasonType = Chaos.Exceptions.PasswordCredentialException.ReasonType;

namespace Chaos.Services.Security;

public class ActiveDirectoryCredentialManager : ICredentialManager
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

    private string ComputeHash(string password)
    {
        #pragma warning disable SYSLIB0045
        using var algorithm = HashAlgorithm.Create(Options.HashAlgorithmName);
        #pragma warning restore SYSLIB0045
        
        var buffer = Encoding.UTF8.GetBytes(password);
        var hash = algorithm!.ComputeHash(buffer);

        return Convert.ToHexString(hash);
    }

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

    public async Task SaveNewCredentialsAsync(string name, string password)
    {
        await using var sync = await Sync.WaitAsync();

        ValidateUserNameRules(name);
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

    public async Task<bool> ValidateCredentialsAsync(string name, string password)
    {
        await using var sync = await Sync.WaitAsync();

        var ret = await InnerValidateCredentialsAsync(name, password);
        Logger.LogTrace("Validated credentials for {Name}", name);

        return ret;
    }

    private void ValidatePasswordRules(string password)
    {
        if (password.Length < Options.MinPasswordLength)
            throw new PasswordCredentialException(PasswordReasonType.TooShort);

        if (password.Length > Options.MaxPasswordLength)
            throw new PasswordCredentialException(PasswordReasonType.TooLong);
    }

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

        Logger.LogTrace("Validated username rules for {Name}", name);
    }
}