using Chaos.Scripts.Abstractions;

namespace Chaos.Services.Scripting.Abstractions;

public interface IScriptFactory<out TScript, in TSource> where TScript: IScript
                                                         where TSource: IScripted
{
    TScript CreateScript(ICollection<string> scriptKeys, TSource source);
}