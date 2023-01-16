namespace Chaos.CommandInterceptor;

/// <summary>
///     An object use to store metadata about a command
/// </summary>
public sealed class CommandDescriptor
{
    /// <summary>
    ///     The details of the command specified in it's identifying attribute
    /// </summary>
    public required CommandAttribute Details { get; init; }

    /// <summary>
    ///     The type of the object that this command is executed by
    /// </summary>
    public required Type Type { get; init; }
}