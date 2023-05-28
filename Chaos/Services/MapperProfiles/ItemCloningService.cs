using Chaos.Common.Identity;
using Chaos.Models.Panel;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class ItemCloningService : ICloningService<Item>
{
    private readonly ITypeMapper Mapper;

    public ItemCloningService(ITypeMapper mapper) => Mapper = mapper;

    public Item Clone(Item obj)
    {
        var schema = Mapper.Map<ItemSchema>(obj);
        schema.UniqueId = PersistentIdGenerator<ulong>.Shared.NextId;
        var cloned = Mapper.Map<Item>(schema);

        return cloned;
    }
}