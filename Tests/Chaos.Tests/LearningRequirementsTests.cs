#region
using Chaos.Models.Data;
using Chaos.Services.Factories.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class LearningRequirementsTests
{
    private readonly Mock<IItemFactory> ItemFactory = new();
    private readonly Mock<ISkillFactory> SkillFactory = new();
    private readonly Mock<ISpellFactory> SpellFactory = new();

    private static LearningRequirements CreateRequirements(
        Stats? stats = null,
        int? gold = null,
        ICollection<AbilityRequirement>? skills = null,
        ICollection<AbilityRequirement>? spells = null,
        ICollection<ItemRequirement>? items = null)
        => new()
        {
            RequiredStats = stats,
            RequiredGold = gold,
            PrerequisiteSkills = skills ?? [],
            PrerequisiteSpells = spells ?? [],
            ItemRequirements = items ?? []
        };

    #region BuildRequirementsString
    [Test]
    public void BuildRequirementsString_NoRequirements_ShouldReturnHeaderOnly()
    {
        var req = CreateRequirements();

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("Required Items");

        str.Should()
           .Contain("Required Skills");

        str.Should()
           .Contain("Required Spells");

        str.Should()
           .NotContain("Required Attributes");

        str.Should()
           .NotContain("Required Gold");
    }

    [Test]
    public void BuildRequirementsString_WithStats_ShouldIncludeAttributeLine()
    {
        var stats = new Stats
        {
            Str = 10,
            Int = 5,
            Wis = 3,
            Con = 8,
            Dex = 7
        };

        var req = CreateRequirements(stats);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("Required Attributes: STR: 10, INT: 5, WIS: 3, CON: 8, DEX: 7");
    }

    [Test]
    public void BuildRequirementsString_WithGold_ShouldIncludeGoldLine()
    {
        var req = CreateRequirements(gold: 50000);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("Required Gold: 50000 gold");
    }

    [Test]
    public void BuildRequirementsString_WithStatsAndGold_ShouldIncludeBothLines()
    {
        var stats = new Stats
        {
            Str = 1,
            Int = 2,
            Wis = 3,
            Con = 4,
            Dex = 5
        };

        var req = CreateRequirements(stats, 1000);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("Required Attributes");

        str.Should()
           .Contain("Required Gold: 1000 gold");
    }

    [Test]
    public void BuildRequirementsString_WithPrerequisiteSkills_ShouldIncludeSkillNames()
    {
        var skills = new List<AbilityRequirement>
        {
            new()
            {
                TemplateKey = "slash",
                Level = 5
            }
        };

        var mockSkill = MockSkill.Create("Slash");

        SkillFactory.Setup(f => f.CreateFaux("slash", null))
                    .Returns(mockSkill);

        var req = CreateRequirements(skills: skills);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("Slash 5");
    }

    [Test]
    public void BuildRequirementsString_WithPrerequisiteSkills_NullLevel_ShouldUseMaxLevel()
    {
        var skills = new List<AbilityRequirement>
        {
            new()
            {
                TemplateKey = "slash",
                Level = null
            }
        };

        var mockSkill = MockSkill.Create("Slash");

        SkillFactory.Setup(f => f.CreateFaux("slash", null))
                    .Returns(mockSkill);

        var req = CreateRequirements(skills: skills);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        // MockSkill template MaxLevel = 100
        str.Should()
           .Contain("Slash 100");
    }

    [Test]
    public void BuildRequirementsString_WithPrerequisiteSpells_ShouldIncludeSpellNames()
    {
        var spells = new List<AbilityRequirement>
        {
            new()
            {
                TemplateKey = "heal",
                Level = 10
            }
        };

        var mockSpell = MockSpell.Create("Heal");

        SpellFactory.Setup(f => f.CreateFaux("heal", null))
                    .Returns(mockSpell);

        var req = CreateRequirements(spells: spells);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("Heal 10");
    }

    [Test]
    public void BuildRequirementsString_WithItemRequirements_ShouldIncludeItemNames()
    {
        var items = new List<ItemRequirement>
        {
            new()
            {
                ItemTemplateKey = "sword",
                AmountRequired = 3
            }
        };

        var mockItem = MockItem.Create("Sword");

        ItemFactory.Setup(f => f.CreateFaux("sword", null))
                   .Returns(mockItem);

        var req = CreateRequirements(items: items);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("3x Sword(s)");
    }

    [Test]
    public void BuildRequirementsString_WithAllRequirements_ShouldIncludeEverything()
    {
        var stats = new Stats
        {
            Str = 10,
            Int = 10,
            Wis = 10,
            Con = 10,
            Dex = 10
        };

        var skills = new List<AbilityRequirement>
        {
            new()
            {
                TemplateKey = "slash",
                Level = 5
            }
        };

        var spells = new List<AbilityRequirement>
        {
            new()
            {
                TemplateKey = "heal",
                Level = 10
            }
        };

        var items = new List<ItemRequirement>
        {
            new()
            {
                ItemTemplateKey = "gem",
                AmountRequired = 2
            }
        };

        SkillFactory.Setup(f => f.CreateFaux("slash", null))
                    .Returns(MockSkill.Create("Slash"));

        SpellFactory.Setup(f => f.CreateFaux("heal", null))
                    .Returns(MockSpell.Create("Heal"));

        ItemFactory.Setup(f => f.CreateFaux("gem", null))
                   .Returns(MockItem.Create("Gem"));

        var req = CreateRequirements(
            stats,
            5000,
            skills,
            spells,
            items);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("Required Attributes");

        str.Should()
           .Contain("Required Gold: 5000 gold");

        str.Should()
           .Contain("Slash 5");

        str.Should()
           .Contain("Heal 10");

        str.Should()
           .Contain("2x Gem(s)");
    }

    [Test]
    public void BuildRequirementsString_WithMultiplePrerequisites_ShouldListAll()
    {
        var skills = new List<AbilityRequirement>
        {
            new()
            {
                TemplateKey = "slash",
                Level = 5
            },
            new()
            {
                TemplateKey = "thrust",
                Level = 3
            }
        };

        SkillFactory.Setup(f => f.CreateFaux("slash", null))
                    .Returns(MockSkill.Create("Slash"));

        SkillFactory.Setup(f => f.CreateFaux("thrust", null))
                    .Returns(MockSkill.Create("Thrust"));

        var req = CreateRequirements(skills: skills);

        var result = req.BuildRequirementsString(ItemFactory.Object, SkillFactory.Object, SpellFactory.Object);
        var str = result.ToString();

        str.Should()
           .Contain("Slash 5");

        str.Should()
           .Contain("Thrust 3");
    }
    #endregion
}