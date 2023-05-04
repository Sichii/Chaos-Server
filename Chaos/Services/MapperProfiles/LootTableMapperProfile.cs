using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Schemas.Content;
using Chaos.Schemas.Data;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class LootTableMapperProfile : IMapperProfile<LootTable, LootTableSchema>,
                                             IMapperProfile<LootDrop, LootDropSchema>
{
    private readonly IItemFactory ItemFactory;
    private readonly ITypeMapper Mapper;

    public LootTableMapperProfile(IItemFactory itemFactory, ITypeMapper mapper)
    {
        ItemFactory = itemFactory;
        Mapper = mapper;
    }

    /// <inheritdoc />
    public LootTable Map(LootTableSchema obj) => new(ItemFactory)
    {
        Key = obj.Key,
        LootDrops = Mapper.MapMany<LootDrop>(obj.LootDrops).ToList()
    };

    /// <inheritdoc />
    public LootTableSchema Map(LootTable obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public LootDrop Map(LootDropSchema obj) => new()
    {
        ItemTemplateKey = obj.ItemTemplateKey,
        DropChance = obj.DropChance
    };

    /// <inheritdoc />
    public LootDropSchema Map(LootDrop obj) => throw new NotImplementedException();
}