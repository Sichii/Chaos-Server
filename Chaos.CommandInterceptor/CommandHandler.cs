using System.Reflection;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.CommandInterceptor.Definitions;
using Chaos.Common.Collections;
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

        var commandParts = RegexCache.COMMAND_SPLIT_REGEX.Matches(command)
                                     .Select(
                                         match =>
                                         {
                                             if (!string.IsNullOrWhiteSpace(match.Groups[1].Value))
                                                 return match.Groups[1].Value;

                                             return match.Groups[2].Value;
                                         })
                                     .ToArray();

        if (commandParts.Length == 0)
            return default;

        var commandName = commandParts[0];
        var commandArgs = commandParts[1..];

        if (!Commands.TryGetValue(commandName, out var descriptor))
            return default;

        if (descriptor.Details.RequiresAdmin)
        {
            if (!Configuration.AdminPredicate(source))
            {
                Logger.LogWarning("Non-Admin {Source} tried to execute admin command {CommandName}", source, commandName);

                return default;
            }

            Logger.LogInformation("Admin {Source} executed command {CommandName}", source, commandName);
        }

        var commandInstance = (ICommand<T>)ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type);

        return commandInstance.ExecuteAsync(source, new ArgumentCollection(commandArgs));
    }

    /// <inheritdoc />
    public bool IsCommand(string commandStr) => commandStr.StartsWithI(Configuration.Prefix);
}