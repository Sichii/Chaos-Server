namespace Chaos.Security.Abstractions;

/// <summary>
///     Defines the methods that are required manage credentials for a Dark Ages server
/// </summary>
public interface ICredentialManager
{
    /// <summary>
    ///     Changes the password for the specified character
    /// </summary>
    /// <param name="name">The name of the character</param>
    /// <param name="oldPassword">The current password for the character</param>
    /// <param name="newPassword">The new password for the character</param>
    Task ChangePasswordAsync(string name, string oldPassword, string newPassword);

    /// <summary>
    ///     Saves a new name and password combination
    /// </summary>
    /// <param name="name">The name of the character</param>
    /// <param name="password">The password for the character</param>
    Task SaveNewCredentialsAsync(string name, string password);

    /// <summary>
    ///     Validates the specified name and password combination against existing credentials
    /// </summary>
    /// <param name="name">The name of the character</param>
    /// <param name="password">The password for the character</param>
    /// <returns><c>true</c> if the provided credentials are correct, otherwise <c>false</c></returns>
    Task<bool> ValidateCredentialsAsync(string name, string password);
}