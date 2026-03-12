#region
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Utilities.SequenceScripter;
using Chaos.Utilities.SequenceScripter.Builder;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Tests;

public sealed class SequenceScripterTests
{
    #region ConditionalAction
    [Test]
    public void ConditionalAction_Update_ConditionTrue_ExecutesActionAndReturnsTrue()
    {
        var executed = false;
        var descriptor = new ConditionalActionDescriptor<string>(_ => true, _ => executed = true);
        var action = new ConditionalAction<string>(descriptor);

        var result = action.Update("entity");

        result.Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void ConditionalAction_Update_ConditionFalse_DoesNotExecuteAndReturnsFalse()
    {
        var executed = false;
        var descriptor = new ConditionalActionDescriptor<string>(_ => false, _ => executed = true);
        var action = new ConditionalAction<string>(descriptor);

        var result = action.Update("entity");

        result.Should()
              .BeFalse();

        executed.Should()
                .BeFalse();
    }

    [Test]
    public void ConditionalAction_Update_PassesEntityToConditionAndAction()
    {
        string? conditionEntity = null;
        string? actionEntity = null;

        var descriptor = new ConditionalActionDescriptor<string>(
            e =>
            {
                conditionEntity = e;

                return true;
            },
            e => actionEntity = e);

        var action = new ConditionalAction<string>(descriptor);

        action.Update("hello");

        conditionEntity.Should()
                       .Be("hello");

        actionEntity.Should()
                    .Be("hello");
    }
    #endregion

    #region ConditionalActionSequence
    [Test]
    public void ConditionalActionSequence_Update_ConditionTrue_UpdatesSequenceAndReturnsTrue()
    {
        var executed = false;

        var seqDescriptor = new TimedActionSequenceDescriptor<string>
        {
            Sequence = [new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => executed = true, true)]
        };

        var descriptor = new ConditionalActionSequenceDescriptor<string>(_ => true, seqDescriptor);
        var action = new ConditionalActionSequence<string>(descriptor);

        var result = action.Update("entity", TimeSpan.FromSeconds(1));

        result.Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void ConditionalActionSequence_Update_ConditionFalse_ReturnsFalse()
    {
        var executed = false;

        var seqDescriptor = new TimedActionSequenceDescriptor<string>
        {
            Sequence = [new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => executed = true, true)]
        };

        var descriptor = new ConditionalActionSequenceDescriptor<string>(_ => false, seqDescriptor);
        var action = new ConditionalActionSequence<string>(descriptor);

        var result = action.Update("entity", TimeSpan.FromSeconds(1));

        result.Should()
              .BeFalse();

        executed.Should()
                .BeFalse();
    }
    #endregion

    #region TimedAction
    [Test]
    public void TimedAction_Update_TimerNotElapsed_ReturnsFalse()
    {
        var executed = false;
        var descriptor = new TimedActionDescriptor<string>(TimeSpan.FromSeconds(5), _ => executed = true);
        var action = new TimedAction<string>(descriptor);

        var result = action.Update("entity", TimeSpan.FromSeconds(1));

        result.Should()
              .BeFalse();

        executed.Should()
                .BeFalse();
    }

    [Test]
    public void TimedAction_Update_TimerElapsed_ExecutesActionAndReturnsTrue()
    {
        var executed = false;
        var descriptor = new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => executed = true);
        var action = new TimedAction<string>(descriptor);

        var result = action.Update("entity", TimeSpan.FromSeconds(1));

