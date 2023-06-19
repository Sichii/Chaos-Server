namespace Chaos.Messaging;

/// <summary>
///     Used to mark command objects that can be built and executed by the <see cref="Chaos.Messaging.CommandInterceptor{T, TOptions}" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class CommandAttribute : Attribute
{
    /// <summary>
    ///     The name of the command.
    /// </summary>
    public string CommandName { get; }

    /// <summary>
    ///     The help text for the command.
    /// </summary>
    public string? HelpText { get; }

    /// <summary>
    ///     Whether or not the command requires admin privileges.
    /// </summary>
    public bool RequiresAdmin { get; }

    /// <summary>
    ///     Creates a new instance of the <see cref="CommandAttribute" /> class.
    /// </summary>
    /// <param name="commandName">The name of the command</param>
    /// <param name="requiresAdmin">Whether or not the command requires admin privileges</param>
    /// <param name="helpText">The help text of the command</param>
    public CommandAttribute(string commandName, bool requiresAdmin = true, string? helpText = null)
    {
        CommandName = commandName;
        RequiresAdmin = requiresAdmin;
        HelpText = helpText;
    }
}