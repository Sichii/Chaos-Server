using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Models.Panel;

namespace Chaos.Collections;

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