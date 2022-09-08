using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class DamageScript : SkillScriptBase
{
    public DamageScript(Skill skill)
        : base(skill) { }

    public override void OnUse(Creature source)
    {
        //TODO: damage calculations n shit
    }
}