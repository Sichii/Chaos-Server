using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Commands;

[Command("spawn")]
public class SpawnCommand : ICommand<Aisling>
{
    private readonly IMonsterFactory MonsterFactory;
    private readonly ISimpleCache SimpleCache;
    public SpawnCommand(IMonsterFactory monsterFactory, ISimpleCache simpleCache)
    {
        MonsterFactory = monsterFactory;
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public void Execute(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGet<string>(0, out var monsterTemplateKey))
            return;
        
        var monster = MonsterFactory.Create(monsterTemplateKey, source.MapInstance, source);

        if (args.TryGet<string>(1, out var lootTableKey))
        {
            var lootTable = SimpleCache.Get<LootTable>(lootTableKey);
            monster.Items.AddRange(lootTable.GenerateLoot());
        }

        if (args.TryGet<int>(2, out var expAmount))
            monster.Experience = expAmount;

        if (args.TryGet<int>(3, out var goldAmount))
            monster.Gold = goldAmount;

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (args.TryGet<int>(4, out var aggroRange))
            monster.AggroRange = aggroRange;
        else
            monster.AggroRange = 0;
        
        source.MapInstance.AddObject(monster, source);
    }
}