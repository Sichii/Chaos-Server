using Chaos.Models.Panel;

namespace Chaos.Models.Abstractions;

public interface ISkillTeacherSource : IDialogSourceEntity
{
    ICollection<Skill> SkillsToTeach { get; }
    bool TryGetSkill(string skillName, [MaybeNullWhen(false)] out Skill skill);
}