using System.Reflection;
using Chaos.Commands.Abstractions;
using Chaos.Core.Utilities;
using Chaos.Objects.World;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.Commands;

public class CommandInterceptor : ICommandInterceptor
{
    private readonly Dictionary<string, CommandDescriptor> Commands;
    private readonly ILogger<CommandInterceptor> Logger;
    private readonly IServiceProvider ServiceProvider;

    public CommandInterceptor(IServiceProvider serviceProvider, ILogger<CommandInterceptor> logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
        Commands = new Dictionary<string, CommandDescriptor>(StringComparer.OrdinalIgnoreCase);

        var commandTypes = TypeLoader.LoadImplementations<ICommand>();

        foreach (var type in commandTypes)
        {
            var attribute = type.GetCustomAttribute<CommandAttribute>();

            if (attribute == null)
                continue;

            var descriptor = new CommandDescriptor
            {
                Details = attribute,
                Type = type
            };

            Commands.Add(attribute.CommandName, descriptor);
        }
    }

    /// <inheritdoc />
    public void HandleCommand(Aisling aisling, string commandStr)
    {
        var command = commandStr[1..];
        var commandParts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (commandParts.Length == 0)
            return;

        var commandName = commandParts[0];
        var commandArgs = commandParts[1..];

        if (!Commands.TryGetValue(commandName, out var descriptor))
            return;

        if (descriptor.Details.RequiresAdmin)
        {
            if (!aisling.IsAdmin)
            {
                Logger.LogWarning("Non-Admin {Username} tried to execute admin command {CommandName}", aisling.Name, commandName);

                return;
            }

            Logger.LogInformation("Admin {Username} executed command {CommandName}", aisling.Name, commandName);
        }

        var commandInstance = (ICommand)ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type);

        commandInstance.Execute(aisling, commandArgs);
    }

    /// <inheritdoc />
    public bool IsCommand(string commandStr) => commandStr.StartsWith('/');
}