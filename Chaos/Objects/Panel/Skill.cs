using Chaos.Extensions.Common;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

/// <summary>
///     Represents an object that exists within the skill panel.
/// </summary>
public sealed class Skill : PanelObjectBase, IScriptedSkill
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

    public void Use(Creature source)
    {
        Script.OnUse(source);

        if (Template.IsAssail)
        {
            var assailInterval = source.StatSheet.CalculateEffectiveAssailInterval(source.AssailIntervalMs);
            Cooldown = TimeSpan.FromMilliseconds(assailInterval);
        } else if (!Cooldown.HasValue)
            return;

        Elapsed = TimeSpan.Zero;

        if (source is Aisling aisling && !Template.IsAssail)
            aisling.Client.SendCooldown(this);
    }
}