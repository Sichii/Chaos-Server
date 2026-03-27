#region
using FluentAssertions;
#endregion

namespace Chaos.Pathfinding.Tests;

public sealed class PathOptionsTests
{
    [Test]
    public void Default_Has_LimitRadius_And_Empty_Collections()
    {
        var def = PathOptions.Default;

        def.LimitRadius
           .Should()
           .Be(12);

        def.BlockedPoints
           .Should()
           .BeEmpty();

        def.IgnoreWalls
           .Should()
           .BeFalse();

        def.IgnoreBlockingReactors
           .Should()
           .BeFalse();
    }
}