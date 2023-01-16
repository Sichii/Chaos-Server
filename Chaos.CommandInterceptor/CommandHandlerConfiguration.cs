namespace Chaos.CommandInterceptor;

/// <summary>
///     Stores the configuration of a command interceptor
/// </summary>
/// <typeparam name="T">The type of the object executing commands</typeparam>
public sealed class CommandHandlerConfiguration<T>
{
    /// <summary>
    ///     A function used to determine if the executing object has admin privileges
    /// </summary>
    public required Func<T, bool> AdminPredicate { get; set; }

    /// <summary>
    ///     The prefix used to identify commands
    /// </summary>
    public required string Prefix { get; set; }
}