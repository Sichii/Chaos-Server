#region
using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Schemas.Content;
using Chaos.Schemas.Data;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public sealed class LootTableMapperProfile(IItemFactory itemFactory, ITypeMapper mapper)
    : IMapperProfile<LootTable, LootTableSchema>, IMapperProfile<LootDrop, LootDropSchema>
{
    private readonly IItemFactory ItemFactory = itemFactory;
    private readonly ITypeMapper Mapper = mapper;

    /// <inheritdoc />
    public LootDrop Map(LootDropSchema obj)
        => new()
        {
            ItemTemplateKey = obj.ItemTemplateKey,
            DropChance = obj.DropChance,
            ExtraScriptKeys = obj.ExtraScriptKeys
        };

    /// <inheritdoc />
    public LootDropSchema Map(LootDrop obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public LootTable Map(LootTableSchema obj)
        => new(ItemFactory)
        {
            Key = obj.Key,
            LootDrops = Mapper.MapMany<LootDrop>(obj.LootDrops)
                              .ToList(),
            Mode = obj.Mode
        };

    /// <inheritdoc />
    public LootTableSchema Map(LootTable obj) => throw new NotImplementedException();
}