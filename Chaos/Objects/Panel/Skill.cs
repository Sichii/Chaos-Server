using Chaos.Entities.Networking.Server;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Scripts.Interfaces;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
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
        ISkillScriptFactory skillScriptFactory,
        ICollection<string>? extraScriptKeys = null,
        ulong? uniqueId = null,
        int? elapsedMs = null
    )
        : base(template, uniqueId, elapsedMs)
    {
        Template = template;

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = skillScriptFactory.CreateScript(ScriptKeys, this);
    }

    public Skill(
        SkillSchema schema,
        ISimpleCache simpleCache,
        ISkillScriptFactory skillScriptFactory
    )
        : this(
            simpleCache.GetObject<SkillTemplate>(schema.TemplateKey),
            skillScriptFactory,
            schema.ScriptKeys,
            schema.UniqueId,
            schema.ElapsedMs) { }
}