namespace Chaos.Commands;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CommandAttribute : Attribute
{
    public string CommandName { get; }
    public bool RequiresAdmin { get; }

    public CommandAttribute(string commandName, bool requiresAdmin = true)
    {
        CommandName = commandName;
        RequiresAdmin = requiresAdmin;
    }
}