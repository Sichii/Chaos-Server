using System.Reflection;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.CommandInterceptor;

public class CommandInterceptor<T> : ICommandInterceptor<T>
{
    private readonly Dictionary<string, CommandDescriptor> Commands;
    private readonly CommandInterceptorConfiguration<T> Configuration;
    private readonly ILogger<CommandInterceptor<T>> Logger;
    private readonly IServiceProvider ServiceProvider;

    public CommandInterceptor(
        IServiceProvider serviceProvider,
        CommandInterceptorConfiguration<T> configuration,
        ILogger<CommandInterceptor<T>> logger
    )
    {
        ServiceProvider = serviceProvider;
        Configuration = configuration;
        Logger = logger;
        Commands = new Dictionary<string, CommandDescriptor>(StringComparer.OrdinalIgnoreCase);

        var commandTypes = TypeLoader.LoadImplementations<ICommand<T>>();

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
    public void HandleCommand(T source, string commandStr)
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
            var identifier = Configuration.IdentifierSelector(source);

            if (!Configuration.AdminPredicate(source))
            {
                Logger.LogWarning("Non-Admin {Identifier} tried to execute admin command {CommandName}", identifier, commandName);

                return;
            }

            Logger.LogInformation("Admin {Identifier} executed command {CommandName}", identifier, commandName);
        }

        var commandInstance = (ICommand<T>)ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type);

        commandInstance.Execute(source, new ArgumentCollection(commandArgs));
    }

    /// <inheritdoc />
    public bool IsCommand(string commandStr) => commandStr.StartsWith(Configuration.Prefix);
}