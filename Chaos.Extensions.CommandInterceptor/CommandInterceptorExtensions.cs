using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class CommandInterceptorExtensions
{
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