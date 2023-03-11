using System.Reflection;
using Chaos.Collections.Common;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Extensions.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chaos.CommandInterceptor;

/// <summary>
///     A service used to handle the detection and execution of commands execute by an object of a given type
/// </summary>
/// <typeparam name="T">The type of the object executing commands</typeparam>
public sealed class CommandHandler<T> : ICommandInterceptor<T>
{
    private readonly Dictionary<string, CommandDescriptor> Commands;
    private readonly CommandHandlerConfiguration<T> Configuration;
    private readonly ILogger<CommandHandler<T>> Logger;
    private readonly IServiceProvider ServiceProvider;

    public CommandHandler(
        IServiceProvider serviceProvider,
        CommandHandlerConfiguration<T> configuration,
        ILogger<CommandHandler<T>> logger
    )
    {
        ServiceProvider = serviceProvider;
        Configuration = configuration;
        Logger = logger;
        Commands = new Dictionary<string, CommandDescriptor>(StringComparer.OrdinalIgnoreCase);

        var commandTypes = typeof(ICommand<T>).LoadImplementations();

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
    public ValueTask HandleCommandAsync(T source, string commandStr)
    {
        var command = commandStr[1..];
        var commandParts = new ArgumentCollection(command);

        if (commandParts.Count == 0)
            return default;

        if (!commandParts.TryGetNext<string>(out var commandName))
            return default;

        if (!Commands.TryGetValue(commandName, out var descriptor))
            return default;

        if (descriptor.Details.RequiresAdmin && !Configuration.AdminPredicate(source))
        {
            Logger.LogWarning("Non-Admin {@Source} tried to execute admin command {CommandName}", source, commandName);

            return default;
        }

        try
        {
            var commandInstance = (ICommand<T>)ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type);

            Logger.LogTrace("Successfully created command {CommandName}", commandName);

            var commandArgs = new ArgumentCollection(commandParts.Skip(1));

            async ValueTask InnerExecute()
            {
                await commandInstance.ExecuteAsync(source, new ArgumentCollection(commandArgs));

                Logger.LogInformation(
                    "{@Source} executed command \"{CommandName}\" with arguments \"{Args}\"",
                    source,
                    commandName,
                    commandArgs.ToString());
            }

            return InnerExecute();
        } catch (Exception e)
        {
            Logger.LogError(
                e,
                "{@Source} failed to execute command {@Command}",
                source,
                descriptor);
        }

        return default;
    }

    /// <inheritdoc />
    public bool IsCommand(string commandStr) => commandStr.StartsWithI(Configuration.Prefix);
}