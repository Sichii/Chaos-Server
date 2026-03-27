#region
using Chaos.Common.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class CompositeAndConfigurableScriptsTests
{
    [Test]
    public void Composite_GetEnumerator_ShouldYieldLeavesAndNestedLeaves()
    {
        var composite = new NestedComposite();

        var list = composite.ToList();

        list.Should()
            .HaveCount(3);

        list.Should()
            .ContainItemsAssignableTo<IDemoScript>();
    }

    [Test]
    public void Composite_GetScript_ShouldReturnFirstMatchAcrossNest()
    {
        var composite = new NestedComposite();

        composite.GetScript<LeafScript>()
                 .Should()
                 .NotBeNull();

        composite.GetScript<LeafScript2>()
                 .Should()
                 .NotBeNull();

        composite.GetScript<DemoComposite>()
                 .Should()
                 .NotBeNull();

        composite.GetScript<CompositeScriptBase<IDemoScript>>()
                 .Should()
                 .NotBeNull();
    }

    [Test]
    public void Composite_GetScripts_ShouldReturnAllMatchesAcrossNest()
    {
        var composite = new DemoComposite();
        composite.Add(new LeafScript());
        composite.Add(new LeafScript2());

        var leaves = composite.GetScripts<IDemoScript>()
                              .ToList();

        leaves.Should()
              .HaveCount(2);

        leaves.Should()
              .ContainItemsAssignableTo<IDemoScript>();
    }

    [Test]
    public void Composite_Remove_ShouldRemoveLeafOrNestedLeaf()
    {
        var composite = new NestedComposite();
        var leaf = composite.GetScript<LeafScript2>()!;

        composite.Remove(leaf);

        composite.GetScript<LeafScript2>()
                 .Should()
                 .BeNull();
    }

    [Test]
    public void ConfigurableScript_WithFactory_ShouldPopulateOrThrowIfMissing()
    {
        var vars = new Vars();
        vars.Set("IntValue", 9);
        vars.Set("Name", "abc");

        var script = new ConfigurableDemo(new DummyScripted(), _ => vars);

        script.Snapshot()
              .Should()
              .Be((9, "abc"));

        // Missing vars should throw
        Action act = () => new ConfigurableDemo(new DummyScripted(), _ => null!);

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void ConfigurableScript_WithVarsObject_ShouldPopulateNonPublicSetters()
    {
        var vars = new Vars();
        vars.Set("IntValue", 7);
        vars.Set("Name", "test");

        var script = new ConfigurableDemo(new DummyScripted(), vars);

        script.Snapshot()
              .Should()
              .Be((7, "test"));
    }

    private sealed class ConfigurableDemo : ConfigurableScriptBase<DummyScripted>
    {
        // Non-public, writable, instance properties to be populated
        private int IntValue { get; init; }
        private string? Name { get; init; }

        public ConfigurableDemo(DummyScripted subject, IScriptVars vars)
            : base(subject, vars) { }

        public ConfigurableDemo(DummyScripted subject, Func<string, IScriptVars> factory)
            : base(subject, factory) { }

        public (int, string?) Snapshot() => (IntValue, Name);
    }

    private sealed class DemoComposite : CompositeScriptBase<IDemoScript>, IDemoScript { }

    private sealed class DummyScripted : IScripted
    {
        public ISet<string> ScriptKeys { get; } = new HashSet<string>();
    }

    private interface IDemoScript : IScript { }

    private sealed class LeafScript : ScriptBase, IDemoScript { }

    private sealed class LeafScript2 : ScriptBase, IDemoScript { }

    private sealed class NestedComposite : CompositeScriptBase<IDemoScript>, IDemoScript
    {
        public NestedComposite()
        {
            Add(new LeafScript());
            var inner = new DemoComposite();
            inner.Add(new LeafScript2());
            Add(inner);
        }
    }

    private sealed class Vars : IScriptVars
    {
        private readonly Dictionary<string, object> _byKey = new();
        public bool ContainsKey(string key) => _byKey.ContainsKey(key);
        public object? Get(Type type, string name) => _byKey.TryGetValue(name, out var v) ? v : null;
        public T? Get<T>(string name) => _byKey.TryGetValue(name, out var v) ? (T)v : default;
        public T GetRequired<T>(string key) => _byKey.TryGetValue(key, out var v) ? (T)v : throw new KeyNotFoundException(key);
        public void Set<T>(string name, T value) => _byKey[name] = value!;
    }
}