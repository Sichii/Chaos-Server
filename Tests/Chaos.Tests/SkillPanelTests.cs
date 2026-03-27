#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.Templates;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

/// <summary>
///     Tests for <see cref="Skill" /> constructor branches, BeginCooldown override, and Use method
/// </summary>
public sealed class SkillPanelTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    private static SkillTemplate CreateSkillTemplate(
        bool levelsUp = false,
        bool isAssail = false,
        byte maxLevel = 100,
        TimeSpan? cooldown = null)
        => new()
        {
            Name = "TestSkill",
            TemplateKey = "testskill",
            PanelSprite = 1,
            MaxLevel = maxLevel,
            LevelsUp = levelsUp,
            IsAssail = isAssail,
            LearningRequirements = null,
            Level = 1,
            AbilityLevel = 0,
            Class = null,
            AdvClass = null,
            Cooldown = cooldown,
            Description = null,
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

    #region Update
    [Test]
    public void Update_ShouldCallBaseUpdateAndScriptUpdate()
    {
        var skill = MockSkill.Create();
        skill.Cooldown = TimeSpan.FromSeconds(5);
        skill.Elapsed = TimeSpan.Zero;

        var delta = TimeSpan.FromSeconds(1);

        skill.Update(delta);

        // Base.Update increments Elapsed
        skill.Elapsed
             .Should()
             .Be(TimeSpan.FromSeconds(1));

        // Script.Update should have been called (shared mock, so verify at-least-once)
        Mock.Get(skill.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_ShouldSetLevelToMaxLevel_WhenLevelsUpIsFalse()
    {
        var template = CreateSkillTemplate();
        var skill = new Skill(template, MockScriptProvider.Instance.Object);

        skill.Level
             .Should()
             .Be(100);
    }

    [Test]
    public void Constructor_ShouldLeaveLevelAtZero_WhenLevelsUpIsTrue()
    {
        var template = CreateSkillTemplate(true, maxLevel: 100);
        var skill = new Skill(template, MockScriptProvider.Instance.Object);

        skill.Level
             .Should()
             .Be(0);
    }

    [Test]
    public void Constructor_ShouldSetCooldownToZero_WhenIsAssailAndNoCooldown()
    {
        var template = CreateSkillTemplate(isAssail: true, cooldown: null);
        var skill = new Skill(template, MockScriptProvider.Instance.Object);

        skill.Cooldown
             .Should()
             .Be(TimeSpan.Zero);
    }

    [Test]
    public void Constructor_ShouldKeepTemplateCooldown_WhenIsAssailAndCooldownAlreadySet()
    {
        var cooldown = TimeSpan.FromSeconds(2);
        var template = CreateSkillTemplate(isAssail: true, cooldown: cooldown);
        var skill = new Skill(template, MockScriptProvider.Instance.Object);

        skill.Cooldown
             .Should()
             .Be(cooldown);
    }

    [Test]
    public void Constructor_ShouldNotSetCooldownToZero_WhenNotAssailAndNoCooldown()
    {
        var template = CreateSkillTemplate(isAssail: false, cooldown: null);
        var skill = new Skill(template, MockScriptProvider.Instance.Object);

        skill.Cooldown
             .Should()
             .BeNull();
    }

    [Test]
    public void Constructor_ShouldAddExtraScriptKeys()
    {
        var template = CreateSkillTemplate();

        var extraKeys = new List<string>
        {
            "BonusScript",
            "DebuffScript"
        };
        var skill = new Skill(template, MockScriptProvider.Instance.Object, extraKeys);

        skill.ScriptKeys
             .Should()
             .Contain("BonusScript")
             .And
             .Contain("DebuffScript");
    }
    #endregion

    #region BeginCooldown
    [Test]
    public void BeginCooldown_ShouldSetElapsedAndReturn_WhenIsAssail()
    {
        var skill = MockSkill.Create(
            templateSetup: t => t with
            {
                IsAssail = true
            });
        var aisling = MockAisling.Create(Map);

        skill.BeginCooldown(aisling);

        skill.Elapsed
             .Should()
             .Be(TimeSpan.Zero);

        // Assails should NOT send cooldown to client
        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(It.IsAny<PanelEntityBase>()), Times.Never);
    }

    [Test]
    public void BeginCooldown_ShouldNotOverrideElapsed_WhenIsAssailAndElapsedAlreadySet()
    {
        var skill = MockSkill.Create(
            templateSetup: t => t with
            {
                IsAssail = true
            });
        skill.Elapsed = TimeSpan.FromSeconds(2);

        var aisling = MockAisling.Create(Map);

        skill.BeginCooldown(aisling);

        skill.Elapsed
             .Should()
             .Be(TimeSpan.FromSeconds(2));
    }

    [Test]
    public void BeginCooldown_ShouldCallBase_WhenNotAssail()
    {
        var template = CreateSkillTemplate(isAssail: false, cooldown: TimeSpan.FromSeconds(5));
        var skill = new Skill(template, MockScriptProvider.Instance.Object);
        var aisling = MockAisling.Create(Map);

        skill.BeginCooldown(aisling);

        skill.Elapsed
             .Should()
             .Be(TimeSpan.Zero);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(skill), Times.Once);
    }
    #endregion

    #region Use
    [Test]
    public void Use_ShouldCallScriptOnUseAndBeginCooldown()
    {
        var template = CreateSkillTemplate(isAssail: false, cooldown: TimeSpan.FromSeconds(5));
        var skill = new Skill(template, MockScriptProvider.Instance.Object);
        var aisling = MockAisling.Create(Map);
        var monster = MockMonster.Create(Map);
        var context = new ActivationContext(aisling, monster);

        skill.Use(context);

        Mock.Get(skill.Script)
            .Verify(s => s.OnUse(context), Times.AtLeastOnce);

        skill.Elapsed
             .Should()
             .Be(TimeSpan.Zero);
    }

    [Test]
    public void Use_ShouldRecalculateCooldown_WhenIsAssail()
    {
        var template = CreateSkillTemplate(isAssail: true, cooldown: null);
        var skill = new Skill(template, MockScriptProvider.Instance.Object);

        // Use a monster as source since its AssailIntervalMs comes from template (1000ms)
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map);
        var context = new ActivationContext(monster, aisling);

        skill.Use(context);

        // The cooldown should have been recalculated from CalculateEffectiveAssailInterval
        skill.Cooldown
             .Should()
             .NotBeNull();

        skill.Cooldown!.Value
             .TotalMilliseconds
             .Should()
             .BeGreaterThan(0);
    }

    [Test]
    public void Use_ShouldNotRecalculateCooldown_WhenNotAssail()
    {
        var cooldown = TimeSpan.FromSeconds(5);
        var template = CreateSkillTemplate(isAssail: false, cooldown: cooldown);
        var skill = new Skill(template, MockScriptProvider.Instance.Object);
        var aisling = MockAisling.Create(Map);
        var monster = MockMonster.Create(Map);
        var context = new ActivationContext(aisling, monster);

        skill.Use(context);

        skill.Cooldown
             .Should()
             .Be(cooldown);
    }
    #endregion
}