namespace Chaos.Scripts.Abstractions;

public interface IScriptedSkill : IScripted
{
    ISkillScript Script { get; }
}