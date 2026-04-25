#region
using Chaos.Scripting.DialogScripts.Quests;
using Chaos.Services.Quests;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Utilities.QuestHelper;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class QuestDialogScriptTests
{
    private enum FakeStage { None, Started, Done }

    /// <summary>Quest that registers a single handler on the configured phase and records each invocation.</summary>
    private sealed class PhaseTrackingQuest : Quest<FakeStage>
    {
        public List<string> Calls { get; } = [];
        public override string Key => "phase-tracking";
        public DialogPhase Phase { get; init; } = DialogPhase.Next;

        protected override void Configure(QuestBuilder<FakeStage> q)
        {
            var step = Phase switch
            {
                DialogPhase.Displaying => q.OnDisplaying("foo"),
                DialogPhase.Displayed => q.OnDisplayed("foo"),
                DialogPhase.Next => q.OnNext("foo"),
                DialogPhase.Previous => q.OnPrevious("foo"),
                _ => throw new ArgumentOutOfRangeException(nameof(Phase))
            };

            step.Run((_, _) => Calls.Add("ran"));
        }
    }

    private sealed class MultiHandlerQuest : Quest<FakeStage>
    {
        public List<string> Calls { get; } = [];
        public override string Key => "multi";

        protected override void Configure(QuestBuilder<FakeStage> q)
        {
            q.OnNext("foo")
             .Run((_, _) => Calls.Add("foo"));

            q.OnNext("bar")
             .Run((_, _) => Calls.Add("bar"));
        }
    }

    private sealed class AdvanceQuest : Quest<FakeStage>
    {
        public override string Key => "advance";

        protected override void Configure(QuestBuilder<FakeStage> q)
            => q.OnNext("foo")
                .When(FakeStage.None)
                .Advance(FakeStage.Started);
    }

    private sealed class HaltingQuest : Quest<FakeStage>
    {
        public List<string> Calls { get; } = [];
        public override string Key => "halting";

        protected override void Configure(QuestBuilder<FakeStage> q)
            => q.OnNext("foo")
                .When(FakeStage.Done) // halts immediately, aisling is at None
                .Run((_, _) => Calls.Add("should_not_run"));
    }

    private sealed class SiblingQuest : Quest<FakeStage>
    {
        public List<string> Calls { get; } = [];
        public override string Key => "sibling";

        protected override void Configure(QuestBuilder<FakeStage> q)
            => q.OnNext("foo")
                .Run((_, _) => Calls.Add("sibling_ran"));
    }

    private sealed class OptionCapturingQuest : Quest<FakeStage>
    {
        public bool WasInvoked { get; private set; }
        public byte? CapturedOptionIndex { get; private set; }
        public override string Key => "option-capture";
        public DialogPhase Phase { get; init; } = DialogPhase.Next;

        protected override void Configure(QuestBuilder<FakeStage> q)
        {
            var step = Phase switch
            {
                DialogPhase.Displaying => q.OnDisplaying("foo"),
                DialogPhase.Displayed => q.OnDisplayed("foo"),
                DialogPhase.Next => q.OnNext("foo"),
                DialogPhase.Previous => q.OnPrevious("foo"),
                _ => throw new ArgumentOutOfRangeException(nameof(Phase))
            };

            step.Run((_, ctx) =>
            {
                WasInvoked = true;
                CapturedOptionIndex = ctx.OptionIndex;
            });
        }
    }

    private static QuestRegistry CreateRegistry(params Quest[] quests)
    {
        var registry = new QuestRegistry(MockServiceProvider.Create().Object);

        foreach (var quest in quests)
        {
            quest.RunConfigure();
            registry.Register(quest);
        }

        return registry;
    }

    //formatter:off
    [Test]
    [Arguments(DialogPhase.Displaying)]
    [Arguments(DialogPhase.Displayed)]
    [Arguments(DialogPhase.Next)]
    [Arguments(DialogPhase.Previous)]
    //formatter:on
    public void Dispatch_RoutesToHandlerMatchingPhase(DialogPhase registered)
    {
        var quest = new PhaseTrackingQuest { Phase = registered };
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        InvokePhase(script, aisling, registered);

        quest.Calls.Should().ContainSingle().And.Contain("ran");
    }

    //formatter:off
    [Test]
    [Arguments(DialogPhase.Displaying, DialogPhase.Next)]
    [Arguments(DialogPhase.Next, DialogPhase.Displaying)]
    [Arguments(DialogPhase.Next, DialogPhase.Displayed)]
    [Arguments(DialogPhase.Next, DialogPhase.Previous)]
    [Arguments(DialogPhase.Previous, DialogPhase.Next)]
    //formatter:on
    public void Dispatch_DoesNotInvokeHandlerRegisteredForDifferentPhase(DialogPhase registered, DialogPhase invoked)
    {
        var quest = new PhaseTrackingQuest { Phase = registered };
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        InvokePhase(script, aisling, invoked);

        quest.Calls.Should().BeEmpty();
    }

    [Test]
    public void Dispatch_NoOpsWhenNoHandlers()
    {
        var registry = CreateRegistry();
        var dialog = MockDialog.Create("nobody_cares");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        Action act = () => script.OnDisplaying(aisling);

        act.Should().NotThrow();
    }

    [Test]
    public void Dispatch_OnlyInvokesHandlersMatchingTemplateKey()
    {
        var quest = new MultiHandlerQuest();
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnNext(aisling);

        quest.Calls.Should().ContainSingle().And.Contain("foo");
    }

    [Test]
    public void Dispatch_BuildsContextWithCurrentStageFromTrackers()
    {
        var quest = new AdvanceQuest();
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        // Set the explicit stage so `When(FakeStage.None)` matches — an absent enum does not
        // satisfy a stage guard even when the guard value equals default(TStage).
        aisling.Trackers.Enums.Set(FakeStage.None);

        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnNext(aisling);

        aisling.Trackers.Enums.TryGetValue<FakeStage>(out var current).Should().BeTrue();
        current.Should().Be(FakeStage.Started);
    }

    [Test]
    public void Dispatch_HaltInOneHandlerDoesNotAffectOthers()
    {
        var halting = new HaltingQuest();
        var sibling = new SiblingQuest();
        var registry = CreateRegistry(halting, sibling);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnNext(aisling);

        halting.Calls.Should().BeEmpty("guard halted this quest's chain");
        sibling.Calls.Should().ContainSingle().And.Contain("sibling_ran");
    }

    [Test]
    public void Dispatch_OnNext_PropagatesOptionIndexIntoContext()
    {
        var quest = new OptionCapturingQuest();
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnNext(aisling, optionIndex: 3);

        quest.WasInvoked.Should().BeTrue();
        quest.CapturedOptionIndex.Should().Be(3);
    }

    [Test]
    public void Dispatch_OnNext_WithoutOptionIndex_LeavesContextOptionIndexNull()
    {
        var quest = new OptionCapturingQuest();
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnNext(aisling);

        quest.WasInvoked.Should().BeTrue();
        quest.CapturedOptionIndex.Should().BeNull();
    }

    //formatter:off
    [Test]
    [Arguments(DialogPhase.Displaying)]
    [Arguments(DialogPhase.Displayed)]
    [Arguments(DialogPhase.Previous)]
    //formatter:on
    public void Dispatch_NonNextPhases_AlwaysHaveNullOptionIndex(DialogPhase phase)
    {
        var quest = new OptionCapturingQuest { Phase = phase };
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        InvokePhase(script, aisling, phase);

        quest.WasInvoked.Should().BeTrue();
        quest.CapturedOptionIndex.Should().BeNull();
    }

    private static void InvokePhase(QuestDialogScript script, Chaos.Models.World.Aisling aisling, DialogPhase phase)
    {
        switch (phase)
        {
            case DialogPhase.Displaying: script.OnDisplaying(aisling); break;
            case DialogPhase.Displayed: script.OnDisplayed(aisling); break;
            case DialogPhase.Next: script.OnNext(aisling); break;
            case DialogPhase.Previous: script.OnPrevious(aisling); break;
            default: throw new ArgumentOutOfRangeException(nameof(phase));
        }
    }
}
