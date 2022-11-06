using Chaos.Objects.Panel;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class NoOpScript : SkillScriptBase
{
    public NoOpScript(Skill subject)
        : base(subject) { }
}