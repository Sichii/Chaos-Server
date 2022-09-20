using Chaos.Objects.Panel;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class NoOpSkillScript : SkillScriptBase
{
    public NoOpSkillScript(Skill subject)
        : base(subject) { }
}