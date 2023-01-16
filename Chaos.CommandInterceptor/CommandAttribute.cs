namespace Chaos.CommandInterceptor;

/// <summary>
///     Used to mark command objects that can be built and executed by the <see cref="CommandInterceptor" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CommandAttribute : Attribute
{
    /// <summary>
    ///     The name of the command.
    /// </summary>
    public string CommandName { get; }

    /// <summary>
    ///     Whether or not the command requires admin privileges.
    /// </summary>
    public bool RequiresAdmin { get; }

    public CommandAttribute(string commandName, bool requiresAdmin = true)
    {
        CommandName = commandName;
        RequiresAdmin = requiresAdmin;
    }
}