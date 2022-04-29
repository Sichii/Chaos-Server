using System;

namespace Chaos.Exceptions;

public class PasswordCredentialException : Exception
{
    public enum ReasonType
    {
        TooShort = 1,
        TooLong = 2,
        WrongPassword = 3
    }

    public ReasonType Reason { get; set; }

    public PasswordCredentialException(ReasonType reason) => Reason = reason;
}