using Chaos.Data;
using Chaos.Entities.Schemas.Content;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class LootDropMapper : IMapperProfile<LootDrop, LootDropSchema>
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