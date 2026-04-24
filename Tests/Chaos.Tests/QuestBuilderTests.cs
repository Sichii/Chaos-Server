#region
using Chaos.Scripting.Quests;
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
    public void OnDialog_RegistersOneHandlerForTemplateKey()
    {
        var quest = new FakeQuest
        {
            ConfigureAction = q => q.OnDialog("fake_initial")
        };

        quest.RunConfigure();

        quest.DialogHandlers.Should().ContainSingle();
        quest.DialogHandlers[0].TemplateKey.Should().Be("fake_initial");
        quest.DialogHandlers[0].Quest.Should().BeSameAs(quest);
    }

    [Test]
    public void OnDialog_MultipleCallsRegisterMultipleHandlers()
    {
        var quest = new FakeQuest
        {
            ConfigureAction = q =>
            {
                q.OnDialog("fake_initial");
                q.OnDialog("fake_accept");
                q.OnDialog("fake_initial"); // same template key — distinct handler
            }
        };

        quest.RunConfigure();

        quest.DialogHandlers.Should().HaveCount(3);
        quest.DialogHandlers.Select(h => h.TemplateKey).Should().ContainInOrder("fake_initial", "fake_accept", "fake_initial");
    }

    [Test]
    public void OnDialog_ThrowsForNullOrEmptyKey()
    {
        var quest = new FakeQuest
        {
            ConfigureAction = q => q.OnDialog("")
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
