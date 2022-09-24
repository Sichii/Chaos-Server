using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.SpellScripts.Abstractions;

public interface IScriptedSpell : IScripted
{
    ISpellScript Script { get; }
}