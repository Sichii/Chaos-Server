#region
using FluentAssertions;
#endregion

namespace Chaos.Time.Tests;

public class ResettingCounterTests
{
    [Test]
    public void Reset_Should_Clear_Counter()
    {
           var counter = new ResettingCounter(1);

        counter.TryIncrement()
               .Should()
               .BeTrue();
        counter.Reset();

        counter.Counter
               .Should()
               .Be(0);

        counter.CanIncrement
               .Should()
               .BeTrue();
    }

    [Test]
       public void SetMaxCount_Should_Set_MaxCount_Directly()
    {
              var counter = new ResettingCounter(2, 3); // MaxCount = ceil(2*3) = 6

              counter.TryIncrement()
                     .Should()
                     .BeTrue();

              counter.TryIncrement()
                     .Should()
                     .BeTrue();

              counter.SetMaxCount(2); // Directly sets MaxCount = 2

              // We already used 2 increments; MaxCount is now 2, so we cannot increment
              counter.TryIncrement()
                     .Should()
                     .BeFalse();
       }

       [Test]
       public void SetMaxPerSecond_Should_Multiply_By_UpdateInterval()
       {
              var counter = new ResettingCounter(2, 3); // MaxCount = ceil(2*3) = 6

        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.TryIncrement()
               .Should()
               .BeTrue();

              counter.SetMaxPerSecond(1); // ceil(1 * 3) = 3

        // We already used 2 increments; ensure we cannot exceed 3
        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.TryIncrement()
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryIncrement_Should_Return_False_When_At_Max()
    {
           var counter = new ResettingCounter(1);

        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.TryIncrement()
               .Should()
               .BeFalse();

        counter.CanIncrement
               .Should()
               .BeFalse();
    }

    [Test]
    public void Update_Should_Reset_Counter_When_Timer_Elapsed()
    {
           var counter = new ResettingCounter(3);

           // Consume initial elapsed state (IntervalTimer starts as elapsed)
           counter.Update(TimeSpan.Zero);

        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.TryIncrement()
               .Should()
               .BeTrue();

           // Elapse the 1-second timer
           counter.Update(TimeSpan.FromSeconds(1));

        counter.Counter
               .Should()
               .Be(0);

        counter.CanIncrement
               .Should()
               .BeTrue();
    }
}