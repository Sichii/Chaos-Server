using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class NoOpItemScript : SkillScriptBase
{
    public NoOpItemScript(Skill skill)
        : base(skill) { }

    public override void OnUse(Aisling aisling) { }
}