        result.Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void TimedAction_Update_StartAsElapsed_FiresOnFirstUpdate()
    {
        var executed = false;
        var descriptor = new TimedActionDescriptor<string>(TimeSpan.FromSeconds(5), _ => executed = true, true);
        var action = new TimedAction<string>(descriptor);

        var result = action.Update("entity", TimeSpan.FromMilliseconds(1));

        result.Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void TimedAction_Update_RepeatedCalls_FiresEachInterval()
    {
        var count = 0;
        var descriptor = new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => count++);
        var action = new TimedAction<string>(descriptor);

        action.Update("entity", TimeSpan.FromMilliseconds(500));

        count.Should()
             .Be(0);

        action.Update("entity", TimeSpan.FromMilliseconds(500));

        count.Should()
             .Be(1);

        action.Update("entity", TimeSpan.FromMilliseconds(500));

        count.Should()
             .Be(1);

        action.Update("entity", TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }
    #endregion

    #region TimedActionSequence
    [Test]
    public void TimedActionSequence_Update_NoInitialTimer_StepsThroughActions()
    {
        var order = new List<int>();

        var descriptor = new TimedActionSequenceDescriptor<string>
        {
            Sequence =
            [
                new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => order.Add(1)),
                new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => order.Add(2)),
                new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => order.Add(3))
            ]
        };

        var sequence = new TimedActionSequence<string>(descriptor);

        // First action fires after 1s
        sequence.Update("e", TimeSpan.FromSeconds(1))
                .Should()
                .BeFalse();

        order.Should()
             .Equal(1);

        // Second action fires after another 1s
        sequence.Update("e", TimeSpan.FromSeconds(1))
                .Should()
                .BeFalse();

        order.Should()
             .Equal(1, 2);

        // Third (last) action fires and returns true (sequence complete)
        sequence.Update("e", TimeSpan.FromSeconds(1))
                .Should()
                .BeTrue();

        order.Should()
             .Equal(1, 2, 3);
    }

    [Test]
    public void TimedActionSequence_Update_WithInitialTimer_FirstCycleFiresImmediately()
    {
        var executed = false;

        var descriptor = new TimedActionSequenceDescriptor<string>
        {
            StartingAtTime = TimeSpan.FromSeconds(3),
            Sequence = [new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => executed = true, true)]
        };

        var sequence = new TimedActionSequence<string>(descriptor);

        // InitialTimer starts as elapsed (IntervalTimer default), so on first update:
        // InitialTimer fires, InitialTimerExpired=true, then action fires (startAsElapsed)
        sequence.Update("e", TimeSpan.FromSeconds(1))
                .Should()
                .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void TimedActionSequence_Update_WithInitialTimer_AfterReset_WaitsForInitialTimer()
    {
        var count = 0;

        var descriptor = new TimedActionSequenceDescriptor<string>
        {
            StartingAtTime = TimeSpan.FromSeconds(3),
            Sequence = [new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => count++, true)]
        };

        var sequence = new TimedActionSequence<string>(descriptor);

        // First cycle — fires immediately (startAsElapsed on both timers)
        sequence.Update("e", TimeSpan.FromSeconds(1))
                .Should()
                .BeTrue();

        count.Should()
             .Be(1);

        // After reset, InitialTimer needs to accumulate time again
        // InitialTimer was not reset in Reset(), but InitialTimerExpired was set to false
        // InitialTimer residual Elapsed from first cycle: 1s - 3s (subtracted) + we need to accumulate more
        // On next Update, InitialTimer.Update adds delta. If Elapsed < Interval, returns false
        sequence.Update("e", TimeSpan.FromSeconds(1))
                .Should()
                .BeFalse();

        count.Should()
             .Be(1);
    }

    [Test]
    public void TimedActionSequence_Update_CompletesSequence_ResetsAndRepeats()
    {
        var count = 0;

        var descriptor = new TimedActionSequenceDescriptor<string>
        {
            Sequence = [new TimedActionDescriptor<string>(TimeSpan.FromSeconds(1), _ => count++)]
        };

        var sequence = new TimedActionSequence<string>(descriptor);

        // First cycle
        sequence.Update("e", TimeSpan.FromSeconds(1))
                .Should()
                .BeTrue();

        count.Should()
             .Be(1);

        // After reset, should repeat
        sequence.Update("e", TimeSpan.FromSeconds(1))
                .Should()
                .BeTrue();

        count.Should()
             .Be(2);
    }

    [Test]
    public void TimedActionSequence_Update_PartialElapse_ReturnsFalse()
    {
        var executed = false;

        var descriptor = new TimedActionSequenceDescriptor<string>
        {
            Sequence = [new TimedActionDescriptor<string>(TimeSpan.FromSeconds(5), _ => executed = true)]
        };

        var sequence = new TimedActionSequence<string>(descriptor);

        sequence.Update("e", TimeSpan.FromSeconds(2))
                .Should()
                .BeFalse();

        executed.Should()
                .BeFalse();
    }
    #endregion

    #region ThresholdAction
    private static Monster CreateMonsterWithHealth(int currentHp, int maxHp = 1000)
    {
        var monster = MockMonster.Create(
            templateSetup: t => t with
            {
                StatSheet = new StatSheet
                {
                    CurrentHp = currentHp,
                    MaximumHp = maxHp
                }
            });

        return monster;
    }

    [Test]
    public void ThresholdAction_Update_HealthAboveThreshold_ReturnsFalse()
    {
        var monster = CreateMonsterWithHealth(1000);
        var executed = false;
        var descriptor = new ThresholdActionDescriptor<Monster>(50, _ => executed = true);
        var action = new ThresholdAction<Monster>(descriptor);

        var result = action.Update(monster, TimeSpan.FromSeconds(1));

        result.Should()
              .BeFalse();

        executed.Should()
                .BeFalse();
    }

    [Test]
    public void ThresholdAction_Update_HealthCrossesThreshold_ActivatesAndExecutes()
    {
        var monster = CreateMonsterWithHealth(1000);
        var executed = false;
        var descriptor = new ThresholdActionDescriptor<Monster>(50, _ => executed = true);
        var action = new ThresholdAction<Monster>(descriptor);

        // First update at 100% health — establishes PreviousValue
        action.Update(monster, TimeSpan.FromSeconds(1))
              .Should()
              .BeFalse();

        // Drop health below threshold
        monster.StatSheet.SetHealthPct(40);

        // Now the threshold crosses: previous=100, current=40, threshold=50
        var result = action.Update(monster, TimeSpan.FromSeconds(1));

        result.Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void ThresholdAction_Update_HealthExactlyAtThreshold_Activates()
    {
        var monster = CreateMonsterWithHealth(1000);
        var executed = false;
        var descriptor = new ThresholdActionDescriptor<Monster>(50, _ => executed = true);
        var action = new ThresholdAction<Monster>(descriptor);

        // First update at 100%
        action.Update(monster, TimeSpan.FromSeconds(1));

        // Set health to exactly 50%
        monster.StatSheet.SetHealthPct(50);

        var result = action.Update(monster, TimeSpan.FromSeconds(1));

        result.Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void ThresholdAction_Update_WithDelay_DelayTimerStartsElapsed()
    {
        var monster = CreateMonsterWithHealth(1000);
        var executed = false;

        var descriptor = new ThresholdActionDescriptor<Monster>(50, _ => executed = true)
        {
            DelayAfterThreshold = TimeSpan.FromSeconds(3)
        };

        var action = new ThresholdAction<Monster>(descriptor);

        // First update at 100%
        action.Update(monster, TimeSpan.FromSeconds(1));

        // Cross threshold — delay timer starts as elapsed (IntervalTimer default),
        // so it fires immediately on the activation update
        monster.StatSheet.SetHealthPct(40);

        action.Update(monster, TimeSpan.FromSeconds(1))
              .Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void ThresholdAction_Update_HealthAlreadyBelowThreshold_DoesNotActivate()
    {
        // Health starts at 30% — never crosses the threshold from above
        var monster = CreateMonsterWithHealth(300);
        var executed = false;
        var descriptor = new ThresholdActionDescriptor<Monster>(50, _ => executed = true);
        var action = new ThresholdAction<Monster>(descriptor);

        // PreviousValue starts at 100.0m (hardcoded), current=30 — this WILL cross threshold
        // because previousHealthPercent=100 > 50 >= currentHealthPercent=30
        action.Update(monster, TimeSpan.FromSeconds(1))
              .Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void ThresholdAction_Update_AlreadyActivated_ContinuesExecuting()
    {
        var monster = CreateMonsterWithHealth(1000);
        var count = 0;
        var descriptor = new ThresholdActionDescriptor<Monster>(50, _ => count++);
        var action = new ThresholdAction<Monster>(descriptor);

        // First update
        action.Update(monster, TimeSpan.FromSeconds(1));

        // Cross threshold
        monster.StatSheet.SetHealthPct(40);
        action.Update(monster, TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Still activated — continues executing even though health doesn't re-cross
        action.Update(monster, TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }
    #endregion

    #region ThresholdActionSequence
    [Test]
    public void ThresholdActionSequence_Update_HealthAboveThreshold_ReturnsFalse()
    {
        var monster = CreateMonsterWithHealth(1000);
        var executed = false;

        var seqDescriptor = new TimedActionSequenceDescriptor<Monster>
        {
            Sequence = [new TimedActionDescriptor<Monster>(TimeSpan.FromSeconds(1), _ => executed = true, true)]
        };

        var descriptor = new ThresholdActionSequenceDescriptor<Monster>(50, seqDescriptor);
        var action = new ThresholdActionSequence<Monster>(descriptor);

        action.Update(monster, TimeSpan.FromSeconds(1))
              .Should()
              .BeFalse();

        executed.Should()
                .BeFalse();
    }

    [Test]
    public void ThresholdActionSequence_Update_HealthCrossesThreshold_UpdatesSequence()
    {
        var monster = CreateMonsterWithHealth(1000);
        var executed = false;

        var seqDescriptor = new TimedActionSequenceDescriptor<Monster>
        {
            Sequence = [new TimedActionDescriptor<Monster>(TimeSpan.FromSeconds(1), _ => executed = true, true)]
        };

        var descriptor = new ThresholdActionSequenceDescriptor<Monster>(50, seqDescriptor);
        var action = new ThresholdActionSequence<Monster>(descriptor);

        // Establish PreviousValue
        action.Update(monster, TimeSpan.FromSeconds(1));

        // Cross threshold
        monster.StatSheet.SetHealthPct(40);
        var result = action.Update(monster, TimeSpan.FromSeconds(1));

        result.Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void ThresholdActionSequence_Update_WithDelay_DelayTimerStartsElapsed()
    {
        var monster = CreateMonsterWithHealth(1000);
        var executed = false;

        var seqDescriptor = new TimedActionSequenceDescriptor<Monster>
        {
            Sequence = [new TimedActionDescriptor<Monster>(TimeSpan.FromSeconds(1), _ => executed = true, true)]
        };

        var descriptor = new ThresholdActionSequenceDescriptor<Monster>(50, seqDescriptor)
        {
            DelayAfterThreshold = TimeSpan.FromSeconds(2)
        };

        var action = new ThresholdActionSequence<Monster>(descriptor);

        // Establish PreviousValue
        action.Update(monster, TimeSpan.FromSeconds(1));

        // Cross threshold — delay timer starts as elapsed (IntervalTimer default),
        // fires immediately, then sequence fires (startAsElapsed)
        monster.StatSheet.SetHealthPct(40);

        action.Update(monster, TimeSpan.FromSeconds(1))
              .Should()
              .BeTrue();

        executed.Should()
                .BeTrue();
    }
    #endregion

    #region TimedActionSequenceBuilder
    [Test]
    public void TimedActionSequenceBuilder_AfterTime_AddsAction()
    {
        var builder = new TimedActionSequenceBuilder<string>();

        builder.AfterTime(TimeSpan.FromSeconds(1), _ => { });

        var descriptor = builder.Build();

        descriptor.Sequence
                  .Should()
                  .HaveCount(1);

        descriptor.StartingAtTime
                  .Should()
                  .BeNull();
    }

    [Test]
    public void TimedActionSequenceBuilder_ThenAfter_ChainsActions()
    {
        var builder = new TimedActionSequenceBuilder<string>();

        builder.AfterTime(TimeSpan.FromSeconds(1), _ => { })
               .ThenAfter(TimeSpan.FromSeconds(2), _ => { })
               .ThenAfter(TimeSpan.FromSeconds(3), _ => { });

        var descriptor = builder.Build();

        descriptor.Sequence
                  .Should()
                  .HaveCount(3);
    }

    [Test]
    public void TimedActionSequenceBuilder_Build_WithStartingTime_SetsStartingAtTime()
    {
        var builder = new TimedActionSequenceBuilder<string>();
        builder.AfterTime(TimeSpan.FromSeconds(1), _ => { });

        var descriptor = builder.Build(TimeSpan.FromSeconds(5));

        descriptor.StartingAtTime
                  .Should()
                  .Be(TimeSpan.FromSeconds(5));
    }

    [Test]
    public void TimedActionSequenceBuilder_Build_WithoutStartingTime_NullStartingAtTime()
    {
        var builder = new TimedActionSequenceBuilder<string>();
        builder.AfterTime(TimeSpan.FromSeconds(1), _ => { });

        var descriptor = builder.Build();

        descriptor.StartingAtTime
                  .Should()
                  .BeNull();
    }

    [Test]
    public void TimedActionSequenceBuilder_AfterTime_StartAsElapsed_SetOnDescriptor()
    {
        var builder = new TimedActionSequenceBuilder<string>();
        builder.AfterTime(TimeSpan.FromSeconds(1), _ => { }, true);

        var descriptor = builder.Build();

        descriptor.Sequence[0]
                  .StartAsElapsed
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void TimedActionSequenceBuilder_ReturnsSelf_ForFluency()
    {
        var builder = new TimedActionSequenceBuilder<string>();

        var result1 = builder.AfterTime(TimeSpan.FromSeconds(1), _ => { });
        var result2 = builder.ThenAfter(TimeSpan.FromSeconds(1), _ => { });

        result1.Should()
               .BeSameAs(builder);

        result2.Should()
               .BeSameAs(builder);
    }
    #endregion

    #region ScriptBuilder
    [Test]
    public void ScriptBuilder_WhileThenRepeatAction_Build_CreatesSequenceWithRepeatedConditionalAction()
    {
        var executed = false;

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));

        builder.WhileThenRepeatAction(_ => true, _ => executed = true);

        var sequence = builder.Build("entity");

        // ScriptTimer starts as elapsed (IntervalTimer default), so fires on first update
        sequence.Update(TimeSpan.FromMilliseconds(1));

        executed.Should()
                .BeTrue();

        // Resets, then needs full interval to fire again
        executed = false;
        sequence.Update(TimeSpan.FromMilliseconds(500));

        executed.Should()
                .BeFalse();

        sequence.Update(TimeSpan.FromMilliseconds(500));

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void ScriptBuilder_WhenThenDoActionOnce_Build_RemovesAfterExecution()
    {
        var count = 0;

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.WhenThenDoActionOnce(_ => true, _ => count++);

        var sequence = builder.Build("entity");

        // First interval — fires and removes
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Second interval — already removed, should not fire again
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void ScriptBuilder_WhenThenDoActionOnce_ConditionFalse_DoesNotRemove()
    {
        var count = 0;
        var conditionResult = false;

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.WhenThenDoActionOnce(_ => conditionResult, _ => count++);

        var sequence = builder.Build("entity");

        // First interval — condition false, stays
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Now condition becomes true
        conditionResult = true;
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Removed — no more
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void ScriptBuilder_AfterTimeDoActionOnce_Build_FiresOnceAfterDelay()
    {
        var count = 0;

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.AfterTimeDoActionOnce(TimeSpan.FromSeconds(3), _ => count++);

        var sequence = builder.Build("entity");

        // First script interval — timed action timer at 1/3
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Second — 2/3
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Third — fires and is removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Fourth — already removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void ScriptBuilder_AfterTimeRepeatAction_Build_FiresRepeatedlyAfterInterval()
    {
        var count = 0;

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.AfterTimeRepeatAction(TimeSpan.FromSeconds(2), _ => count++);

        var sequence = builder.Build("entity");

        // First interval — timed action at 1/2
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Second — fires
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Third — at 1/2 again
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Fourth — fires again
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }

    [Test]
    public void ScriptBuilder_WhileThenRepeatSequence_Build_ExecutesSequenceWhileConditionTrue()
    {
        var order = new List<int>();

        var seqBuilder = new TimedActionSequenceBuilder<string>();

        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => order.Add(1), true)
                  .ThenAfter(TimeSpan.FromSeconds(1), _ => order.Add(2), true);

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.WhileThenRepeatSequence(_ => true, seqBuilder);

        var sequence = builder.Build("entity");

        // First interval — first action fires (startAsElapsed)
        sequence.Update(TimeSpan.FromSeconds(1));

        order.Should()
             .Equal(1);

        // Second interval — second action fires (startAsElapsed), sequence completes
        sequence.Update(TimeSpan.FromSeconds(1));

        order.Should()
             .Equal(1, 2);
    }

    [Test]
    public void ScriptBuilder_WhenThenDoSequenceOnce_Build_ExecutesSequenceOnceWhenConditionMet()
    {
        var count = 0;

        var seqBuilder = new TimedActionSequenceBuilder<string>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.WhenThenDoSequenceOnce(_ => true, seqBuilder);

        var sequence = builder.Build("entity");

        // First interval — condition true, sequence fires and completes, removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Already removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void ScriptBuilder_AfterTimeDoSequenceOnce_Build_FiresSequenceOnce()
    {
        var count = 0;

        var seqBuilder = new TimedActionSequenceBuilder<string>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.AfterTimeDoSequenceOnce(TimeSpan.FromSeconds(2), seqBuilder);

        var sequence = builder.Build("entity");

        // First interval — InitialTimer starts elapsed (IntervalTimer default),
        // fires immediately, action fires (startAsElapsed), sequence completes, removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void ScriptBuilder_AfterTimeRepeatSequence_Build_RepeatsSequence()
    {
        var count = 0;

        var seqBuilder = new TimedActionSequenceBuilder<string>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.AfterTimeRepeatSequence(TimeSpan.FromSeconds(2), seqBuilder);

        var sequence = builder.Build("entity");

        // First interval — InitialTimer starts elapsed, fires immediately,
        // action fires (startAsElapsed), sequence completes and resets
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // After reset, InitialTimer needs to accumulate time again
        // (InitialTimerExpired reset to false, InitialTimer has residual elapsed from first cycle)
        // Eventually fires again
        sequence.Update(TimeSpan.FromSeconds(1));
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .BeGreaterThanOrEqualTo(1);
    }

    [Test]
    public void ScriptBuilder_FluentApi_ReturnsSelf()
    {
        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        var seqBuilder = new TimedActionSequenceBuilder<string>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => { });

        builder.WhileThenRepeatAction(_ => true, _ => { })
               .Should()
               .BeSameAs(builder);

        builder.WhileThenRepeatSequence(_ => true, seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.WhenThenDoActionOnce(_ => true, _ => { })
               .Should()
               .BeSameAs(builder);

        builder.WhenThenDoSequenceOnce(_ => true, seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.AfterTimeDoActionOnce(TimeSpan.FromSeconds(1), _ => { })
               .Should()
               .BeSameAs(builder);

        builder.AfterTimeDoSequenceOnce(TimeSpan.FromSeconds(1), seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.AfterTimeRepeatAction(TimeSpan.FromSeconds(1), _ => { })
               .Should()
               .BeSameAs(builder);

        builder.AfterTimeRepeatSequence(TimeSpan.FromSeconds(1), seqBuilder)
               .Should()
               .BeSameAs(builder);
    }

    [Test]
    public void ScriptBuilder_AfterTimeDoActionOnce_StartAsElapsed_FiresImmediately()
    {
        var count = 0;

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.AfterTimeDoActionOnce(TimeSpan.FromSeconds(5), _ => count++, true);

        var sequence = builder.Build("entity");

        // First interval — startAsElapsed means timer already elapsed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }
    #endregion

    #region CreatureScriptBuilder
    [Test]
    public void CreatureScriptBuilder_Build_CreatesCreatureScriptedSequence()
    {
        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        var monster = CreateMonsterWithHealth(1000);

        var sequence = builder.Build(monster);

        sequence.Should()
                .BeOfType<CreatureScriptedSequence<Monster>>();
    }

    [Test]
    public void CreatureScriptBuilder_AtThresholdDoActionOnce_FiresWhenHealthCrosses()
    {
        var executed = false;
        var monster = CreateMonsterWithHealth(1000);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdDoActionOnce(50, _ => executed = true);

        var sequence = builder.Build(monster);

        // First interval at full health
        sequence.Update(TimeSpan.FromSeconds(1));

        executed.Should()
                .BeFalse();

        // Drop health
        monster.StatSheet.SetHealthPct(40);

        // Threshold crossed — fires and should be removed
        sequence.Update(TimeSpan.FromSeconds(1));

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void CreatureScriptBuilder_AtThresholdRepeatAction_RepeatsAfterActivation()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdRepeatAction(50, _ => count++);

        var sequence = builder.Build(monster);

        // First interval at full health
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Cross threshold
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Continues firing (repeated, not removed)
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }

    [Test]
    public void CreatureScriptBuilder_AtThresholdDoSequenceOnce_FiresSequenceOnce()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var seqBuilder = new TimedActionSequenceBuilder<Monster>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdDoSequenceOnce(50, seqBuilder);

        var sequence = builder.Build(monster);

        // Full health
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Cross threshold — sequence fires, completes, removed
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void CreatureScriptBuilder_AtThresholdRepeatSequence_RepeatsSequence()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var seqBuilder = new TimedActionSequenceBuilder<Monster>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdRepeatSequence(50, seqBuilder);

        var sequence = builder.Build(monster);

        // Full health
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Cross threshold
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Repeats
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }

    [Test]
    public void CreatureScriptBuilder_AtThresholdThenAfterTimeDoActionOnce_FiresWhenThresholdCrossed()
    {
        var executed = false;
        var monster = CreateMonsterWithHealth(1000);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdThenAfterTimeDoActionOnce(TimeSpan.FromSeconds(2), 50, _ => executed = true);

        var sequence = builder.Build(monster);

        // Full health — ScriptTimer fires (startAsElapsed), but threshold not crossed
        sequence.Update(TimeSpan.FromSeconds(1));

        executed.Should()
                .BeFalse();

        // Cross threshold — delay timer starts as elapsed (IntervalTimer default),
        // fires immediately on activation
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        executed.Should()
                .BeTrue();
    }

    [Test]
    public void CreatureScriptBuilder_AtThresholdThenAfterTimeDoSequenceOnce_FiresWhenThresholdCrossed()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var seqBuilder = new TimedActionSequenceBuilder<Monster>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdThenAfterTimeDoSequenceOnce(TimeSpan.FromSeconds(2), 50, seqBuilder);

        var sequence = builder.Build(monster);

        // Full health
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Cross threshold — delay timer fires immediately (startAsElapsed),
        // then sequence fires (startAsElapsed)
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Removed (one-shot)
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void CreatureScriptBuilder_AtThresholdThenAfterTimeRepeatAction_FiresOnThresholdCross()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdThenAfterTimeRepeatAction(TimeSpan.FromSeconds(2), 50, _ => count++);

        var sequence = builder.Build(monster);

        // Full health
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Cross threshold — delay timer fires immediately (startAsElapsed)
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Continues repeating
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .BeGreaterThanOrEqualTo(1);
    }

    [Test]
    public void CreatureScriptBuilder_AtThresholdThenAfterTimeRepeatSequence_FiresOnThresholdCross()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var seqBuilder = new TimedActionSequenceBuilder<Monster>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdThenAfterTimeRepeatSequence(TimeSpan.FromSeconds(2), 50, seqBuilder);

        var sequence = builder.Build(monster);

        // Full health
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Cross threshold — delay timer fires immediately (startAsElapsed),
        // then sequence fires (startAsElapsed)
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Continues repeating
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }

    [Test]
    public void CreatureScriptBuilder_FluentApi_ReturnsSelf()
    {
        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        var seqBuilder = new TimedActionSequenceBuilder<Monster>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => { });

        builder.WhileThenRepeatAction(_ => true, _ => { })
               .Should()
               .BeSameAs(builder);

        builder.WhileThenRepeatSequence(_ => true, seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.WhenThenDoActionOnce(_ => true, _ => { })
               .Should()
               .BeSameAs(builder);

        builder.WhenThenDoSequenceOnce(_ => true, seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.AfterTimeDoActionOnce(TimeSpan.FromSeconds(1), _ => { })
               .Should()
               .BeSameAs(builder);

        builder.AfterTimeDoSequenceOnce(TimeSpan.FromSeconds(1), seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.AfterTimeRepeatAction(TimeSpan.FromSeconds(1), _ => { })
               .Should()
               .BeSameAs(builder);

        builder.AfterTimeRepeatSequence(TimeSpan.FromSeconds(1), seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.AtThresholdDoActionOnce(50, _ => { })
               .Should()
               .BeSameAs(builder);

        builder.AtThresholdDoSequenceOnce(50, seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.AtThresholdRepeatAction(50, _ => { })
               .Should()
               .BeSameAs(builder);

        builder.AtThresholdRepeatSequence(50, seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.AtThresholdThenAfterTimeDoActionOnce(TimeSpan.FromSeconds(1), 50, _ => { })
               .Should()
               .BeSameAs(builder);

        builder.AtThresholdThenAfterTimeDoSequenceOnce(TimeSpan.FromSeconds(1), 50, seqBuilder)
               .Should()
               .BeSameAs(builder);

        builder.AtThresholdThenAfterTimeRepeatAction(TimeSpan.FromSeconds(1), 50, _ => { })
               .Should()
               .BeSameAs(builder);

        builder.AtThresholdThenAfterTimeRepeatSequence(TimeSpan.FromSeconds(1), 50, seqBuilder)
               .Should()
               .BeSameAs(builder);
    }
    #endregion

    #region ScriptedSequence Integration
    [Test]
    public void ScriptedSequence_Update_SecondCycleBeforeInterval_DoesNothing()
    {
        var count = 0;

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(2));
        builder.WhileThenRepeatAction(_ => true, _ => count++);

        var sequence = builder.Build("entity");

        // First update fires immediately (ScriptTimer starts as elapsed)
        sequence.Update(TimeSpan.FromMilliseconds(1));

        count.Should()
             .Be(1);

        // Second update before the 2s interval — should not fire
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Accumulate enough time for the interval
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }

    [Test]
    public void ScriptedSequence_Update_MixedActions_ExecutesAllTypes()
    {
        var results = new List<string>();

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));

        // Repeated conditional — fires every interval while condition true
        builder.WhileThenRepeatAction(_ => true, _ => results.Add("repeated-cond"));

        // One-time conditional — fires once when condition true
        builder.WhenThenDoActionOnce(_ => true, _ => results.Add("once-cond"));

        // Repeated timed — fires every 1s (same as script interval, so fires each cycle)
        builder.AfterTimeRepeatAction(TimeSpan.FromSeconds(1), _ => results.Add("repeated-timed"), true);

        // One-time timed — fires once after 1s
        builder.AfterTimeDoActionOnce(TimeSpan.FromSeconds(1), _ => results.Add("once-timed"), true);

        var sequence = builder.Build("entity");

        // First interval
        sequence.Update(TimeSpan.FromSeconds(1));

        results.Should()
               .Contain("repeated-cond");

        results.Should()
               .Contain("once-cond");

        results.Should()
               .Contain("repeated-timed");

        results.Should()
               .Contain("once-timed");

        results.Clear();

        // Second interval — one-time actions should be removed
        sequence.Update(TimeSpan.FromSeconds(1));

        results.Should()
               .Contain("repeated-cond");

        results.Should()
               .Contain("repeated-timed");

        results.Should()
               .NotContain("once-cond");

        results.Should()
               .NotContain("once-timed");
    }

    [Test]
    public void ScriptedSequence_Update_RepeatedConditionalSequence_ExecutesSequenceWhileConditionTrue()
    {
        var count = 0;

        var seqBuilder = new TimedActionSequenceBuilder<string>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.WhileThenRepeatSequence(_ => true, seqBuilder);

        var sequence = builder.Build("entity");

        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }

    [Test]
    public void ScriptedSequence_Update_OneShotConditionalSequence_RemovedAfterCompletion()
    {
        var count = 0;

        var seqBuilder = new TimedActionSequenceBuilder<string>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.WhenThenDoSequenceOnce(_ => true, seqBuilder);

        var sequence = builder.Build("entity");

        // Fires and completes — removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Already removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void ScriptedSequence_Update_RepeatedTimedSequence_RepeatsAfterCompletion()
    {
        var order = new List<int>();

        var seqBuilder = new TimedActionSequenceBuilder<string>();

        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => order.Add(1), true)
                  .ThenAfter(TimeSpan.FromSeconds(1), _ => order.Add(2), true);

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.AfterTimeRepeatSequence(TimeSpan.FromSeconds(1), seqBuilder);

        var sequence = builder.Build("entity");

        // Wait for initial timer (1s) — uses the starting time from Build(startingTime)
        // Actually AfterTimeRepeatSequence passes time as starting time
        // First interval — initial timer (1s) elapses, first step fires
        sequence.Update(TimeSpan.FromSeconds(1));

        order.Should()
             .Equal(1);

        // Second step fires
        sequence.Update(TimeSpan.FromSeconds(1));

        order.Should()
             .Equal(1, 2);

        // Reset — initial timer again. Wait for it to elapse
        sequence.Update(TimeSpan.FromSeconds(1));

        order.Should()
             .Equal(1, 2, 1);
    }

    [Test]
    public void ScriptedSequence_Update_OneShotTimedSequence_RemovedAfterCompletion()
    {
        var count = 0;

        var seqBuilder = new TimedActionSequenceBuilder<string>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new ScriptBuilder<string>(TimeSpan.FromSeconds(1));
        builder.AfterTimeDoSequenceOnce(TimeSpan.FromSeconds(1), seqBuilder);

        var sequence = builder.Build("entity");

        // Initial timer (1s) elapses, action fires, sequence completes — removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }
    #endregion

    #region CreatureScriptedSequence Integration
    [Test]
    public void CreatureScriptedSequence_Update_ThresholdActions_FireOnHealthCross()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdDoActionOnce(50, _ => count++);
        builder.AtThresholdRepeatAction(75, _ => count += 10);

        var sequence = builder.Build(monster);

        // Full health — neither threshold crossed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Drop to 70% — crosses 75 threshold
        monster.StatSheet.SetHealthPct(70);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(10);

        // Still below 75 — repeated threshold continues
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(20);

        // Drop to 40% — crosses 50 threshold
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(31); // 20 + 10 (repeated 75) + 1 (once 50)

        // 50 threshold removed (once), 75 continues
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(41); // 31 + 10 (repeated 75 only)
    }

    [Test]
    public void CreatureScriptedSequence_Update_CombinesBaseAndThresholdActions()
    {
        var results = new List<string>();
        var monster = CreateMonsterWithHealth(1000);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.WhileThenRepeatAction(_ => true, _ => results.Add("conditional"));
        builder.AtThresholdRepeatAction(50, _ => results.Add("threshold"));

        var sequence = builder.Build(monster);

        // Full health — only conditional fires
        sequence.Update(TimeSpan.FromSeconds(1));

        results.Should()
               .Equal("conditional");

        results.Clear();

        // Cross threshold — both fire
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        results.Should()
               .Contain("conditional");

        results.Should()
               .Contain("threshold");
    }

    [Test]
    public void CreatureScriptedSequence_Update_SecondCycleBeforeInterval_DoesNotFireThreshold()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(2));
        builder.AtThresholdRepeatAction(50, _ => count++);

        var sequence = builder.Build(monster);

        // First update — ScriptTimer fires (startAsElapsed), but health still at 100%
        sequence.Update(TimeSpan.FromMilliseconds(1));

        count.Should()
             .Be(0);

        // Drop health and cross threshold
        monster.StatSheet.SetHealthPct(40);

        // Before the 2s interval elapses — should not fire
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Accumulate enough time for the interval
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void CreatureScriptedSequence_Update_ThresholdSequence_OneShotRemoved()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var seqBuilder = new TimedActionSequenceBuilder<Monster>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdDoSequenceOnce(50, seqBuilder);

        var sequence = builder.Build(monster);

        // Full health
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Cross threshold — sequence fires and completes — removed
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Already removed
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);
    }

    [Test]
    public void CreatureScriptedSequence_Update_ThresholdSequence_RepeatedContinues()
    {
        var count = 0;
        var monster = CreateMonsterWithHealth(1000);

        var seqBuilder = new TimedActionSequenceBuilder<Monster>();
        seqBuilder.AfterTime(TimeSpan.FromSeconds(1), _ => count++, true);

        var builder = new CreatureScriptBuilder<Monster>(TimeSpan.FromSeconds(1));
        builder.AtThresholdRepeatSequence(50, seqBuilder);

        var sequence = builder.Build(monster);

        // Full health
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(0);

        // Cross threshold
        monster.StatSheet.SetHealthPct(40);
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(1);

        // Repeated — keeps firing
        sequence.Update(TimeSpan.FromSeconds(1));

        count.Should()
             .Be(2);
    }
    #endregion
}