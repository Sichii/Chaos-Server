#region
using Chaos.Collections;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class ActivationContextTests
{
    #region Constructor(Creature, MapEntity)
    [Test]
    public void Constructor_WithMapEntity_ShouldCaptureSnapshots()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.Source
               .Should()
               .BeSameAs(source);

        context.SnapshotSourcePoint
               .Should()
               .Be(Point.From(source));

        context.SnapshotTargetPoint
               .Should()
               .Be(Point.From(target));

        context.SnapshotSourceMap
               .Should()
               .BeSameAs(map);

        context.SnapshotTargetMap
               .Should()
               .BeSameAs(map);
    }

    [Test]
    public void Constructor_WithMapEntity_ShouldSetSourceAisling_WhenSourceIsAisling()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.SourceAisling
               .Should()
               .BeSameAs(source);
    }

    [Test]
    public void Constructor_WithMapEntity_ShouldSetTargetCreature_WhenTargetIsCreature()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.TargetCreature
               .Should()
               .BeSameAs(target);
    }

    [Test]
    public void Constructor_WithMapEntity_ShouldSetTargetAisling_WhenTargetIsAisling()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockAisling.Create(map);

        var context = new ActivationContext(source, target);

        context.TargetAisling
               .Should()
               .BeSameAs(target);
    }

    [Test]
    public void Constructor_WithMapEntity_ShouldSetTargetAislingNull_WhenTargetIsMonster()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.TargetAisling
               .Should()
               .BeNull();
    }
    #endregion

    #region Constructor(Creature, IPoint, MapInstance)
    [Test]
    public void Constructor_WithPoint_ShouldUseProvidedMap()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var targetPoint = new Point(3, 3);

        var context = new ActivationContext(source, targetPoint, map);

        context.SnapshotTargetMap
               .Should()
               .BeSameAs(map);

        context.SnapshotTargetPoint
               .Should()
               .Be(targetPoint);
    }

    [Test]
    public void Constructor_WithPoint_ShouldSetNullAislingAndCreature()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var targetPoint = new Point(3, 3);

        var context = new ActivationContext(source, targetPoint, map);

        context.TargetAisling
               .Should()
               .BeNull();

        context.TargetCreature
               .Should()
               .BeNull();
    }
    #endregion

    #region Live Properties
    [Test]
    public void SourceMap_ShouldReturnCurrentMap()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.SourceMap
               .Should()
               .BeSameAs(map);
    }

    [Test]
    public void SourcePoint_ShouldReturnCurrentSourcePosition()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.SourcePoint
               .Should()
               .Be(Point.From(source));
    }

    [Test]
    public void TargetPoint_ShouldReturnCurrentTargetPosition()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.TargetPoint
               .Should()
               .Be(Point.From(target));
    }

    [Test]
    public void TargetMap_ShouldReturnTargetEntityMap_WhenTargetIsMapEntity()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.TargetMap
               .Should()
               .BeSameAs(map);
    }

    [Test]
    public void TargetMap_ShouldReturnSnapshotMap_WhenTargetIsPoint()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var targetPoint = new Point(3, 3);

        var context = new ActivationContext(source, targetPoint, map);

        context.TargetMap
               .Should()
               .BeSameAs(map);
    }
    #endregion

    #region Direction
    [Test]
    public void Direction_ShouldReturnSourceDirection()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.Direction
               .Should()
               .Be(source.Direction);
    }

    [Test]
    public void SnapshotSourceDirection_ShouldCaptureDirectionAtCreation()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.SnapshotSourceDirection
               .Should()
               .Be(source.Direction);
    }
    #endregion

    #region TargetDirection
    [Test]
    public void TargetDirection_ShouldReturnCreatureDirection_WhenTargetIsCreature()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        context.TargetDirection
               .Should()
               .Be(target.Direction);
    }

    [Test]
    public void TargetDirection_ShouldReturnRelationalDirection_WhenTargetIsPointNotAtSameLocation()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map, position: new Point(5, 5));

        // Point above the source (y - 1 means Up)
        var targetPoint = new Point(5, 4);

        var context = new ActivationContext(source, targetPoint, map);

        context.TargetDirection
               .Should()
               .Be(Direction.Up);
    }

    [Test]
    public void TargetDirection_ShouldFallBackToSourceDirection_WhenTargetIsPointAtSameLocation()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map, position: new Point(5, 5));

        // Same point as source => DirectionalRelationTo returns Invalid => falls back to SourceDirection
        var targetPoint = new Point(5, 5);

        var context = new ActivationContext(source, targetPoint, map);

        context.TargetDirection
               .Should()
               .Be(context.SourceDirection);
    }
    #endregion

    #region Constructor — Monster Source (non-Aisling)
    [Test]
    public void Constructor_WithMapEntity_ShouldSetSourceAislingNull_WhenSourceIsMonster()
    {
        var map = MockMapInstance.Create();
        var source = MockMonster.Create(map, "Attacker");
        var target = MockAisling.Create(map);

        var context = new ActivationContext(source, target);

        context.SourceAisling
               .Should()
               .BeNull();

        context.TargetAisling
               .Should()
               .BeSameAs(target);
    }

    [Test]
    public void Constructor_WithPoint_ShouldSetSourceAislingNull_WhenSourceIsMonster()
    {
        var map = MockMapInstance.Create();
        var source = MockMonster.Create(map, "Attacker");
        var targetPoint = new Point(3, 3);

        var context = new ActivationContext(source, targetPoint, map);

        context.SourceAisling
               .Should()
               .BeNull();

        context.TargetAisling
               .Should()
               .BeNull();

        context.TargetCreature
               .Should()
               .BeNull();
    }
    #endregion

    #region SnapshotTargetDirection
    [Test]
    public void SnapshotTargetDirection_ShouldBeValidDirection_WhenNonCreatureTargetAtDifferentLocation()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map, position: new Point(5, 5));

        // GroundItem below the source (y + 1 means Down)
        var groundItem = MockGroundItem.Create(map, position: new Point(5, 6));

        var context = new ActivationContext(source, groundItem);

        // Non-creature at different position => DirectionalRelationTo returns valid direction
        context.SnapshotTargetDirection
               .Should()
               .Be(Direction.Down);
    }

    [Test]
    public void SnapshotTargetDirection_ShouldBeCreatureDirection_WhenTargetIsCreature()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map, position: new Point(3, 3));
        var target = MockMonster.Create(map);

        var context = new ActivationContext(source, target);

        // Target is a Creature => (Target as Creature)?.Direction returns target.Direction
        context.SnapshotTargetDirection
               .Should()
               .Be(target.Direction);
    }

    [Test]
    public void SnapshotTargetDirection_ShouldBeNull_WhenUsingPointConstructor()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map, position: new Point(5, 5));
        var targetPoint = new Point(3, 3);

        var context = new ActivationContext(source, targetPoint, map);

        context.SnapshotTargetDirection
               .Should()
               .BeNull();
    }

    [Test]
    public void SnapshotTargetDirection_ShouldFallBackToSourceDirection_WhenNonCreatureTargetAtSameLocation()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map, position: new Point(5, 5));

        // GroundItem is a MapEntity but not a Creature, at the same position as source
        var groundItem = MockGroundItem.Create(map, position: new Point(5, 5));

        var context = new ActivationContext(source, groundItem);

        // Non-creature at same position => (Target as Creature)?.Direction is null
        // => DirectionalRelationTo returns Invalid => falls back to SnapshotSourceDirection
        context.SnapshotTargetDirection
               .Should()
               .Be(context.SnapshotSourceDirection);
    }
    #endregion

    #region Constructor — Null Arguments
    [Test]
    public void Constructor_WithMapEntity_ShouldThrow_WhenSourceIsNull()
    {
        var map = MockMapInstance.Create();
        var target = MockMonster.Create(map);

        var act = () => new ActivationContext(null!, target);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WithMapEntity_ShouldThrow_WhenTargetIsNull()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);

        var act = () => new ActivationContext(source, null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WithPoint_ShouldThrow_WhenSourceIsNull()
    {
        var map = MockMapInstance.Create();
        var targetPoint = new Point(3, 3);

        var act = () => new ActivationContext(null!, targetPoint, map);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WithPoint_ShouldThrow_WhenTargetIsNull()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);

        var act = () => new ActivationContext(source, null!, map);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WithPoint_ShouldThrow_WhenMapIsNull()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map);
        var targetPoint = new Point(3, 3);

        var act = () => new ActivationContext(source, targetPoint, null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }
    #endregion

    #region Constructor — non-Creature MapEntity target
    [Test]
    public void Constructor_WithNonCreatureTarget_ShouldUseDirectionalRelation()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map, position: new Point(5, 5));
        var groundItem = MockGroundItem.Create(map, position: new Point(5, 6));

        var context = new ActivationContext(source, groundItem);

        context.TargetCreature
               .Should()
               .BeNull();

        context.SnapshotTargetDirection
               .Should()
               .NotBe(Direction.Invalid);
    }

    [Test]
    public void Constructor_WithNonCreatureTarget_ShouldFallbackToSourceDirection_WhenSamePosition()
    {
        var map = MockMapInstance.Create();
        var source = MockAisling.Create(map, position: new Point(5, 5));
        source.Direction = Direction.Right;

        var groundItem = MockGroundItem.Create(map, position: new Point(5, 5));

        // Same position → DirectionalRelationTo returns Invalid → fallback to source direction
        var context = new ActivationContext(source, groundItem);

        context.SnapshotTargetDirection
               .Should()
               .Be(Direction.Right);
    }
    #endregion
}