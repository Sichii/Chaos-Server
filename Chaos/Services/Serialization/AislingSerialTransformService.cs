using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Objects.World;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Serialization.Interfaces;
using Chaos.Services.Utility.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Serialization;

public class AislingSerialTransformService : ISerialTransformService<Aisling, SerializableAisling>
{
    private readonly ISimpleCache<MapInstance> MapInstanceCache;
    private readonly ISerialTransformService<Item, SerializableItem> ItemTransformer;
    private readonly ISerialTransformService<Skill, SerializableSkill> SkillTransformer;
    private readonly ISerialTransformService<Spell, SerializableSpell> SpellTransformer;
    private readonly ICloningService<Item> ItemCloner;
    private readonly IExchangeFactory ExchangeFactory;
    private readonly ILoggerFactory LoggerFactory;
    private readonly ILogger<AislingSerialTransformService> Logger;

    public AislingSerialTransformService(
        ISimpleCache<MapInstance> mapInstanceCache,
        ISerialTransformService<Item, SerializableItem> itemTransformer,
        ISerialTransformService<Skill, SerializableSkill> skillTransformer,
        ISerialTransformService<Spell, SerializableSpell> spellTransformer,
        ICloningService<Item> itemCloner,
        ILoggerFactory loggerFactory,
        ILogger<AislingSerialTransformService> logger,
        IExchangeFactory exchangeFactory
    )
    {
        ItemTransformer = itemTransformer;
        SkillTransformer = skillTransformer;
        SpellTransformer = spellTransformer;
        Logger = logger;
        ExchangeFactory = exchangeFactory;
        MapInstanceCache = mapInstanceCache;
        ItemCloner = itemCloner;
        LoggerFactory = loggerFactory;
    }

    public Aisling Transform(SerializableAisling serialized) => new(
        serialized,
        MapInstanceCache,
        ItemCloner,
        ItemTransformer,
        SkillTransformer,
        SpellTransformer,
        ExchangeFactory,
        LoggerFactory.CreateLogger<Aisling>());

    public SerializableAisling Transform(Aisling entity) => new(entity);
}