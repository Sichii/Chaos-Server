using Chaos.Containers;
using Chaos.Data;
using Chaos.Entities.Schemas.Content;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Mappers.Abstractions;

namespace Chaos.Services.Mappers;

public class LootTableMapperProfile : IMapperProfile<LootTable, LootTableSchema>
{
    private readonly IItemFactory ItemFactory;
    private readonly ITypeMapper Mapper;

    public LootTableMapperProfile(IItemFactory itemFactory, ITypeMapper mapper)
    {
        ItemFactory = itemFactory;
        Mapper = mapper;
    }

    /// <inheritdoc />
    public LootTable Map(LootTableSchema obj) => new()
    {
        ItemFactory = ItemFactory,
        Key = obj.Key,
        LootDrops = Mapper.MapMany<LootDrop>(obj.LootDrops).ToList()
    };

    /// <inheritdoc />
    public LootTableSchema Map(LootTable obj) => throw new NotImplementedException();
}