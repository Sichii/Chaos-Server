using Chaos.Objects.Panel;

namespace Chaos.Factories.Interfaces;

public interface ISkillFactory
{
    Skill CreateSkill(string templateKey, ICollection<string>? extraScriptKeys = null);
}