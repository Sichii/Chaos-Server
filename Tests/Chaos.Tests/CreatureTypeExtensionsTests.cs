#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class CreatureTypeExtensionsTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region Creature.WillCollideWith(Creature)
    [Test]
    public void Creature_WillCollideWith_ShouldDelegateToCreatureType()
    {
        var aisling = MockAisling.Create(Map, "Player");
        var monster = MockMonster.Create(Map, "Monster");

        // Aisling (CreatureType.Aisling) vs Monster (CreatureType.Normal) => true
        aisling.WillCollideWith(monster)
               .Should()
               .BeTrue();
    }
    #endregion

    #region WillCollideWith(CreatureType)
    [Test]
    public void Normal_ShouldCollideWithAll()
    {
        CreatureType.Normal
                    .WillCollideWith(CreatureType.Normal)
                    .Should()
                    .BeTrue();

        CreatureType.Normal
                    .WillCollideWith(CreatureType.Aisling)
                    .Should()
                    .BeTrue();

        CreatureType.Normal
                    .WillCollideWith(CreatureType.WalkThrough)
                    .Should()
                    .BeTrue();

        CreatureType.Normal
                    .WillCollideWith(CreatureType.Merchant)
                    .Should()
                    .BeTrue();

        CreatureType.Normal
                    .WillCollideWith(CreatureType.WhiteSquare)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void WalkThrough_ShouldNotCollideWithAisling()
        => CreatureType.WalkThrough
                       .WillCollideWith(CreatureType.Aisling)
                       .Should()
                       .BeFalse();

    [Test]
    public void WalkThrough_ShouldCollideWithNonAisling()
    {
        CreatureType.WalkThrough
                    .WillCollideWith(CreatureType.Normal)
                    .Should()
                    .BeTrue();

        CreatureType.WalkThrough
                    .WillCollideWith(CreatureType.Merchant)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void Merchant_ShouldCollideWithAll()
    {
        CreatureType.Merchant
                    .WillCollideWith(CreatureType.Aisling)
                    .Should()
                    .BeTrue();

        CreatureType.Merchant
                    .WillCollideWith(CreatureType.Normal)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void WhiteSquare_ShouldCollideWithAll()
    {
        CreatureType.WhiteSquare
                    .WillCollideWith(CreatureType.Aisling)
                    .Should()
                    .BeTrue();

        CreatureType.WhiteSquare
                    .WillCollideWith(CreatureType.WalkThrough)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void Aisling_ShouldNotCollideWithWalkThrough()
        => CreatureType.Aisling
                       .WillCollideWith(CreatureType.WalkThrough)
                       .Should()
                       .BeFalse();

    [Test]
    public void Aisling_ShouldCollideWithNonWalkThrough()
    {
        CreatureType.Aisling
                    .WillCollideWith(CreatureType.Normal)
                    .Should()
                    .BeTrue();

        CreatureType.Aisling
                    .WillCollideWith(CreatureType.Merchant)
                    .Should()
                    .BeTrue();

        CreatureType.Aisling
                    .WillCollideWith(CreatureType.Aisling)
                    .Should()
                    .BeTrue();
    }
    #endregion

    #region WillCollideWith(Creature) - GmHidden
    [Test]
    public void WillCollideWith_GmHiddenAdmin_ShouldNotCollide()
    {
        var admin = MockAisling.Create(
            Map,
            "Admin",
            setup: a =>
            {
                a.IsAdmin = true;
                a.SetVisibility(VisibilityType.GmHidden);
            });

        CreatureType.Normal
                    .WillCollideWith(admin)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void WillCollideWith_NonAdminGmHidden_ShouldCollide()
    {
        var aisling = MockAisling.Create(
            Map,
            "Player",
            setup: a =>
            {
                a.IsAdmin = false;
                a.SetVisibility(VisibilityType.GmHidden);
            });

        CreatureType.Normal
                    .WillCollideWith(aisling)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void WillCollideWith_AdminNotGmHidden_ShouldCollide()
    {
        var admin = MockAisling.Create(
            Map,
            "Admin",
            setup: a =>
            {
                a.IsAdmin = true;

                // Default visibility is Normal
            });

        CreatureType.Normal
                    .WillCollideWith(admin)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void WillCollideWith_Monster_ShouldDelegateToTypeCheck()
    {
        var monster = MockMonster.Create(Map, "Monster");

        // Monster.Type is CreatureType.Normal
        // WalkThrough vs Normal => true
        CreatureType.WalkThrough
                    .WillCollideWith(monster)
                    .Should()
                    .BeTrue();
    }
    #endregion
}