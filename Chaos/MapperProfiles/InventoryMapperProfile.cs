using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class InventoryMapperProfile : IMapperProfile<Inventory, InventorySchema>
{
    private readonly ICloningService<Item> ItemCloner;
    private readonly ITypeMapper Mapper;

    public InventoryMapperProfile(ITypeMapper mapper, ICloningService<Item> itemCloner)
    {
        Mapper = mapper;
        ItemCloner = itemCloner;
    }

    public Inventory Map(InventorySchema obj) => new(ItemCloner, Mapper.MapMany<Item>(obj));

    public InventorySchema Map(Inventory obj) => new(Mapper.MapMany<ItemSchema>(obj));
}