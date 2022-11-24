using Chaos.Scripting;
using Chaos.Scripting.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

/// <summary>
///     <see cref="Chaos.Scripting"/> DI extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     A shorthand way to add a script factory for a given <see cref="IScript"/> and <see cref="IScripted"/> type pair
    /// </summary>
    /// <param name="services">The service collection to add the service to</param>
    /// <typeparam name="TScript">A type that inherits <see cref="IScript"/></typeparam>
    /// <typeparam name="TScripted">A type that inherits <see cref="IScripted"/></typeparam>
    public static void AddScriptFactory<TScript, TScripted>(this IServiceCollection services) where TScript: IScript
        where TScripted: IScripted =>
        services.AddSingleton<IScriptFactory<TScript, TScripted>, ScriptFactory<TScript, TScripted>>();
}