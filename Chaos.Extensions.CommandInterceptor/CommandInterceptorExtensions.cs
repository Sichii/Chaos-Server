using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     Command interceptor DI extensions
/// </summary>
public static class CommandInterceptorExtensions
{
    /// <summary>
    ///     Adds a command interceptor to the service collection, using provided details as configuration
    /// </summary>
    /// <param name="services">The servicecollection to add the command interceptor to</param>
    /// <param name="commandPrefix">The prefix the command interceptor will look for that let it determine what is or is not a command</param>
    /// <param name="isAdminPredicate">A selector used to determine whether or not the object executing the command has administrative privilege</param>
    /// <param name="idOrNameSelector">A selector used to determine the id of the object executing the command</param>
    /// <typeparam name="T">The type of object that will execute commands</typeparam>
    public static void AddCommandInterceptorForType<T>(
        this IServiceCollection services,
        string commandPrefix,
        Func<T, bool> isAdminPredicate,
        Func<T, string> idOrNameSelector
    )
    {
        var config = new CommandHandlerConfiguration<T>
        {
            Prefix = commandPrefix,
            AdminPredicate = isAdminPredicate,
            IdentifierSelector = idOrNameSelector
        };

        services.AddSingleton<ICommandInterceptor<T>, CommandHandler<T>>(
            p => ActivatorUtilities.CreateInstance<CommandHandler<T>>(p, config));
    }
}