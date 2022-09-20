using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public interface IScriptedSkill : IScripted
{
    ISkillScript Script { get; }
}