#region
using Chaos.Services.Quests;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Utilities.QuestHelper;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class QuestRegistryTests
{
    private enum FooStage { None, Started, Done }

    private sealed class FooQuest : Quest<FooStage>
    {
        public override string Key => "registry.foo";

        protected override void Configure(QuestBuilder<FooStage> q)
        {
            q.OnDialog("foo_initial");
            q.OnDialog("foo_accept");
        }
    }

    private sealed class BarQuest : Quest<FooStage>
    {
        public override string Key => "registry.bar";

        protected override void Configure(QuestBuilder<FooStage> q) => q.OnDialog("foo_initial");
    }

    private static QuestRegistry CreateRegistry() => new(MockServiceProvider.Create().Object);

    [Test]
    public void Register_StoresQuestByTypeAndKey()
    {
        var registry = CreateRegistry();
        var quest = new FooQuest();
        quest.RunConfigure();

        registry.Register(quest);

        registry.Get<FooQuest>().Should().BeSameAs(quest);
        registry.Get("registry.foo").Should().BeSameAs(quest);
    }

    [Test]
    public void Register_ThrowsForDuplicateKey()
    {
        var registry = CreateRegistry();
        var first = new FooQuest();
        first.RunConfigure();
        registry.Register(first);

        var duplicate = new FooQuest();
        duplicate.RunConfigure();

        Action act = () => registry.Register(duplicate);
        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void GetDialogHandlers_ReturnsAllRegisteredForTemplateKey()
    {
        var registry = CreateRegistry();
        var foo = new FooQuest();
        foo.RunConfigure();
        var bar = new BarQuest();
        bar.RunConfigure();

        registry.Register(foo);
        registry.Register(bar);

        var handlers = registry.GetDialogHandlers("foo_initial");

        handlers.Should().HaveCount(2);
        handlers.Select(h => h.Quest).Should().Contain([foo, bar]);
    }

    [Test]
    public void GetDialogHandlers_ReturnsEmptyForUnknownKey()
    {
        var registry = CreateRegistry();
        var foo = new FooQuest();
        foo.RunConfigure();
        registry.Register(foo);

        var handlers = registry.GetDialogHandlers("nonexistent_key");

        handlers.Should().BeEmpty();
    }

    [Test]
    public void Get_ReturnsNullForUnknownKey()
    {
        var registry = CreateRegistry();

        registry.Get("missing").Should().BeNull();
    }

    [Test]
    public void GetByType_ThrowsForUnknownType()
    {
        var registry = CreateRegistry();

        Action act = () => registry.Get<FooQuest>();

        act.Should().Throw<KeyNotFoundException>();
    }

    [Test]
    public void AutoDiscover_RegistersAllQuestSubclassesInLoadedAssemblies()
    {
        var registry = CreateRegistry();

        registry.AutoDiscover();

        // Both test quests in this assembly should be discovered & registered.
        registry.Get("registry.foo").Should().NotBeNull().And.BeOfType<FooQuest>();
        registry.Get("registry.bar").Should().NotBeNull().And.BeOfType<BarQuest>();

        // Discovery must have invoked Configure (handlers populated).
        registry.GetDialogHandlers("foo_initial").Should().HaveCount(2);
        registry.GetDialogHandlers("foo_accept").Should().ContainSingle();
    }
}
