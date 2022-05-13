namespace Chaos.Scripts.Interfaces;

public interface IScriptedSpell : IScripted
{
    ISpellScript Script { get; }
}