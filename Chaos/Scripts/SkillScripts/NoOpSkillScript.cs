using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class NoOpSkillScript : SkillScriptBase
{
    public NoOpSkillScript(Skill skill)
        : base(skill) { }

    public override void OnUse(Aisling aisling) { }
}