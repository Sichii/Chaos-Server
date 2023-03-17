using Chaos.Collections.Common;
using Chaos.Extensions.Geometry;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Commands.Admin;

[Command("stressTest")]
public class StressTestCommand : ICommand<Aisling>
{
    private readonly IItemFactory ItemFactory;

    public StressTestCommand(IItemFactory itemFactory) => ItemFactory = itemFactory;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var type))
            return default;

        switch (type.ToLower())
        {
            case "grounditems":
                if (!args.TryGetNext<int>(out var amount))
                    return default;

                var items = new List<GroundItem>();
                var map = source.MapInstance;

                for (var i = 0; i < amount; i++)
                {
                    var item = ItemFactory.Create("stick");
                    var point = map.Template.Bounds.RandomPoint();
                    items.Add(new GroundItem(item, map, point));
                }

                map.AddObjects(items);

                source.SendOrangeBarMessage($"{amount} stick(s) spawned on the ground");

                break;
        }

        return default;
    }
}