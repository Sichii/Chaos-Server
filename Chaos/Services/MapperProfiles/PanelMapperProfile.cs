using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class PanelMapperProfile : IMapperProfile<Inventory, InventorySchema>,
                                         IMapperProfile<SkillBook, SkillBookSchema>,
                                         IMapperProfile<SpellBook, SpellBookSchema>
{
    private readonly ICloningService<Item> ItemCloner;
    private readonly ITypeMapper Mapper;

    public PanelMapperProfile(ITypeMapper mapper, ICloningService<Item> itemCloner)
    {
        Mapper = mapper;
        ItemCloner = itemCloner;
    }

    public Inventory Map(InventorySchema obj) => new(ItemCloner, Mapper.MapMany<Item>(obj));

    public InventorySchema Map(Inventory obj) => new(Mapper.MapMany<ItemSchema>(obj));

    public SkillBook Map(SkillBookSchema obj) => new(Mapper.MapMany<Skill>(obj));

    public SkillBookSchema Map(SkillBook obj) => new(Mapper.MapMany<SkillSchema>(obj));

    public SpellBook Map(SpellBookSchema obj) => new(Mapper.MapMany<Spell>(obj));

    public SpellBookSchema Map(SpellBook obj) => new(Mapper.MapMany<SpellSchema>(obj));
}