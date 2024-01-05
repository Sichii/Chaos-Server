using Chaos.Scripting.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Scripting;

/// <summary>
///     Provides any kind of <see cref="Chaos.Scripting.Abstractions.IScript" /> implementation, as long as it has a
///     <see cref="Chaos.Scripting.Abstractions.IScriptFactory{TScript,TScripted}" /> implemented and added to the service
///     provider
/// </summary>
/// <remarks>
///     This provider merely looks up the <see cref="Chaos.Scripting.Abstractions.IScriptFactory{TScript,TScripted}" />
///     used to generate a given <see cref="Chaos.Scripting.Abstractions.IScript" /> type, and then requests that script
///     from the factory.
/// </remarks>
public sealed class ScriptProvider : IScriptProvider
{
    private readonly IServiceProvider ServiceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ScriptProvider" /> class.
    /// </summary>
    /// <param name="serviceProvider">
    ///     The application si container
    /// </param>
    public ScriptProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    /// <inheritdoc />
    public TScript CreateScript<TScript, TScripted>(ICollection<string> scriptKeys, TScripted subject) where TScript: IScript
        where TScripted: IScripted
    {
        var factory = ServiceProvider.GetRequiredService<IScriptFactory<TScript, TScripted>>();

        return factory.CreateScript(scriptKeys, subject);
    }
}