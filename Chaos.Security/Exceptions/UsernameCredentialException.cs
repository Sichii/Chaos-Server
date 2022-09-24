namespace Chaos.Security.Exceptions;

public class UsernameCredentialException : Exception
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

    public string Name { get; set; }
    public ReasonType Reason { get; set; }

    public UsernameCredentialException(string name, ReasonType reason)
    {
        Name = name;
        Reason = reason;
    }
}