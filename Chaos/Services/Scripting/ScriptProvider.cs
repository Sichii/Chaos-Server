using Chaos.Scripts.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Services.Scripting;

public class ScriptProvider : IScriptProvider
{
    private readonly IServiceProvider ServiceProvider;

    public ScriptProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public TScript CreateScript<TScript, TScripted>(ICollection<string> scriptKeys, TScripted source) where TScript: IScript
        where TScripted: IScripted
    {
        var factory = ServiceProvider.GetRequiredService<IScriptFactory<TScript, TScripted>>();

        return factory.CreateScript(scriptKeys, source);
    }
}