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
        if (!args.TryGetNext<string>(out var entityType))
            return default;

        if ("monster".ContainsI(entityType))
        {
            if (!args.TryGetNext<string>(out var monsterTemplateKey))
                return default;

            var monster = MonsterFactory.Create(monsterTemplateKey, source.MapInstance, source);

            if (args.TryGetNext<string>(out var lootTableKey))
            {
                var lootTable = SimpleCache.Get<LootTable>(lootTableKey);
                monster.Items.AddRange(lootTable.GenerateLoot());
            }

            if (args.TryGetNext<int>(out var expAmount))
                monster.Experience = expAmount;

            if (args.TryGetNext<int>(out var goldAmount))
                monster.Gold = goldAmount;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (args.TryGetNext<int>(out var aggroRange))
                monster.AggroRange = aggroRange;
            else
                monster.AggroRange = 0;

            source.MapInstance.AddObject(monster, source);
        } else if ("merchant".ContainsI(entityType))
        {
            if (!args.TryGetNext<string>(out var merchantTemplateKey))
                return default;

            var merchant = MerchantFactory.Create(merchantTemplateKey, source.MapInstance, source);

            if (args.TryGetNext<Direction>(out var direction))
                merchant.Direction = direction;

            source.MapInstance.AddObject(merchant, source);
        }

        return default;
    }
}