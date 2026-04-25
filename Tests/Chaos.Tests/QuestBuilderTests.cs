#region
using Chaos.Utilities.QuestHelper;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class QuestBuilderTests
{
    private enum FakeStage { None, Started, Done }

    private sealed class FakeQuest : Quest<FakeStage>
    {
        public override string Key => "fake";
        public Action<QuestBuilder<FakeStage>> ConfigureAction { get; init; } = _ => { };
        protected override void Configure(QuestBuilder<FakeStage> q) => ConfigureAction(q);
    }

    [Test]
    public void OnNext_RegistersOneHandlerForTemplateKey()
    {
        var quest = new FakeQuest
        {
            ConfigureAction = q => q.OnNext("fake_initial")
        };

        quest.RunConfigure();

        quest.DialogHandlers.Should().ContainSingle();
        quest.DialogHandlers[0].TemplateKey.Should().Be("fake_initial");
        quest.DialogHandlers[0].Phase.Should().Be(DialogPhase.Next);
        quest.DialogHandlers[0].Quest.Should().BeSameAs(quest);
    }

    //formatter:off
    [Test]
    [Arguments(DialogPhase.Displaying)]
    [Arguments(DialogPhase.Displayed)]
    [Arguments(DialogPhase.Next)]
    [Arguments(DialogPhase.Previous)]
    //formatter:on
    public void EachPhaseMethodTagsHandlerWithMatchingPhase(DialogPhase expected)
    {
        var quest = new FakeQuest
        {
            ConfigureAction = q =>
            {
                switch (expected)
                {
                    case DialogPhase.Displaying: q.OnDisplaying("fake_initial"); break;
                    case DialogPhase.Displayed: q.OnDisplayed("fake_initial"); break;
                    case DialogPhase.Next: q.OnNext("fake_initial"); break;
                    case DialogPhase.Previous: q.OnPrevious("fake_initial"); break;
                }
            }
        };

        quest.RunConfigure();

        quest.DialogHandlers.Should().ContainSingle();
        quest.DialogHandlers[0].Phase.Should().Be(expected);
    }

    [Test]
    public void MultipleCallsRegisterMultipleHandlers()
    {
        var quest = new FakeQuest
        {
            ConfigureAction = q =>
            {
                q.OnNext("fake_initial");
                q.OnNext("fake_accept");
                q.OnNext("fake_initial"); // same template key — distinct handler
            }
        };

        quest.RunConfigure();

        quest.DialogHandlers.Should().HaveCount(3);
        quest.DialogHandlers.Select(h => h.TemplateKey).Should().ContainInOrder("fake_initial", "fake_accept", "fake_initial");
    }

    [Test]
    public void SameTemplateKeyAcrossDifferentPhasesRegistersDistinctHandlers()
    {
        var quest = new FakeQuest
        {
            ConfigureAction = q =>
            {
                q.OnDisplaying("fake_initial");
                q.OnNext("fake_initial");
            }
        };

        quest.RunConfigure();

        quest.DialogHandlers.Should().HaveCount(2);
        quest.DialogHandlers[0].Phase.Should().Be(DialogPhase.Displaying);
        quest.DialogHandlers[1].Phase.Should().Be(DialogPhase.Next);
    }

    //formatter:off
    [Test]
    [Arguments(DialogPhase.Displaying)]
    [Arguments(DialogPhase.Displayed)]
    [Arguments(DialogPhase.Next)]
    [Arguments(DialogPhase.Previous)]
    //formatter:on
    public void PhaseMethodThrowsForNullOrEmptyKey(DialogPhase phase)
    {
        var quest = new FakeQuest
        {
            ConfigureAction = q =>
            {
                switch (phase)
                {
                    case DialogPhase.Displaying: q.OnDisplaying(""); break;
                    case DialogPhase.Displayed: q.OnDisplayed(""); break;
                    case DialogPhase.Next: q.OnNext(""); break;
                    case DialogPhase.Previous: q.OnPrevious(""); break;
                }
            }
        };

        Action act = quest.RunConfigure;
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Quest_HasEmptyDialogHandlersBeforeConfigure()
    {
        var quest = new FakeQuest();
        quest.DialogHandlers.Should().BeEmpty();
    }
}
