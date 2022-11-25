using Chaos.Scripting.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Scripting;

/// <summary>
///     Provides any kind of <see cref="IScript" /> implementation, as long as it has a <see cref="IScriptFactory{TScript,TScripted}" />
///     implemented and added to the service provider
/// </summary>
/// <remarks>
///     This provider merely looks up the <see cref="IScriptFactory{TScript,TScripted}" /> used to generate a given <see cref="IScript" />
///     type, and then requests that script from the factory.
/// </remarks>
public sealed class ScriptProvider : IScriptProvider
{
    private readonly IServiceProvider ServiceProvider;

    public ScriptProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    /// <inheritdoc />
    public TScript CreateScript<TScript, TScripted>(ICollection<string> scriptKeys, TScripted subject) where TScript: IScript
        where TScripted: IScripted
    {
        var factory = ServiceProvider.GetRequiredService<IScriptFactory<TScript, TScripted>>();

        return factory.CreateScript(scriptKeys, subject);
    }
}