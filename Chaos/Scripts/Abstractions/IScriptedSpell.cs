namespace Chaos.Scripts.Abstractions;

public interface IScriptedSpell : IScripted
{
    ISpellScript Script { get; }
}