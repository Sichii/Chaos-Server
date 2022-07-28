using Chaos.Objects.Panel;
using Chaos.Scripts.Interfaces;

namespace Chaos.Services.Factories.Interfaces;

public interface IScriptFactory<out TScript, in TSource> where TScript: IScript
                                                         where TSource: IScripted
{
    TScript CreateScript(ICollection<string> scriptKeys, TSource source);
}

public interface ISpellScriptFactory : IScriptFactory<ISpellScript, Spell> { }

public interface ISkillScriptFactory : IScriptFactory<ISkillScript, Skill> { }

public interface IItemScriptFactory : IScriptFactory<IItemScript, Item> { }