using Chaos.Scripts.Interfaces;

namespace Chaos.Services.Factories.Interfaces;

public interface IScriptFactoryProvider
{
    IScriptFactory<TScript, TScripted> GetScriptFactory<TScript, TScripted>() where TScript: IScript
                                                                              where TScripted: IScripted;
}