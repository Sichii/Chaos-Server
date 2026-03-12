#region
using Chaos.Collections;
using Chaos.Geometry;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class MonsterTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region ResetAggro (no args)
    [Test]
    public void ResetAggro_ShouldClearTarget()
    {
        var monster = MockMonster.Create(Map);
        var target = MockAisling.Create(Map, "Target");

        monster.Target = target;

        monster.ResetAggro();

        monster.Target
               .Should()
               .BeNull();
    }

    [Test]
    public void ResetAggro_ShouldClearAggroList()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Attacker");

        monster.AggroList.AddAggro(aisling, 100);

        monster.AggroList
               .Should()
               .NotBeEmpty();

        monster.ResetAggro();

        monster.AggroList
               .Should()
               .BeEmpty();
    }

    [Test]
    public void ResetAggro_ShouldResetApproachTimes()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Approacher");

        // Add an approach time entry
        monster.ApproachTime[aisling] = DateTime.UtcNow.AddMinutes(-5);
        var oldTime = monster.ApproachTime[aisling];

        monster.ResetAggro();

        monster.ApproachTime[aisling]
               .Should()
               .BeAfter(oldTime);
    }
    #endregion

    #region ResetAggro (Creature)
    [Test]
    public void ResetAggro_Creature_ShouldClearTarget_WhenTargetMatchesCreature()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Target");

        monster.Target = aisling;

        monster.ResetAggro(aisling);

        monster.Target
               .Should()
               .BeNull();
    }

    [Test]
    public void ResetAggro_Creature_ShouldNotClearTarget_WhenTargetDoesNotMatch()
    {
        var monster = MockMonster.Create(Map);
        var target = MockAisling.Create(Map, "Target");
        var other = MockAisling.Create(Map, "Other");

        monster.Target = target;

        monster.ResetAggro(other);

        monster.Target
               .Should()
               .Be(target);
    }

    [Test]
    public void ResetAggro_Creature_ShouldClearAggroForCreature()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Target");

        monster.AggroList.AddAggro(aisling, 100);

        monster.ResetAggro(aisling);

        // AggroList.Clear(creature) clears aggro for that specific creature
        monster.AggroList
               .GetAggro(aisling)
               .Should()
               .Be(0);
    }

    [Test]
    public void ResetAggro_Creature_ShouldResetApproachTime_WhenExists()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Target");

        monster.ApproachTime[aisling] = DateTime.UtcNow.AddMinutes(-5);
        var oldTime = monster.ApproachTime[aisling];

        monster.ResetAggro(aisling);

        monster.ApproachTime[aisling]
               .Should()
               .BeAfter(oldTime);
    }

    [Test]
    public void ResetAggro_Creature_ShouldNotThrow_WhenNoApproachTime()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Target");

        // No approach time entry exists
        var act = () => monster.ResetAggro(aisling);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void ResetAggro_Creature_ShouldHandleNullTarget()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Target");

        // Target is null initially
        monster.Target
               .Should()
               .BeNull();

        var act = () => monster.ResetAggro(aisling);

        act.Should()
           .NotThrow();
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldNotThrow_WithDefaultMovementSpeed()
    {
        var monster = MockMonster.Create(Map);

        // MovementSpeedPct defaults to 0
        var act = () => monster.Update(TimeSpan.FromMilliseconds(100));

        act.Should()
           .NotThrow();
    }

    [Test]
    public void Update_ShouldApplyPositiveMovementSpeedModifier()
    {
        var monster = MockMonster.Create(Map);
        monster.MovementSpeedPct = 50;

        // Positive modifier: movementDelta /= 1 + (50/100) = 1.5
        // Should not throw
        var act = () => monster.Update(TimeSpan.FromMilliseconds(100));

        act.Should()
           .NotThrow();
    }

    [Test]
    public void Update_ShouldApplyNegativeMovementSpeedModifier()
    {
        var monster = MockMonster.Create(Map);
        monster.MovementSpeedPct = -50;

        // Negative modifier: movementDelta *= Math.Abs(-0.5 - 1) = 1.5
        // Should not throw
        var act = () => monster.Update(TimeSpan.FromMilliseconds(100));

        act.Should()
           .NotThrow();
    }
    #endregion

    #region BlackList
    [Test]
    public void BlackList_ShouldBeInitiallyEmpty()
    {
        var monster = MockMonster.Create(Map);

        monster.BlackList
               .Should()
               .BeEmpty();
    }

    [Test]
    public void BlackList_ShouldAcceptPoints()
    {
        var monster = MockMonster.Create(Map);

        monster.BlackList.Add(new Point(1, 1));
        monster.BlackList.Add(new Point(2, 3));

        monster.BlackList
               .Should()
               .HaveCount(2);
    }
    #endregion

    #region Update — Skills and Spells
    [Test]
    public void Update_ShouldUpdateSkills_WhenSkillsArePresent()
    {
        var monster = MockMonster.Create(Map);

        var skill = MockSkill.Create(
            "MonsterSlash",
            setup: s =>
            {
                s.Cooldown = TimeSpan.FromSeconds(10);
                s.Elapsed = TimeSpan.Zero;
            });

        monster.Skills.Add(skill);

        var delta = TimeSpan.FromSeconds(1);
        monster.Update(delta);

        // Skill.Update calls base.Update which increments Elapsed, and Script.Update
        skill.Elapsed
             .Should()
             .NotBe(TimeSpan.Zero);

        Mock.Get(skill.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);
    }

    [Test]
    public void Update_ShouldUpdateSpells_WhenSpellsArePresent()
    {
        var monster = MockMonster.Create(Map);

        var spell = MockSpell.Create(
            "MonsterFire",
            setup: s =>
            {
                s.Cooldown = TimeSpan.FromSeconds(10);
                s.Elapsed = TimeSpan.Zero;
            });

        monster.Spells.Add(spell);

        var delta = TimeSpan.FromSeconds(1);
        monster.Update(delta);

        // Spell.Update calls base.Update which increments Elapsed, and Script.Update
        spell.Elapsed
             .Should()
             .NotBe(TimeSpan.Zero);

        Mock.Get(spell.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);
    }

    [Test]
    public void Update_ShouldUpdateMultipleSkillsAndSpells()
    {
        var monster = MockMonster.Create(Map);

        var skill1 = MockSkill.Create(
            "Slash1",
            setup: s =>
            {
                s.Cooldown = TimeSpan.FromSeconds(10);
                s.Elapsed = TimeSpan.Zero;
            });

        var skill2 = MockSkill.Create(
            "Slash2",
            setup: s =>
            {
                s.Cooldown = TimeSpan.FromSeconds(10);
                s.Elapsed = TimeSpan.Zero;
            });

        var spell1 = MockSpell.Create(
            "Fire1",
            setup: s =>
            {
                s.Cooldown = TimeSpan.FromSeconds(10);
                s.Elapsed = TimeSpan.Zero;
            });

        var spell2 = MockSpell.Create(
            "Fire2",
            setup: s =>
            {
                s.Cooldown = TimeSpan.FromSeconds(10);
                s.Elapsed = TimeSpan.Zero;
            });

        monster.Skills.Add(skill1);
        monster.Skills.Add(skill2);
        monster.Spells.Add(spell1);
        monster.Spells.Add(spell2);

        var delta = TimeSpan.FromSeconds(1);
        monster.Update(delta);

        Mock.Get(skill1.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);

        Mock.Get(skill2.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);

        Mock.Get(spell1.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);

        Mock.Get(spell2.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);
    }
    #endregion

    #region Properties
    [Test]
    public void MovementSpeedPct_ShouldBeSettable()
    {
        var monster = MockMonster.Create(Map);

        monster.MovementSpeedPct = 100;

        monster.MovementSpeedPct
               .Should()
               .Be(100);
    }

    [Test]
    public void Target_ShouldBeSettable()
    {
        var monster = MockMonster.Create(Map);
        var target = MockAisling.Create(Map, "Target");

        monster.Target = target;

        monster.Target
               .Should()
               .Be(target);
    }

    [Test]
    public void Experience_ShouldComeFromTemplate()
    {
        var monster = MockMonster.Create(Map);

        // Template.ExpReward defaults to 100 in MockMonster
        monster.Experience
               .Should()
               .Be(100);
    }

    [Test]
    public void AggroRange_ShouldComeFromTemplate()
    {
        var monster = MockMonster.Create(Map);

        // Template.AggroRange defaults to 5 in MockMonster
        monster.AggroRange
               .Should()
               .Be(5);
    }
    #endregion
}