#region
using Chaos.Time;
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public sealed class ChantTimerTests
{
    [Test]
    public void Start_ResetsElapsed_AllowingNewChant()
    {
        var timer = new ChantTimer(1000);

        timer.Start(1);
        timer.Update(TimeSpan.FromMilliseconds(500));

        // Reset with a new chant — should zero elapsed
        timer.Start(1);

        // Elapsed is now 0, burden is 0, so this should succeed
        var result = timer.Validate(1);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Update_AdvancesElapsed_AffectingTimeBurden()
    {
        // Generous max burden so burden doesn't block
        var timer = new ChantTimer(5000);

        timer.Start(2); // expects 2000ms

        timer.Update(TimeSpan.FromMilliseconds(1000));
        timer.Update(TimeSpan.FromMilliseconds(1000)); // total 2000ms = expectedChantTime

        // Elapsed == expectedChantTime → burden += 0
        var result = timer.Validate(2);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Validate_ReturnsFalse_WhenCastLinesGreaterThanExpected()
    {
        var timer = new ChantTimer(1000);
        timer.Start(1);
        timer.Update(TimeSpan.FromMilliseconds(1000));

        // castLines(2) > ExpectedCastLines(1) → false
        var result = timer.Validate(2);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Validate_ReturnsFalse_WhenTimeBurdenExceedsMax()
    {
        // MaxTimeBurden = 500ms
        var timer = new ChantTimer(500);

        // First spell: cast instantly (elapsed=0) → burden = ClampPositive(1000-0) = 1000ms
        timer.Start(1);
        var firstResult = timer.Validate(1);

        firstResult.Should()
                   .BeTrue();

        // Second spell: burden(1000ms) > maxBurden(500ms) → fails
        timer.Start(1);
        timer.Update(TimeSpan.FromMilliseconds(1000));

        var secondResult = timer.Validate(1);

        secondResult.Should()
                    .BeFalse();
    }

    [Test]
    public void Validate_ReturnsFalse_WhenZeroCastLines()
    {
        var timer = new ChantTimer(1000);
        timer.Start(1);
        timer.Update(TimeSpan.FromMilliseconds(1000));

        var result = timer.Validate(0);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void Validate_ReturnsTrue_WhenExactTimeElapsed()
    {
        var timer = new ChantTimer(1000);

        timer.Start(1); // 1 cast line → 1000ms expected

        timer.Update(TimeSpan.FromMilliseconds(1000));

        var result = timer.Validate(1);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void Validate_SecondCallFails_AfterExpectedCastLinesReset()
    {
        var timer = new ChantTimer(1000);
        timer.Start(1);
        timer.Update(TimeSpan.FromMilliseconds(1000));

        // First validate succeeds and resets ExpectedCastLines to 0
        var first = timer.Validate(1);

        first.Should()
             .BeTrue();

        // Second validate: castLines(1) > ExpectedCastLines(0) → false
        var second = timer.Validate(1);

        second.Should()
              .BeFalse();
    }
}