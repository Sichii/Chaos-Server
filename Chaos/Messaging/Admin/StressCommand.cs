using Chaos.Collections.Common;
using Chaos.Extensions.Geometry;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("stress", helpText: "<grounditems|monsters> <amount>")]
public class StressCommand(IItemFactory itemFactory, IMonsterFactory monsterFactory) : ICommand<Aisling>
{
    private readonly IItemFactory ItemFactory = itemFactory;
    private readonly IMonsterFactory MonsterFactory = monsterFactory;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var type))
            return default;

        switch (type.ToLower())
        {
            case "grounditems":
            {
                if (!args.TryGetNext<int>(out var amount))
                    return default;

                var items = new List<GroundItem>();
                var map = source.MapInstance;

                for (var i = 0; i < amount; i++)
                {
                    var item = ItemFactory.Create("stick");
                    var point = map.Template.Bounds.GetRandomPoint(pt => !map.IsWall(pt));
                    items.Add(new GroundItem(item, map, point));
                }

                map.AddObjects(items);

                source.SendOrangeBarMessage($"{amount} stick(s) spawned on the ground");

                break;
            }
            case "monsters":
            {
                if (!args.TryGetNext<int>(out var amount))
                    return default;

                var monsters = new List<Monster>();
                var map = source.MapInstance;

                for (var i = 0; i < amount; i++)
                {
                    var point = map.GetRandomWalkablePoint();
                    var monster = MonsterFactory.Create("common_rat", map, point);
                    monster.AggroRange = 12;
                    monsters.Add(monster);
                }

                map.AddObjects(monsters);

                source.SendOrangeBarMessage($"{amount} rat(s) spawned");

                break;
            }
        }

        return default;
    }
}