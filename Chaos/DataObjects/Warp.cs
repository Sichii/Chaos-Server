using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.WorldObjects;

namespace Chaos.DataObjects;

public record Warp
{
    public Location? SourceLocation { get; init; }
    public Location TargetLocation { get; init; }

    /// <summary>
    ///     Creates a warp that will send a user to purgatory, and returns it.
    /// </summary>
    public static Warp Death(User user) => new()
    {
        TargetLocation = CONSTANTS.DEATH_LOCATION,
        SourceLocation = user.Location
    };

    /*
    public static Warp Home(User user)
    {
        var home = user.Nation switch
        {
            Nation.None     => CONSTANTS.NO_NATION_LOCATION,
            Nation.Suomi    => CONSTANTS.SUOMI_LOCATION,
            Nation.Loures   => CONSTANTS.LOURES_LOCATION,
            Nation.Mileth   => CONSTANTS.MILETH_LOCATION,
            Nation.Tagor    => CONSTANTS.TAGOR_LOCATION,
            Nation.Rucesion => CONSTANTS.RUCESION_LOCATION,
            Nation.Noes     => CONSTANTS.NOES_LOCATION,
            _               => throw new InvalidOperationException()
        };

        return new Warp
        {
            TargetLocation = home,
            SourceLocation = user.Location
        };
    }*/

    public override string ToString() => $@"{SourceLocation.ToString()} => {TargetLocation.ToString()}";

    public static Warp Unsourced(Location targetLocation) => new()
    {
        TargetLocation = targetLocation,
        SourceLocation = null
    };
}