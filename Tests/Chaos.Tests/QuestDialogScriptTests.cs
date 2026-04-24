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

    private sealed class TrackingQuest : Quest<FakeStage>
    {
        public List<string> Calls { get; } = [];
        public override string Key => "tracking";

        protected override void Configure(QuestBuilder<FakeStage> q)
            => q.OnDialog("foo")
                .Run((_, _) => Calls.Add("ran"));
    }

    private sealed class MultiHandlerQuest : Quest<FakeStage>
    {
        public List<string> Calls { get; } = [];
        public override string Key => "multi";

        protected override void Configure(QuestBuilder<FakeStage> q)
        {
            q.OnDialog("foo")
             .Run((_, _) => Calls.Add("foo"));

            q.OnDialog("bar")
             .Run((_, _) => Calls.Add("bar"));
        }
    }

    private sealed class AdvanceQuest : Quest<FakeStage>
    {
        public override string Key => "advance";

        protected override void Configure(QuestBuilder<FakeStage> q)
            => q.OnDialog("foo")
                .When(FakeStage.None)
                .Advance(FakeStage.Started);
    }

    private sealed class HaltingQuest : Quest<FakeStage>
    {
        public List<string> Calls { get; } = [];
        public override string Key => "halting";

        protected override void Configure(QuestBuilder<FakeStage> q)
            => q.OnDialog("foo")
                .When(FakeStage.Done) // halts immediately, aisling is at None
                .Run((_, _) => Calls.Add("should_not_run"));
    }

    private sealed class SiblingQuest : Quest<FakeStage>
    {
        public List<string> Calls { get; } = [];
        public override string Key => "sibling";

        protected override void Configure(QuestBuilder<FakeStage> q)
            => q.OnDialog("foo")
                .Run((_, _) => Calls.Add("sibling_ran"));
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

    [Test]
    public void OnDisplaying_RoutesToMatchingHandlers()
    {
        var quest = new TrackingQuest();
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnDisplaying(aisling);

        quest.Calls.Should().ContainSingle().And.Contain("ran");
    }

    [Test]
    public void OnDisplayed_RoutesToMatchingHandlers()
    {
        var quest = new TrackingQuest();
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnDisplayed(aisling);

        quest.Calls.Should().ContainSingle().And.Contain("ran");
    }

    [Test]
    public void OnNext_RoutesToMatchingHandlers()
    {
        var quest = new TrackingQuest();
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnNext(aisling);

        quest.Calls.Should().ContainSingle().And.Contain("ran");
    }

    [Test]
    public void OnPrevious_RoutesToMatchingHandlers()
    {
        var quest = new TrackingQuest();
        var registry = CreateRegistry(quest);

        var dialog = MockDialog.Create("foo");
        var aisling = MockAisling.Create();
        var script = new QuestDialogScript(dialog, registry, MockServiceProvider.Create().Object);

        script.OnPrevious(aisling);

        quest.Calls.Should().ContainSingle().And.Contain("ran");
    }

    [Test]
    public void OnDisplaying_NoOpsWhenNoHandlers()
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

        script.OnDisplaying(aisling);

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

        script.OnDisplaying(aisling);

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

        script.OnDisplaying(aisling);

        halting.Calls.Should().BeEmpty("guard halted this quest's chain");
        sibling.Calls.Should().ContainSingle().And.Contain("sibling_ran");
    }
}
