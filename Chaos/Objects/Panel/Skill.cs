using Chaos.Networking.Model.Server;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.Serializable;
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
        ulong? uniqueId = null
    )
        : base(template, uniqueId)
    {
        Template = template;

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = skillScriptFactory.CreateScript(ScriptKeys, this);
    }

    public Skill(
        SerializableSkill serializableSkill,
        ISimpleCache<SkillTemplate> skillTemplateCache,
        ISkillScriptFactory skillScriptFactory
    )
        : this(
            skillTemplateCache.GetObject(serializableSkill.TemplateKey),
            skillScriptFactory,
            serializableSkill.ScriptKeys,
            serializableSkill.UniqueId) =>
        Elapsed = TimeSpan.FromMilliseconds(serializableSkill.ElapsedMs);

    public SkillInfo ToSkillInfo() => new()
    {
        Name = Template.Name,
        Slot = Slot,
        Sprite = Template.PanelSprite
    };
}