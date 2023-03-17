using Chaos.Collections.Common;

namespace Chaos.Messaging.Abstractions;

/// <summary>
///     Defines the pattern for a command that can be executed on an object
/// </summary>
/// <typeparam name="T">The type of the object that is the source of the command</typeparam>
public interface ICommand<in T> where T: ICommandSubject
{
    /// <summary>
    ///     Executes the command
    /// </summary>
    /// <param name="source">The source of the command</param>
    /// <param name="args">Arguments to be used by the command</param>
    ValueTask ExecuteAsync(T source, ArgumentCollection args);
}