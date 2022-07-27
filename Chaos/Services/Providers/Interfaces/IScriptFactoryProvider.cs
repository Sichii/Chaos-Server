using Chaos.Scripts.Interfaces;
using Chaos.Services.Factories.Interfaces;

namespace Chaos.Services.Providers.Interfaces;

public interface IScriptFactoryProvider
{
    IScriptFactory<TScript, TScripted> GetScriptFactory<TScript, TScripted>() where TScript: IScript
                                                                              where TScripted: IScripted;
}