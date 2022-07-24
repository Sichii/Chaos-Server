using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;

namespace Chaos.Factories.Interfaces;

public interface ISkillFactory
{
    Skill CreateSkill(string templateKey, ICollection<string>? extraScriptKeys = null);
    Skill DeserializeSkill(SerializableSkill serializableSkill);
    Skill CloneSkill(Skill skill);
}