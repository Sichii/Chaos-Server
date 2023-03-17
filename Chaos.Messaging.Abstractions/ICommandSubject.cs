namespace Chaos.Messaging.Abstractions;

/// <summary>
///     An interface used to mark objects that can be used to execute commands
/// </summary>
public interface ICommandSubject
{
    /// <summary>
    ///     Whether or not the subject has admin privileges
    /// </summary>
    bool IsAdmin { get; }
}