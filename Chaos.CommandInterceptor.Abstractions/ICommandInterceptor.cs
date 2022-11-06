namespace Chaos.CommandInterceptor.Abstractions;

/// <summary>
///     Intercepts string commands and executes actions based on the input
/// </summary>
/// <typeparam name="T">The type of the object that will be executing the commands</typeparam>
public interface ICommandInterceptor<in T>
{
    /// <summary>
    ///     Parses the pieces of the command and executes the appropriate action
    /// </summary>
    /// <param name="source">The object executing the command</param>
    /// <param name="commandStr">The full command string</param>
    ValueTask HandleCommandAsync(T source, string commandStr);

    /// <summary>
    ///     Determines whether or not a string is a valid command
    /// </summary>
    /// <param name="commandStr">A string</param>
    /// <returns><c>true</c> if the string is a command, otherwise <c>false</c></returns>
    bool IsCommand(string commandStr);
}