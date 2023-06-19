using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("spawnMonster", helpText: "<templateKey> <lootTableKey?null> <expAmount?0> <goldAmount?0> <aggroRange?0>")]
public class SpawnMonsterCommand : ICommand<Aisling>
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;

    public SpawnMonsterCommand(IMonsterFactory monsterFactory, ISimpleCache simpleCache)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
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

        source.MapInstance.AddObject(monster, source);

        return default;
    }
}