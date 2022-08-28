using Chaos.Common.Definitions;
using Chaos.Containers.Abstractions;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Containers;

public class SpellBook : PanelBase<Spell>
{
    public SpellBook()
        : base(
            PanelType.SpellBook,
            90,
            new byte[] { 0, 36, 72 }) { }

    public SpellBook(IEnumerable<SpellSchema> spelSchemas, ITypeMapper mapper)
        : this()
    {
        foreach (var schema in spelSchemas)
        {
            var spell = mapper.Map<Spell>(schema);
            Objects[spell.Slot] = spell;
        }
    }
}