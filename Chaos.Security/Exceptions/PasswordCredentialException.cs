namespace Chaos.Security.Exceptions;

/// <summary>
///     An exception thrown when a password is invalid
/// </summary>
public sealed class PasswordCredentialException : Exception
{
    public enum ReasonType
    {
        TooShort = 1,
        TooLong = 2,
        WrongPassword = 3
    }

    /// <summary>
    ///     The reason the password is invalid
    /// </summary>
    public ReasonType Reason { get; set; }

    public PasswordCredentialException(ReasonType reason) => Reason = reason;
}