using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Abstractions;

public interface IScriptedMonster : IScripted
{
    IMonsterScript Script { get; }
}