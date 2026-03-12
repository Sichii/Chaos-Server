#region
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Pathfinding;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class PathOptionsExtensionsTests
{
    #region ForCreatureType
    [Test]
    public void ForCreatureType_WalkThrough_ShouldSetIgnoreWallsTrue()
    {
        var options = PathOptions.Default;

        var result = options.ForCreatureType(CreatureType.WalkThrough);

        result.IgnoreWalls
              .Should()
              .BeTrue();
    }

    [Test]
    public void ForCreatureType_Normal_ShouldSetIgnoreWallsFalse()
    {
        var options = PathOptions.Default;

        var result = options.ForCreatureType(CreatureType.Normal);

        result.IgnoreWalls
              .Should()
              .BeFalse();
    }
    #endregion
}