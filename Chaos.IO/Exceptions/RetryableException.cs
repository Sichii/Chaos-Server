namespace Chaos.IO.Exceptions;

/// <summary>
///     An exception that can be retried
/// </summary>
public sealed class RetryableException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RetryableException" /> class
    /// </summary>
    public RetryableException(string message, Exception? innerException = null)
        : base(message, innerException) { }
}