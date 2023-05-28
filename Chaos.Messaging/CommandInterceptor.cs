using System.Reflection;
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Messaging;

/// <summary>
///     A service used to handle the detection and execution of commands execute by an object of a given type
/// </summary>
/// <typeparam name="T">The type of the object executing commands</typeparam>
/// <typeparam name="TOptions">The type of the options object to use for this command interceptor</typeparam>
public sealed class CommandInterceptor<T, TOptions> : ICommandInterceptor<T> where T: ICommandSubject
                                                                             where TOptions: class, ICommandInterceptorOptions
{
    private readonly Dictionary<string, CommandDescriptor> Commands;
    private readonly ILogger<CommandInterceptor<T, TOptions>> Logger;
    private readonly TOptions Options;
    private readonly IServiceProvider ServiceProvider;

    /// <summary>
    ///     Creates a new instance of <see cref="CommandInterceptor{T, TOptions}" />
    /// </summary>
    public CommandInterceptor(
        IServiceProvider serviceProvider,
        IOptions<TOptions> options,
        ILogger<CommandInterceptor<T, TOptions>> logger
    )
    {
        ServiceProvider = serviceProvider;
        Options = options.Value;
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
    /// <remarks>async is intentional, so that the try/catch handles any exception that comes from executing the command</remarks>
    public async ValueTask HandleCommandAsync(T source, string commandStr)
    {
        var command = commandStr[1..];
        var commandParts = new ArgumentCollection(command);

        if (commandParts.Count == 0)
            return;

        if (!commandParts.TryGetNext<string>(out var commandName))
            return;

        if (!Commands.TryGetValue(commandName, out var descriptor))
            return;

        if (descriptor.Details.RequiresAdmin && !source.IsAdmin)
        {
            Logger.WithProperties(source, descriptor, commandStr)
                  .LogWarning("Non-Admin {@SourceType} tried to execute admin command {@CommandStr}", source.GetType().Name, commandStr);

            return;
        }

        try
        {
            var commandInstance = (ICommand<T>)ActivatorUtilities.CreateInstance(ServiceProvider, descriptor.Type);

            Logger.WithProperties(source, descriptor, commandStr)
                  .LogTrace("Successfully created command {@CommandName}", commandName);

            var commandArgs = new ArgumentCollection(commandParts.Skip(1));

            async ValueTask InnerExecute()
            {
                await commandInstance.ExecuteAsync(source, new ArgumentCollection(commandArgs));

                Logger.WithProperties(source, descriptor)
                      .LogInformation("{@SourceType} executed {@CommandStr}", source.GetType().Name, commandStr);
            }

            await InnerExecute();
        } catch (Exception e)
        {
            Logger.WithProperties(source, descriptor)
                  .LogError(
                      e,
                      "{@SourceType} failed to execute {@Command}",
                      source.GetType().Name,
                      commandStr);
        }
    }

    /// <inheritdoc />
    public bool IsCommand(string commandStr) => commandStr.StartsWithI(Options.Prefix);
}