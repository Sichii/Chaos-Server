#region
using Chaos.Geometry;
using Chaos.Models.Data;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class SpellContextTests
{
    #region Inherited Properties
    [Test]
    public void ShouldInheritSourceFromActivationContext()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockAisling.Create(map);

        var context = new SpellContext(source, target);

        context.Source
               .Should()
               .BeSameAs(source);

        context.SourceAisling
               .Should()
               .BeSameAs(source);

        context.TargetAisling
               .Should()
               .BeSameAs(target);

        context.SnapshotSourceMap
               .Should()
               .BeSameAs(map);

        context.SnapshotTargetMap
               .Should()
               .BeSameAs(map);

        context.SnapshotSourcePoint
               .Should()
               .Be(Point.From(source));

        context.SnapshotTargetPoint
               .Should()
               .Be(Point.From(target));
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_ShouldSetPromptResponse()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockAisling.Create(map);

        var context = new SpellContext(source, target, "hello");

        context.PromptResponse
               .Should()
               .Be("hello");
    }

    [Test]
    public void Constructor_ShouldDefaultPromptResponseToNull()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockAisling.Create(map);

        var context = new SpellContext(source, target);

        context.PromptResponse
               .Should()
               .BeNull();
    }
    #endregion
}