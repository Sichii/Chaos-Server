using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Models.Panel;

namespace Chaos.Collections;

public sealed class SkillBook : KnowledgeBookBase<Skill>
{
    public SkillBook(IEnumerable<Skill>? skills = null)
        : base(PanelType.SkillBook, skills) { }
}