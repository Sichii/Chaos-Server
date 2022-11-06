using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Containers;
using Chaos.Extensions.Common;
using Chaos.Factories.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects.World;
using Chaos.Storage.Abstractions;

namespace Chaos.Commands;

[Command("spawn")]
public class SpawnCommand : ICommand<Aisling>
{
    private readonly IMerchantFactory MerchantFactory;
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;

    public SpawnCommand(IMonsterFactory monsterFactory, ISimpleCache simpleCache, IMerchantFactory merchantFactory)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
        MerchantFactory = merchantFactory;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGet<string>(0, out var entityType))
            return default;

        if ("monster".ContainsI(entityType))
        {
            if (!args.TryGet<string>(1, out var monsterTemplateKey))
                return default;

            var monster = MonsterFactory.Create(monsterTemplateKey, source.MapInstance, source);

            if (args.TryGet<string>(2, out var lootTableKey))
            {
                var lootTable = SimpleCache.Get<LootTable>(lootTableKey);
                monster.Items.AddRange(lootTable.GenerateLoot());
            }

            if (args.TryGet<int>(3, out var expAmount))
                monster.Experience = expAmount;

            if (args.TryGet<int>(4, out var goldAmount))
                monster.Gold = goldAmount;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (args.TryGet<int>(5, out var aggroRange))
                monster.AggroRange = aggroRange;
            else
                monster.AggroRange = 0;

            source.MapInstance.AddObject(monster, source);
        } else if ("merchant".ContainsI(entityType))
        {
            if (!args.TryGet<string>(1, out var merchantTemplateKey))
                return default;

            var merchant = MerchantFactory.Create(merchantTemplateKey, source.MapInstance, source);

            if (args.TryGet<Direction>(2, out var direction))
                merchant.Direction = direction;

            source.MapInstance.AddObject(merchant, source);
        }

        return default;
    }
}