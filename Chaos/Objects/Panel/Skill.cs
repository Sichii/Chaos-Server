using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

/// <summary>
///     Represents an object that exists within the skill panel.
/// </summary>
public class Skill : PanelObjectBase, IScriptedSkill
{
    public ISkillScript Script { get; }
    public override SkillTemplate Template { get; }

    public Skill(
        SkillTemplate template,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null,
        ulong? uniqueId = null,
        int? elapsedMs = null
    )
        : base(template, uniqueId, elapsedMs)
    {
        Template = template;

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = scriptProvider.CreateScript<ISkillScript, Skill>(ScriptKeys, this);
    }

    public void Use(Creature source) => Script.OnUse(source);
}