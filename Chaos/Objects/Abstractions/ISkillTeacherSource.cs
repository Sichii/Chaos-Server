using Chaos.Objects.Panel;

namespace Chaos.Objects.Abstractions;

public interface ISkillTeacherSource : IDialogSourceEntity
{
    ICollection<Skill> SkillsToTeach { get; }
    bool TryGetSkill(string skillName, [MaybeNullWhen(false)] out Skill skill);
}