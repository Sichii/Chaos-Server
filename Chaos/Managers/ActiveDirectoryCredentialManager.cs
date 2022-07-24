using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Chaos.Core.Synchronization;
using Chaos.Core.Utilities;
using Chaos.Exceptions;
using Chaos.Managers.Interfaces;
using Chaos.Options;
using Microsoft.Extensions.Options;
using NameReasonType = Chaos.Exceptions.UsernameCredentialException.ReasonType;
using PasswordReasonType = Chaos.Exceptions.PasswordCredentialException.ReasonType;

namespace Chaos.Managers;

public class ActiveDirectoryCredentialManager : ICredentialManager
{
    private readonly ActiveDirectoryCredentialManagerOptions Options;
    private readonly AutoReleasingSemaphoreSlim Sync;

    public ActiveDirectoryCredentialManager(IOptionsSnapshot<ActiveDirectoryCredentialManagerOptions> options)
    {
        Options = options.Value;
        Sync = new AutoReleasingSemaphoreSlim(1, 1);

        if (!Directory.Exists(Options.Directory))
            Directory.CreateDirectory(Options.Directory);
    }

    public async Task ChangePasswordAsync(string name, string oldPassword, string newPassword)
    {
        await using var sync = await Sync.WaitAsync();

        if (!await InnerValidateCredentialsAsync(name, oldPassword))
            throw new PasswordCredentialException(PasswordReasonType.WrongPassword);

        ValidatePassword(newPassword);

        var newHash = ComputeHash(newPassword);
        var passwordPath = Path.Combine(Options.Directory, name, "password.txt");

        await File.WriteAllTextAsync(passwordPath, newHash);
    }

    private string ComputeHash(string password)
    {
        using var algorithm = HashAlgorithm.Create(Options.HashAlgorithmName);
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

        ValidateUserName(name);
        ValidatePassword(password);

        var characterDir = Path.Combine(Options.Directory, name);

        if (Directory.Exists(characterDir))
            throw new UsernameCredentialException(name, NameReasonType.AlreadyExists);

        Directory.CreateDirectory(characterDir);
        var hash = ComputeHash(password);

        var passwordPath = Path.Combine(characterDir, "password.txt");
        await File.WriteAllTextAsync(passwordPath, hash);
    }

    public async Task<bool> ValidateCredentialsAsync(string name, string password)
    {
        await using var sync = await Sync.WaitAsync();

        return await InnerValidateCredentialsAsync(name, password);
    }

    private void ValidatePassword(string password)
    {
        if (password.Length < Options.MinUsernameLength)
            throw new PasswordCredentialException(PasswordReasonType.TooShort);

        if (password.Length > Options.MaxPasswordLength)
            throw new PasswordCredentialException(PasswordReasonType.TooLong);
    }

    private void ValidateUserName(string name)
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