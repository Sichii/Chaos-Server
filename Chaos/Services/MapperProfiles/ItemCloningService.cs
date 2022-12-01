using Chaos.Common.Identity;
using Chaos.Objects.Panel;
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
        schema.UniqueId = ServerId.NextId;
        var cloned = Mapper.Map<Item>(schema);

        Logger.LogDebug("Cloned item {Item} with new Uid: {NewUniqueId}", obj, cloned.UniqueId);

        return cloned;
    }
}