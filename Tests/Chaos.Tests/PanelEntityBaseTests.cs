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
///     Tests for <see cref="PanelEntityBase" /> via <see cref="Item" /> as a concrete subclass
/// </summary>
public sealed class PanelEntityBaseTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region BeginCooldown — zero-tick cooldown
    [Test]
    public void BeginCooldown_ShouldDoNothing_WhenCooldownIsZeroAndNoElapsedAndNoCustom()
    {
        // Cooldown is { Ticks: > 0 } → false for TimeSpan.Zero
        // Elapsed.HasValue → false
        // customCooldown.HasValue → false
        // All three OR operands are false → doesn't enter block
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.Zero;
        item.Elapsed = null;

        var aisling = MockAisling.Create(Map);

        item.BeginCooldown(aisling);

        item.Elapsed
            .Should()
            .BeNull();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(It.IsAny<PanelEntityBase>()), Times.Never);
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_ShouldLeaveElapsedNull_WhenElapsedMsNotProvided()
    {
        var item = MockItem.Create();

        item.Elapsed
            .Should()
            .BeNull();
    }

    [Test]
    public void Constructor_ShouldSetElapsed_WhenElapsedMsProvided()
    {
        var template = new ItemTemplate
        {
            Name = "Test",
            TemplateKey = "test",
            ItemSprite = new ItemSprite(1, 1),
            PanelSprite = 1,
            Color = DisplayColor.Default,
            PantsColor = DisplayColor.Default,
            MaxStacks = 1,
            BuyCost = 0,
            SellValue = 0,
            Category = "test",
            Description = null,
            EquipmentType = null,
            Gender = null,
            Class = null,
            AdvClass = null,
            IsDyeable = false,
            IsModifiable = false,
            NoTrade = false,
            AccountBound = false,
            PreventBanking = false,
            Level = 1,
            AbilityLevel = 0,
            MaxDurability = null,
            Modifiers = null,
            Weight = 1,
            Cooldown = null,
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var item = new Item(template, MockScriptProvider.Instance.Object, elapsedMs: 500);

        item.Elapsed
            .Should()
            .Be(TimeSpan.FromMilliseconds(500));
    }

    [Test]
    public void Constructor_ShouldCopyCooldownFromTemplate()
    {
        var template = new ItemTemplate
        {
            Name = "Test",
            TemplateKey = "test",
            ItemSprite = new ItemSprite(1, 1),
            PanelSprite = 1,
            Color = DisplayColor.Default,
            PantsColor = DisplayColor.Default,
            MaxStacks = 1,
            BuyCost = 0,
            SellValue = 0,
            Category = "test",
            Description = null,
            EquipmentType = null,
            Gender = null,
            Class = null,
            AdvClass = null,
            IsDyeable = false,
            IsModifiable = false,
            NoTrade = false,
            AccountBound = false,
            PreventBanking = false,
            Level = 1,
            AbilityLevel = 0,
            MaxDurability = null,
            Modifiers = null,
            Weight = 1,
            Cooldown = TimeSpan.FromSeconds(5),
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var item = new Item(template, MockScriptProvider.Instance.Object);

        item.Cooldown
            .Should()
            .Be(TimeSpan.FromSeconds(5));
    }

    [Test]
    public void Constructor_ShouldCopyScriptKeysFromTemplate()
    {
        var template = new ItemTemplate
        {
            Name = "Test",
            TemplateKey = "test",
            ItemSprite = new ItemSprite(1, 1),
            PanelSprite = 1,
            Color = DisplayColor.Default,
            PantsColor = DisplayColor.Default,
            MaxStacks = 1,
            BuyCost = 0,
            SellValue = 0,
            Category = "test",
            Description = null,
            EquipmentType = null,
            Gender = null,
            Class = null,
            AdvClass = null,
            IsDyeable = false,
            IsModifiable = false,
            NoTrade = false,
            AccountBound = false,
            PreventBanking = false,
            Level = 1,
            AbilityLevel = 0,
            MaxDurability = null,
            Modifiers = null,
            Weight = 1,
            Cooldown = null,
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "TestScript",
                "BuffScript"
            },
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var item = new Item(template, MockScriptProvider.Instance.Object);

        item.ScriptKeys
            .Should()
            .Contain("TestScript")
            .And
            .Contain("BuffScript");
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldDoNothing_WhenElapsedIsNull()
    {
        var item = MockItem.Create();
        item.Elapsed = null;
        item.Cooldown = TimeSpan.FromSeconds(5);

        item.Update(TimeSpan.FromSeconds(1));

        item.Elapsed
            .Should()
            .BeNull();
    }

    [Test]
    public void Update_ShouldIncrementElapsed_WhenElapsedPlusDeltaIsLessThanCooldown()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = TimeSpan.Zero;

        item.Update(TimeSpan.FromSeconds(2));

        item.Elapsed
            .Should()
            .Be(TimeSpan.FromSeconds(2));
    }

    [Test]
    public void Update_ShouldSetElapsedToNull_WhenElapsedPlusDeltaExceedsCooldown()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = TimeSpan.FromSeconds(3);

        item.Update(TimeSpan.FromSeconds(3));

        item.Elapsed
            .Should()
            .BeNull();
    }

    [Test]
    public void Update_ShouldUseZeroAsCooldown_WhenCooldownIsNull()
    {
        // When Cooldown is null, the expression (Cooldown ?? TimeSpan.Zero) yields Zero
        // So any Elapsed + delta > Zero should clear Elapsed
        var item = MockItem.Create();
        item.Cooldown = null;
        item.Elapsed = TimeSpan.Zero;

        item.Update(TimeSpan.FromMilliseconds(1));

        item.Elapsed
            .Should()
            .BeNull();
    }

    [Test]
    public void Update_ShouldNotClearElapsed_WhenElapsedPlusDeltaEqualsCooldown()
    {
        // Elapsed > Cooldown is required to clear, so exactly equal should NOT clear
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = TimeSpan.FromSeconds(2);

        item.Update(TimeSpan.FromSeconds(3));

        item.Elapsed
            .Should()
            .Be(TimeSpan.FromSeconds(5));
    }
    #endregion

    #region BeginCooldown
    [Test]
    public void BeginCooldown_ShouldDoNothing_WhenNoCooldownNoElapsedNoCustom()
    {
        var item = MockItem.Create();
        item.Cooldown = null;
        item.Elapsed = null;

        var aisling = MockAisling.Create(Map);

        item.BeginCooldown(aisling);

        item.Elapsed
            .Should()
            .BeNull();

        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(It.IsAny<PanelEntityBase>()), Times.Never);
    }

    [Test]
    public void BeginCooldown_ShouldSetElapsedToZeroAndSendCooldown_WhenCooldownHasPositiveTicks()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = null;

        var aisling = MockAisling.Create(Map);

        item.BeginCooldown(aisling);

        item.Elapsed
            .Should()
            .Be(TimeSpan.Zero);

        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(item), Times.Once);
    }

    [Test]
    public void BeginCooldown_ShouldSetElapsedButNotSendCooldown_WhenCreatureIsNotAisling()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = null;

        var monster = MockMonster.Create(Map);

        item.BeginCooldown(monster);

        item.Elapsed
            .Should()
            .Be(TimeSpan.Zero);
    }

    [Test]
    public void BeginCooldown_ShouldNotOverrideElapsed_WhenElapsedAlreadySet()
    {
        // Elapsed ??= means it should not overwrite an existing value
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = TimeSpan.FromSeconds(2);

        var aisling = MockAisling.Create(Map);

        item.BeginCooldown(aisling);

        item.Elapsed
            .Should()
            .Be(TimeSpan.FromSeconds(2));
    }

    [Test]
    public void BeginCooldown_ShouldSetElapsedToOffset_WhenCustomCooldownProvided()
    {
        // Elapsed = (Cooldown ?? Zero) - customCooldown
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(10);
        item.Elapsed = null;

        var aisling = MockAisling.Create(Map);

        item.BeginCooldown(aisling, TimeSpan.FromSeconds(3));

        item.Elapsed
            .Should()
            .Be(TimeSpan.FromSeconds(7));
    }

    [Test]
    public void BeginCooldown_ShouldUseZeroForCooldown_WhenCustomCooldownProvidedButCooldownIsNull()
    {
        // Elapsed = (null ?? Zero) - customCooldown = Zero - customCooldown = negative
        var item = MockItem.Create();
        item.Cooldown = null;
        item.Elapsed = null;

        var aisling = MockAisling.Create(Map);

        item.BeginCooldown(aisling, TimeSpan.FromSeconds(3));

        item.Elapsed
            .Should()
            .Be(TimeSpan.Zero - TimeSpan.FromSeconds(3));
    }

    [Test]
    public void BeginCooldown_ShouldEnterBlock_WhenElapsedIsAlreadySet_EvenWithZeroCooldown()
    {
        // Condition: Elapsed.HasValue is true, so we enter the block
        var item = MockItem.Create();
        item.Cooldown = null;
        item.Elapsed = TimeSpan.FromSeconds(1);

        var aisling = MockAisling.Create(Map);

        item.BeginCooldown(aisling);

        // Elapsed ??= should keep original value since it's already set
        item.Elapsed
            .Should()
            .Be(TimeSpan.FromSeconds(1));

        Mock.Get(aisling.Client)
            .Verify(c => c.SendCooldown(item), Times.Once);
    }
    #endregion

    #region CanUse
    [Test]
    public void CanUse_ShouldReturnTrue_WhenCooldownIsNull()
    {
        var item = MockItem.Create();
        item.Cooldown = null;

        item.CanUse()
            .Should()
            .BeTrue();
    }

    [Test]
    public void CanUse_ShouldReturnTrue_WhenCooldownSetButElapsedIsNull()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = null;

        item.CanUse()
            .Should()
            .BeTrue();
    }

    [Test]
    public void CanUse_ShouldReturnFalse_WhenElapsedIsLessThanCooldown()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = TimeSpan.FromSeconds(2);

        item.CanUse()
            .Should()
            .BeFalse();
    }

    [Test]
    public void CanUse_ShouldReturnTrue_WhenElapsedIsGreaterThanCooldown()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = TimeSpan.FromSeconds(6);

        item.CanUse()
            .Should()
            .BeTrue();
    }

    [Test]
    public void CanUse_ShouldReturnFalse_WhenElapsedEqualsCooldown()
    {
        // Elapsed > Cooldown is required, so exactly equal is false
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(5);
        item.Elapsed = TimeSpan.FromSeconds(5);

        item.CanUse()
            .Should()
            .BeFalse();
    }
    #endregion

    #region SetTemporaryCooldown
    [Test]
    public void SetTemporaryCooldown_ShouldSetElapsedToCooldownMinusTemporary()
    {
        var item = MockItem.Create();
        item.Cooldown = TimeSpan.FromSeconds(10);

        item.SetTemporaryCooldown(TimeSpan.FromSeconds(3));

        item.Elapsed
            .Should()
            .Be(TimeSpan.FromSeconds(7));
    }

    [Test]
    public void SetTemporaryCooldown_ShouldUseZero_WhenCooldownIsNull()
    {
        var item = MockItem.Create();
        item.Cooldown = null;

        item.SetTemporaryCooldown(TimeSpan.FromSeconds(3));

        item.Elapsed
            .Should()
            .Be(TimeSpan.Zero - TimeSpan.FromSeconds(3));
    }
    #endregion
}