using Chaos.Containers.Abstractions;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Serialization.Interfaces;

namespace Chaos.Containers;

public class SkillBook : PanelBase<Skill>
{
    public SkillBook()
        : base(
            PanelType.SkillBook,
            89,
            new byte[] { 0, 36, 72 }) { }

    public SkillBook(IEnumerable<SerializableSkill> serializedSkills, ISerialTransformService<Skill, SerializableSkill> skillTransformer)
        : this()
    {
        foreach (var serialized in serializedSkills)
        {
            var skill = skillTransformer.Transform(serialized);
            Objects[skill.Slot] = skill;
        }
    }
}