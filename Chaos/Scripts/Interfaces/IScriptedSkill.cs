namespace Chaos.Scripts.Interfaces;

public interface IScriptedSkill : IScripted
{
    ISkillScript Script { get; }
}