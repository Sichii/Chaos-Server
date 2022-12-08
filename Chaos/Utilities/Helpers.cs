using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Utilities;

public static class Helpers
{
    public static EntityType? GetEntityType(object obj) =>
        obj switch
        {
            Monster or Merchant => EntityType.Creature,
            GroundItem or Item  => EntityType.Item,
            Aisling             => EntityType.Aisling,
            _                   => null
        };

    public static void HandleApproach(Creature creature1, Creature creature2)
    {
        if (creature1.Equals(creature2))
            return;

        if (creature1.IsVisibleTo(creature2))
            creature2.OnApproached(creature1);

        if (creature2.IsVisibleTo(creature1))
            creature1.OnApproached(creature2);
    }

    public static void HandleDeparture(Creature creature1, Creature creature2)
    {
        if (creature1.Equals(creature2))
            return;

        if (creature1.IsVisibleTo(creature2))
            creature2.OnDeparture(creature1);

        if (creature2.IsVisibleTo(creature1))
            creature1.OnDeparture(creature2);
    }
}