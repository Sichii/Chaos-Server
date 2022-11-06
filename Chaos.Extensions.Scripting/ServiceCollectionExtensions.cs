using Chaos.Scripting;
using Chaos.Scripting.Abstractions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddScriptFactory<TScript, TSource>(this IServiceCollection services) where TScript: IScript
                                                                                            where TSource: IScripted =>
        services.AddSingleton<IScriptFactory<TScript, TSource>, ScriptFactory<TScript, TSource>>();
}