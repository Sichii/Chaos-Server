#region
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Extensions;

public static class CreatureTypetensions
{
    public static bool WillCollideWith(this Creature creature, Creature other)
        =>

            //for the most specific creature v creature interactions
            creature.Type.WillCollideWith(other);

    extension(CreatureType type)
    {
        public bool WillCollideWith(CreatureType otherType)
            => type switch
            {
                CreatureType.Normal      => true,
                CreatureType.WalkThrough => otherType is not CreatureType.Aisling,
                CreatureType.Merchant    => true,
                CreatureType.WhiteSquare => true,
                CreatureType.Aisling     => otherType is not CreatureType.WalkThrough,
                _                        => throw new ArgumentOutOfRangeException()
            };

        public bool WillCollideWith(Creature creature)
        {
            if (creature is Aisling { IsAdmin: true, Visibility: VisibilityType.GmHidden })
                return false;

            return type.WillCollideWith(creature.Type);
        }
    }
}