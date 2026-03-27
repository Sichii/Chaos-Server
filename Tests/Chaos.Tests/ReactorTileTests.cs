#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Tests;

public sealed class ReactorTileTests
{
    private static ReactorTile CreateReactorTile(
        MapInstance? mapInstance = null,
        IPoint? point = null,
        bool shouldBlockPathfinding = false,
        Creature? owner = null,
        IScript? sourceScript = null)
    {
        mapInstance ??= MockMapInstance.Create();
        point ??= new Point(3, 3);

        return new ReactorTile(
            mapInstance,
            point,
            shouldBlockPathfinding,
            MockScriptProvider.Instance.Object,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "TestReactor"
            },
            new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
            owner,
            sourceScript);
    }

    private static TemplatedReactorTile CreateTemplatedReactorTile(
        MapInstance? mapInstance = null,
        IPoint? point = null,
        bool shouldBlockPathfinding = true,
        Creature? owner = null,
        IScript? sourceScript = null)
    {
        mapInstance ??= MockMapInstance.Create();
        point ??= new Point(3, 3);

        var template = new ReactorTileTemplate
        {
            TemplateKey = "testReactor",
            ShouldBlockPathfinding = shouldBlockPathfinding,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "TestReactor"
            },
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        return new TemplatedReactorTile(
            template,
            mapInstance,
            point,
            MockScriptProvider.Instance.Object,
            null,
            owner,
            sourceScript);
    }

    #region SourceScript
    [Test]
    public void SourceScript_CanBeSetAfterConstruction()
    {
        var reactor = CreateReactorTile();
        var newScript = new Mock<IScript>().Object;

        reactor.SourceScript = newScript;

        reactor.SourceScript
               .Should()
               .BeSameAs(newScript);
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_SetsProperties()
    {
        var map = MockMapInstance.Create();
        var point = new Point(5, 5);
        var owner = MockMonster.Create(map);
        var sourceScript = new Mock<IScript>().Object;

        var reactor = CreateReactorTile(
            map,
            point,
            true,
            owner,
            sourceScript);

        reactor.ShouldBlockPathfinding
               .Should()
               .BeTrue();

        reactor.Owner
               .Should()
               .BeSameAs(owner);

        reactor.SourceScript
               .Should()
               .BeSameAs(sourceScript);

        reactor.ScriptKeys
               .Should()
               .Contain("TestReactor");

        reactor.Script
               .Should()
               .NotBeNull();
    }

    [Test]
    public void Constructor_NullOwner_SetsOwnerNull()
    {
        var reactor = CreateReactorTile();

        reactor.Owner
               .Should()
               .BeNull();
    }

    [Test]
    public void Constructor_NullSourceScript_SetsSourceScriptNull()
    {
        var reactor = CreateReactorTile();

        reactor.SourceScript
               .Should()
               .BeNull();
    }

    [Test]
    public void Constructor_ShouldBlockPathfinding_False()
    {
        var reactor = CreateReactorTile(shouldBlockPathfinding: false);

        reactor.ShouldBlockPathfinding
               .Should()
               .BeFalse();
    }
    #endregion

    #region TemplatedReactorTile
    [Test]
    public void TemplatedReactorTile_Constructor_SetsTemplate()
    {
        var templated = CreateTemplatedReactorTile();

        templated.Template
                 .Should()
                 .NotBeNull();

        templated.Template
                 .TemplateKey
                 .Should()
                 .Be("testReactor");
    }

    [Test]
    public void TemplatedReactorTile_Constructor_PassesShouldBlockPathfinding()
    {
        var templated = CreateTemplatedReactorTile(shouldBlockPathfinding: true);

        templated.ShouldBlockPathfinding
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void TemplatedReactorTile_Constructor_WithExtraScriptKeys_MergesKeys()
    {
        var map = MockMapInstance.Create();

        var template = new ReactorTileTemplate
        {
            TemplateKey = "testReactor",
            ShouldBlockPathfinding = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Base"
            },
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var templated = new TemplatedReactorTile(
            template,
            map,
            new Point(1, 1),
            MockScriptProvider.Instance.Object,
            new List<string>
            {
                "Extra"
            },
            null);

        templated.ScriptKeys
                 .Should()
                 .Contain("Base");

        templated.ScriptKeys
                 .Should()
                 .Contain("Extra");
    }

    [Test]
    public void TemplatedReactorTile_Constructor_WithOwnerAndSourceScript()
    {
        var map = MockMapInstance.Create();
        var owner = MockMonster.Create(map);
        var sourceScript = new Mock<IScript>().Object;

        var templated = CreateTemplatedReactorTile(map, owner: owner, sourceScript: sourceScript);

        templated.Owner
                 .Should()
                 .BeSameAs(owner);

        templated.SourceScript
                 .Should()
                 .BeSameAs(sourceScript);
    }
    #endregion

    #region Script Delegation
    [Test]
    public void Update_DelegatesToScript()
    {
        var reactor = CreateReactorTile();
        var scriptMock = Mock.Get(reactor.Script);
        var elapsed = TimeSpan.FromSeconds(1);

        reactor.Update(elapsed);

        scriptMock.Verify(s => s.Update(elapsed), Times.Once);
    }

    [Test]
    public void OnClicked_DelegatesToScript()
    {
        var map = MockMapInstance.Create();
        var reactor = CreateReactorTile(map);
        var scriptMock = Mock.Get(reactor.Script);
        var aisling = MockAisling.Create(map);

        reactor.OnClicked(aisling);

        scriptMock.Verify(s => s.OnClicked(aisling), Times.Once);
    }

    [Test]
    public void OnGoldDroppedOn_DelegatesToScript()
    {
        var map = MockMapInstance.Create();
        var reactor = CreateReactorTile(map);
        var scriptMock = Mock.Get(reactor.Script);
        var monster = MockMonster.Create(map);
        var money = new Money(100, map, new Point(3, 3));

        reactor.OnGoldDroppedOn(monster, money);

        scriptMock.Verify(s => s.OnGoldDroppedOn(monster, money), Times.Once);
    }

    [Test]
    public void OnGoldPickedUpFrom_DelegatesToScript()
    {
        var map = MockMapInstance.Create();
        var reactor = CreateReactorTile(map);
        var scriptMock = Mock.Get(reactor.Script);
        var aisling = MockAisling.Create(map);
        var money = new Money(100, map, new Point(3, 3));

        reactor.OnGoldPickedUpFrom(aisling, money);

        scriptMock.Verify(s => s.OnGoldPickedUpFrom(aisling, money), Times.Once);
    }

    [Test]
    public void OnItemDroppedOn_DelegatesToScript()
    {
        var map = MockMapInstance.Create();
        var reactor = CreateReactorTile(map);
        var scriptMock = Mock.Get(reactor.Script);
        var monster = MockMonster.Create(map);
        var groundItem = MockGroundItem.Create(map);

        reactor.OnItemDroppedOn(monster, groundItem);

        scriptMock.Verify(s => s.OnItemDroppedOn(monster, groundItem), Times.Once);
    }

    [Test]
    public void OnItemPickedUpFrom_DelegatesToScript()
    {
        var map = MockMapInstance.Create();
        var reactor = CreateReactorTile(map);
        var scriptMock = Mock.Get(reactor.Script);
        var aisling = MockAisling.Create(map);
        var groundItem = MockGroundItem.Create(map);

        reactor.OnItemPickedUpFrom(aisling, groundItem, 5);

        scriptMock.Verify(s => s.OnItemPickedUpFrom(aisling, groundItem, 5), Times.Once);
    }

    [Test]
    public void OnWalkedOn_DelegatesToScript()
    {
        var map = MockMapInstance.Create();
        var reactor = CreateReactorTile(map);
        var scriptMock = Mock.Get(reactor.Script);
        var monster = MockMonster.Create(map);

        reactor.OnWalkedOn(monster);

        scriptMock.Verify(s => s.OnWalkedOn(monster), Times.Once);
    }
    #endregion
}