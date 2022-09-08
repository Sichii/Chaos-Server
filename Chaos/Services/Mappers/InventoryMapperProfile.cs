using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Services.Utility.Abstractions;

namespace Chaos.Services.Mappers;

public class InventoryMapperProfile : IMapperProfile<Inventory, InventorySchema>
{
    private readonly ITypeMapper Mapper;
    private readonly ICloningService<Item> ItemCloner;

    public InventoryMapperProfile(ITypeMapper mapper, ICloningService<Item> itemCloner)
    {
        Mapper = mapper;
        ItemCloner = itemCloner;
    }

    public Inventory Map(InventorySchema obj) => new(ItemCloner, Mapper.MapMany<Item>(obj));

    public InventorySchema Map(Inventory obj) => new(Mapper.MapMany<ItemSchema>(obj));
}