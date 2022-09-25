namespace Chaos.CommandInterceptor.Abstractions;

/// <summary>
///     Represents a command
/// </summary>
/// <typeparam name="T">The type of the object that is the source of the command</typeparam>
public interface ICommand<in T>
{
    /// <summary>
    ///     Executes the command
    /// </summary>
    /// <param name="source">The source of the command</param>
    /// <param name="args">Arguments to be used by the command</param>
    void Execute(T source, params string[] args);
}