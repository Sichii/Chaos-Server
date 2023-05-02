using Chaos.Objects.World.Abstractions;

namespace Chaos.Utilities;

public static class Helpers
{
    public static void HandleApproach(Creature creature1, Creature creature2)
    {
        if (creature1.Equals(creature2))
            return;

        if (creature2.CanObserve(creature1))
            creature2.OnApproached(creature1);

        if (creature1.CanObserve(creature2))
            creature1.OnApproached(creature2);
    }

    public static void HandleDeparture(Creature creature1, Creature creature2)
    {
        if (creature1.Equals(creature2))
            return;

        if (creature2.CanObserve(creature1))
            creature2.OnDeparture(creature1);

        if (creature1.CanObserve(creature2))
            creature1.OnDeparture(creature2);
    }
}