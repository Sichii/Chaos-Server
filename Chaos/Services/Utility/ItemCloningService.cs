using Chaos.Core.Identity;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Utility;

public class ItemCloningService : ICloningService<Item>
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

        Logger.LogDebug(
            "Cloned item - Name: {ItemName} FromUniqueId: {UniqueId}, ToUniqueId: {ClonedId}",
            obj.DisplayName,
            obj.UniqueId,
            cloned.UniqueId);

        return cloned;
    }
}