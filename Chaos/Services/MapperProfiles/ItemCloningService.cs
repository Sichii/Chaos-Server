using Chaos.Common.Identity;
using Chaos.Models.Panel;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.MapperProfiles;

public sealed class ItemCloningService : ICloningService<Item>
{
    private readonly ILogger<ItemCloningService> Logger;
    private readonly ITypeMapper Mapper;

    public ItemCloningService(ITypeMapper mapper, ILogger<ItemCloningService> logger)
    {
        Logger = logger;
        Mapper = mapper;
    }

    public Item Clone(Item obj)
    {
        var schema = Mapper.Map<ItemSchema>(obj);
        schema.UniqueId = PersistentIdGenerator<ulong>.Shared.NextId;
        var cloned = Mapper.Map<Item>(schema);

        Logger.LogDebug("Cloned {@OriginItem} into {@ClonedItem}", obj, cloned);

        return cloned;
    }
}