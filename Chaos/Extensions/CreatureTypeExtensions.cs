using Chaos.Common.Definitions;

namespace Chaos.Extensions;

public static class CreatureTypeExtensions
{
    public static bool WillCollideWith(this CreatureType type, CreatureType otherType) => type switch
    {
        CreatureType.Normal      => true,
        CreatureType.WalkThrough => otherType is not CreatureType.Aisling,
        CreatureType.Merchant    => true,
        CreatureType.WhiteSquare => true,
        CreatureType.Aisling     => otherType is not CreatureType.WalkThrough,
        _                        => throw new ArgumentOutOfRangeException()
    };
}