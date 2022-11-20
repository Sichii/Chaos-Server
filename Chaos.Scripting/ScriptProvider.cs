using Chaos.Scripting.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Scripting;

public sealed class ScriptProvider : IScriptProvider
{
    private readonly IServiceProvider ServiceProvider;

    public ScriptProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public TScript CreateScript<TScript, TScripted>(ICollection<string> scriptKeys, TScripted subject) where TScript: IScript
        where TScripted: IScripted
    {
        var factory = ServiceProvider.GetRequiredService<IScriptFactory<TScript, TScripted>>();

        return factory.CreateScript(scriptKeys, subject);
    }
}