using Chaos.Common.Definitions;
using Chaos.Containers.Abstractions;
using Chaos.Objects.Panel;

namespace Chaos.Containers;

public sealed class SkillBook : PanelBase<Skill>
{
    public SkillBook(IEnumerable<Skill>? skills = null)
        : base(
            PanelType.SkillBook,
            89,
            new byte[] { 0, 36, 72 })
    {
        skills ??= Array.Empty<Skill>();

        foreach (var skill in skills)
            Objects[skill.Slot] = skill;
    }
}