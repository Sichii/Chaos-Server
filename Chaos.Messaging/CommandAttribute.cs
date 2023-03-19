namespace Chaos.Messaging;

/// <summary>
///     Used to mark command objects that can be built and executed by the <see cref="Chaos.Messaging.CommandInterceptor{T, TOptions}" />.
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

    /// <summary>
    ///     Creates a new instance of the <see cref="CommandAttribute" /> class.
    /// </summary>
    /// <param name="commandName"></param>
    /// <param name="requiresAdmin"></param>
    public CommandAttribute(string commandName, bool requiresAdmin = true)
    {
        CommandName = commandName;
        RequiresAdmin = requiresAdmin;
    }
}