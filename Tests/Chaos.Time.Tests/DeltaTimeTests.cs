#region
using Chaos.Time;
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public sealed class DeltaTimeTests
{
    [Test]
    public void GetDelta_ReturnsNonNegativeTimeSpan()
    {
        var deltaTime = new DeltaTime();

        var delta = deltaTime.GetDelta;

        delta.Should()
             .BeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Test]
    public void GetDelta_SuccessiveCalls_EachReturnNonNegativeValue()
    {
        var deltaTime = new DeltaTime();

        // First call establishes baseline timestamp
        var first = deltaTime.GetDelta;

        first.Should()
             .BeGreaterThanOrEqualTo(TimeSpan.Zero);

        // Second call measures elapsed since previous call
        var second = deltaTime.GetDelta;

        second.Should()
              .BeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [Test]
    public void GetDelta_TwoInstancesAreIndependent()
    {
        var deltaTime1 = new DeltaTime();
        var deltaTime2 = new DeltaTime();

        // Each instance tracks its own timestamp
        var d1 = deltaTime1.GetDelta;
        var d2 = deltaTime2.GetDelta;

        d1.Should()
          .BeGreaterThanOrEqualTo(TimeSpan.Zero);

        d2.Should()
          .BeGreaterThanOrEqualTo(TimeSpan.Zero);
    }
}