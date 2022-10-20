using Chaos.Data;
using Chaos.Schemas.Content;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class LootDropMapper : IMapperProfile<LootDrop, LootDropSchema>
{
    /// <inheritdoc />
    public LootDrop Map(LootDropSchema obj) => new()
    {
        ItemTemplateKey = obj.ItemTemplateKey,
        DropChance = obj.DropChance
    };

    /// <inheritdoc />
    public LootDropSchema Map(LootDrop obj) => throw new NotImplementedException();
}