using Chaos.Common.Collections;

namespace Chaos.CommandInterceptor.Abstractions;

/// <summary>
///     Defines a command that can be executed on an object
/// </summary>
/// <typeparam name="T">The type of the object that is the source of the command</typeparam>
public interface ICommand<in T>
{
    /// <summary>
    ///     Executes the command
    /// </summary>
    /// <param name="source">The source of the command</param>
    /// <param name="args">Arguments to be used by the command</param>
    ValueTask ExecuteAsync(T source, ArgumentCollection args);
}