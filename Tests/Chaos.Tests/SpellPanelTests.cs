#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
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
///     Tests for <see cref="Spell" /> constructor branches, Update, and Use method
/// </summary>
public sealed class SpellPanelTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    private static SpellTemplate CreateSpellTemplate(
        bool levelsUp = false,
        byte maxLevel = 100,
        byte castLines = 0,
        TimeSpan? cooldown = null)
        => new()
        {
            Name = "TestSpell",
            TemplateKey = "testspell",
            PanelSprite = 1,
            MaxLevel = maxLevel,
            LevelsUp = levelsUp,
            CastLines = castLines,
            LearningRequirements = null,
            Prompt = null,
            SpellType = SpellType.None,
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

    #region PanelDisplayName
    [Test]
    public void PanelDisplayName_ShouldFormatCorrectly()
    {
        var template = CreateSpellTemplate(maxLevel: 50);

        var spell = new Spell(template, MockScriptProvider.Instance.Object)
        {
            Level = 25
        };

        spell.PanelDisplayName
             .Should()
             .Be("TestSpell (Lev:25/50)");
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_ShouldSetLevelToMaxLevel_WhenLevelsUpIsFalse()
    {
        var template = CreateSpellTemplate();
        var spell = new Spell(template, MockScriptProvider.Instance.Object);

        spell.Level
             .Should()
             .Be(100);
    }

    [Test]
    public void Constructor_ShouldLeaveLevelAtZero_WhenLevelsUpIsTrue()
    {
        var template = CreateSpellTemplate(true);
        var spell = new Spell(template, MockScriptProvider.Instance.Object);

        spell.Level
             .Should()
             .Be(0);
    }

    [Test]
    public void Constructor_ShouldSetCastLinesFromTemplate()
    {
        var template = CreateSpellTemplate(castLines: 4);
        var spell = new Spell(template, MockScriptProvider.Instance.Object);

        spell.CastLines
             .Should()
             .Be(4);
    }

    [Test]
    public void Constructor_ShouldSetMaxLevelFromTemplate()
    {
        var template = CreateSpellTemplate(maxLevel: 50);
        var spell = new Spell(template, MockScriptProvider.Instance.Object);

        spell.MaxLevel
             .Should()
             .Be(50);
    }

    [Test]
    public void Constructor_ShouldAddExtraScriptKeys()
    {
        var template = CreateSpellTemplate();

        var extraKeys = new List<string>
        {
            "BonusScript",
            "DebuffScript"
        };
        var spell = new Spell(template, MockScriptProvider.Instance.Object, extraKeys);

        spell.ScriptKeys
             .Should()
             .Contain("BonusScript")
             .And
             .Contain("DebuffScript");
    }

    [Test]
    public void Constructor_ShouldSetCooldownFromTemplate()
    {
        var cooldown = TimeSpan.FromSeconds(10);
        var template = CreateSpellTemplate(cooldown: cooldown);
        var spell = new Spell(template, MockScriptProvider.Instance.Object);

        spell.Cooldown
             .Should()
             .Be(cooldown);
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldCallBaseUpdateAndScriptUpdate()
    {
        var spell = MockSpell.Create();
        spell.Cooldown = TimeSpan.FromSeconds(5);
        spell.Elapsed = TimeSpan.Zero;

        var delta = TimeSpan.FromSeconds(1);

        spell.Update(delta);

        spell.Elapsed
             .Should()
             .Be(TimeSpan.FromSeconds(1));

        Mock.Get(spell.Script)
            .Verify(s => s.Update(delta), Times.AtLeastOnce);
    }

    [Test]
    public void Update_ShouldClearElapsed_WhenExceedsCooldown()
    {
        var spell = MockSpell.Create();
        spell.Cooldown = TimeSpan.FromSeconds(5);
        spell.Elapsed = TimeSpan.FromSeconds(4);

        spell.Update(TimeSpan.FromSeconds(2));

        spell.Elapsed
             .Should()
             .BeNull();
    }
    #endregion

    #region Use
    [Test]
    public void Use_ShouldCallScriptOnUseAndBeginCooldown()
    {
        var template = CreateSpellTemplate(cooldown: TimeSpan.FromSeconds(5));
        var spell = new Spell(template, MockScriptProvider.Instance.Object);
        var aisling = MockAisling.Create(Map);
        var target = MockAisling.Create(Map);
        var context = new SpellContext(aisling, target);

        spell.Use(context);

        Mock.Get(spell.Script)
            .Verify(s => s.OnUse(context), Times.AtLeastOnce);

        spell.Elapsed
             .Should()
             .Be(TimeSpan.Zero);
    }

    [Test]
    public void Use_ShouldSendCooldownToAislingClient()
    {
        var template = CreateSpellTemplate(cooldown: TimeSpan.FromSeconds(5));
        var spell = new Spell(template, MockScriptProvider.Instance.Object);
        var aisling = MockAisling.Create(Map);
        var target = MockAisling.Create(Map);
        var context = new SpellContext(aisling, target);

        spell.Use(context);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(spell), Times.Once);
    }

    [Test]
    public void Use_ShouldNotSendCooldown_WhenCooldownIsNull()
    {
        var template = CreateSpellTemplate(cooldown: null);
        var spell = new Spell(template, MockScriptProvider.Instance.Object);
        var aisling = MockAisling.Create(Map);
        var target = MockAisling.Create(Map);
        var context = new SpellContext(aisling, target);

        spell.Use(context);

        Mock.Get(spell.Script)
            .Verify(s => s.OnUse(context), Times.AtLeastOnce);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(It.IsAny<PanelEntityBase>()), Times.Never);
    }

    [Test]
    public void Use_ShouldWorkWithMonsterSource()
    {
        var template = CreateSpellTemplate(cooldown: TimeSpan.FromSeconds(5));
        var spell = new Spell(template, MockScriptProvider.Instance.Object);
        var monster = MockMonster.Create(Map);
        var target = MockAisling.Create(Map);
        var context = new SpellContext(monster, target);

        spell.Use(context);

        Mock.Get(spell.Script)
            .Verify(s => s.OnUse(context), Times.AtLeastOnce);

        // Monster is not an Aisling, so no SendCooldown
        spell.Elapsed
             .Should()
             .Be(TimeSpan.Zero);
    }
    #endregion
}