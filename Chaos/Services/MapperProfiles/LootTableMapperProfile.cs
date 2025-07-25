#region
using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Schemas.Content;
using Chaos.Schemas.Data;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public sealed class LootTableMapperProfile(
    IItemFactory itemFactory,
    ITypeMapper mapper,
    ISimpleCache cache,
    ICloningService<Item> itemClonser) : IMapperProfile<LootTable, LootTableSchema>, IMapperProfile<LootDrop, LootDropSchema>
{
    private readonly ISimpleCache Cache = cache;
    private readonly ICloningService<Item> ItemClonser = itemClonser;
    private readonly IItemFactory ItemFactory = itemFactory;
    private readonly ITypeMapper Mapper = mapper;

    /// <inheritdoc />
    public LootDrop Map(LootDropSchema obj)
        => new()
        {
            ItemTemplateKey = obj.ItemTemplateKey,
            DropChance = obj.DropChance,
            ExtraScriptKeys = obj.ExtraScriptKeys,
            MinAmount = obj.MinAmount ?? 1,
            MaxAmount = obj.MaxAmount ?? 1
        };

    /// <inheritdoc />
    public LootDropSchema Map(LootDrop obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public LootTable Map(LootTableSchema obj)
        => new(ItemFactory, Cache, ItemClonser)
        {
            Key = obj.Key,
            LootDrops = Mapper.MapMany<LootDrop>(obj.LootDrops)
                              .ToList(),
            Mode = obj.Mode
        };

    /// <inheritdoc />
    public LootTableSchema Map(LootTable obj) => throw new NotImplementedException();
}