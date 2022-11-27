namespace Chaos.Security.Exceptions;

/// <summary>
///     An exception thrown when a username is invalid
/// </summary>
public sealed class UsernameCredentialException : Exception
{
    public enum ReasonType
    {
        InvalidFormat = 0,
        TooLong = 1,
        TooShort = 2,
        InvalidCharacters = 3,
        Reserved = 4,
        NotAllowed = 5,
        AlreadyExists = 6,
        DoesntExist = 7,
        Unknown = 255
    }

    /// <summary>
    ///     The captured named that is invalid
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     The reason why <see cref="Name" /> is invalid
    /// </summary>
    public ReasonType Reason { get; set; }

    public UsernameCredentialException(string name, ReasonType reason)
    {
        Name = name;
        Reason = reason;
    }
}