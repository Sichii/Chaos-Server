namespace Chaos.Security.Abstractions;

/// <summary>
///     Represents the result of a credential validation attempt
/// </summary>
public sealed class CredentialValidationResult
{
    /// <summary>
    ///     The code that represents the result of the validation
    /// </summary>
    public enum FailureCode
    {
        /// <summary>
        ///     The validation succeeded
        /// </summary>
        Success,
        /// <summary>
        ///     The validation failed because the provided username was invalid
        /// </summary>
        InvalidUsername,
        /// <summary>
        ///     The validation failed because the provided password was invalid
        /// </summary>
        InvalidPassword,
        /// <summary>
        ///     The validation failed because the provided password was too long
        /// </summary>
        PasswordTooLong,
        /// <summary>
        ///     The validation failed because the provided password was too short
        /// </summary>
        PasswordTooShort,
        /// <summary>
        ///     The validation failed because the provided username was too long
        /// </summary>
        UsernameTooLong,
        /// <summary>
        ///     The validation failed because the provided username was too short
        /// </summary>
        UsernameTooShort,
        /// <summary>
        ///     The validation failed because the provided username was not allowed
        /// </summary>
        UsernameNotAllowed,
        /// <summary>
        ///     The validation failed because the ip has failed validation too many times
        /// </summary>
        TooManyAttempts
    }

    /// <summary>
    ///     The code that represents the result of the validation
    /// </summary>
    public FailureCode Code { get; init; }

    /// <summary>
    ///     The message to display if the validation failed
    /// </summary>
    public string? FailureMessage { get; init; }

    /// <summary>
    ///     Whether or not the validation succeeded
    /// </summary>
    public bool Success => string.IsNullOrEmpty(FailureMessage);

    /// <summary>
    ///     The result of a successful credential validation
    /// </summary>
    public static CredentialValidationResult SuccessResult { get; } = new();
}