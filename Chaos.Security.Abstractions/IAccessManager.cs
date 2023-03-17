using System.Net;

namespace Chaos.Security.Abstractions;

/// <summary>
///     Defines the methods that are required to determine if an IP address should be allowed to connect
/// </summary>
public interface IAccessManager
{
    /// <summary>
    ///     Changes the password for the specified character
    /// </summary>
    /// <param name="ipAddress">The ip address of the client</param>
    /// <param name="name">The name of the character</param>
    /// <param name="oldPassword">The current password for the character</param>
    /// <param name="newPassword">The new password for the character</param>
    Task<CredentialValidationResult> ChangePasswordAsync(
        IPAddress ipAddress,
        string name,
        string oldPassword,
        string newPassword
    );

    /// <summary>
    ///     Saves a new name and password combination
    /// </summary>
    /// <param name="ipAddress">The ip address of the client</param>
    /// <param name="name">The name of the character</param>
    /// <param name="password">The password for the character</param>
    Task<CredentialValidationResult> SaveNewCredentialsAsync(IPAddress ipAddress, string name, string password);

    /// <summary>
    ///     Determines whether the specified IP address should be allowed to connect
    /// </summary>
    /// <param name="ipAddress">The IP address of the client</param>
    /// <returns><c>true</c> if the IP address should be allowed to connect, otherwise <c>false</c></returns>
    Task<bool> ShouldAllowAsync(IPAddress ipAddress);

    /// <summary>
    ///     Validates the specified name and password combination against existing credentials
    /// </summary>
    /// <param name="ipAddress">The ip address of the client</param>
    /// <param name="name">The name of the character</param>
    /// <param name="password">The password for the character</param>
    /// <returns><c>true</c> if the provided credentials are correct, otherwise <c>false</c></returns>
    Task<CredentialValidationResult> ValidateCredentialsAsync(IPAddress ipAddress, string name, string password);
}