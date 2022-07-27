using Chaos.Scripts.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Providers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Chaos.Services.Providers;

public class ScriptFactoryProvider : IScriptFactoryProvider
{
    private readonly IServiceProvider ServiceProvider;

    public ScriptFactoryProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public IScriptFactory<TScript, TScripted> GetScriptFactory<TScript, TScripted>() where TScript: IScript
                                                                                     where TScripted: IScripted =>
        ServiceProvider.GetRequiredService<IScriptFactory<TScript, TScripted>>();
}