using Chaos.Common.Definitions;
using Chaos.Containers.Abstractions;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Containers;

public class SkillBook : PanelBase<Skill>
{
    public SkillBook()
        : base(
            PanelType.SkillBook,
            89,
            new byte[] { 0, 36, 72 }) { }

    public SkillBook(IEnumerable<SkillSchema> skillSchemas, ITypeMapper mapper)
        : this()
    {
        foreach (var schema in skillSchemas)
        {
            var skill = mapper.Map<Skill>(schema);
            Objects[skill.Slot] = skill;
        }
    }
}