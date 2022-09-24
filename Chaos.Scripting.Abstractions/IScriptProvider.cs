namespace Chaos.Scripting.Abstractions;

public interface IScriptProvider
{
    TScript CreateScript<TScript, TScripted>(ICollection<string> scriptKeys, TScripted source) where TScript: IScript
        where TScripted: IScripted;
}