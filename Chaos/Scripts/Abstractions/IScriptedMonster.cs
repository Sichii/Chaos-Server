namespace Chaos.Scripts.Abstractions;

public interface IScriptedMonster : IScripted
{
    IMonsterScript Script { get; }
}