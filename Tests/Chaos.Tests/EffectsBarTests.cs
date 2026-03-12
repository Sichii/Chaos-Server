#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class EffectsBarTests
{
    #region Construction
    [Test]
    public void Constructor_ShouldAcceptInitialEffects()
    {
        var monster = MockMonster.Create();
        var effectMock = CreateEffectMock("InitialEffect");
        var effectsBar = new EffectsBar(monster, [effectMock.Object]);

        effectsBar.Contains("InitialEffect")
                  .Should()
                  .BeTrue();
    }
    #endregion

    private static Mock<IEffect> CreateEffectMock(
        string name = "TestEffect",
        string scriptKey = "testEffect",
        byte icon = 1,
        TimeSpan? remaining = null)
    {
        var mock = new Mock<IEffect>();

        mock.SetupGet(e => e.Name)
            .Returns(name);

        mock.SetupGet(e => e.ScriptKey)
            .Returns(scriptKey);

        mock.SetupGet(e => e.Icon)
            .Returns(icon);
        mock.SetupProperty(e => e.Remaining, remaining ?? TimeSpan.FromSeconds(30));
        mock.SetupProperty(e => e.Color, EffectColor.Blue);
        mock.SetupProperty(e => e.Subject);
        mock.SetupProperty(e => e.Source);
        mock.SetupProperty(e => e.SourceScript);
        mock.SetupProperty(e => e.SnapshotVars);

        mock.Setup(e => e.ShouldApply(It.IsAny<Creature>(), It.IsAny<Creature>()))
            .Returns(true);

        return mock;
    }

    #region Terminate with Aisling
    [Test]
    public void Terminate_ShouldSendClearEffect_WhenAisling()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock(icon: 7);
        effectsBar.Apply(aisling, effectMock.Object);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        effectsBar.Terminate("TestEffect");

        clientMock.Verify(c => c.SendEffect(EffectColor.None, 7), Times.AtLeastOnce);
    }
    #endregion

    #region Update with Aisling
    [Test]
    public void Update_ShouldResetDisplay_WhenEffectExpires_WithAisling()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effect1 = CreateEffectMock("E1", icon: 1, remaining: TimeSpan.FromSeconds(1));
        var effect2 = CreateEffectMock("E2", icon: 2, remaining: TimeSpan.FromSeconds(30));

        effect1.Setup(e => e.Update(It.IsAny<TimeSpan>()))
               .Callback<TimeSpan>(_ => effect1.Object.Remaining = TimeSpan.Zero);

        effectsBar.Apply(aisling, effect1.Object);
        effectsBar.Apply(aisling, effect2.Object);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        effectsBar.Update(TimeSpan.FromSeconds(2));

        // E1 expired → OnTerminated + ResetDisplay (parameterless)
        effect1.Verify(e => e.OnTerminated(), Times.Once);

        // After expiry, ResetDisplay clears and re-adds E2
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 2), Times.AtLeastOnce);
    }
    #endregion

    #region Apply
    [Test]
    public void Apply_ShouldAddEffect_WhenShouldApplyReturnsTrue()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(aisling, effectMock.Object);

        effectsBar.Contains("TestEffect")
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Apply_ShouldCallOnApplied()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(aisling, effectMock.Object);

        effectMock.Verify(e => e.OnApplied(), Times.Once);
    }

    [Test]
    public void Apply_ShouldCallPrepareSnapshot()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(aisling, effectMock.Object);

        effectMock.Verify(e => e.PrepareSnapshot(aisling), Times.Once);
    }

    [Test]
    public void Apply_ShouldNotAddEffect_WhenShouldApplyReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock();

        effectMock.Setup(e => e.ShouldApply(It.IsAny<Creature>(), It.IsAny<Creature>()))
                  .Returns(false);

        effectsBar.Apply(aisling, effectMock.Object);

        effectsBar.Contains("TestEffect")
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void Apply_ShouldNotCallOnApplied_WhenShouldApplyReturnsFalse()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock();

        effectMock.Setup(e => e.ShouldApply(It.IsAny<Creature>(), It.IsAny<Creature>()))
                  .Returns(false);

        effectsBar.Apply(aisling, effectMock.Object);

        effectMock.Verify(e => e.OnApplied(), Times.Never);
    }

    [Test]
    public void Apply_ShouldSetSubjectOnEffect()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(aisling, effectMock.Object);

        effectMock.VerifySet(e => e.Subject = aisling);
    }
    #endregion

    #region Contains
    [Test]
    public void Contains_ShouldReturnTrue_WhenEffectExistsByName()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("Poison");

        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Contains("Poison")
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Contains_ShouldReturnTrue_WhenEffectExistsByScriptKey()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("Poison", "poisonEffect");

        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Contains("poisonEffect")
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Contains_ShouldReturnFalse_WhenEffectNotPresent()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);

        effectsBar.Contains("Nonexistent")
                  .Should()
                  .BeFalse();
    }
    #endregion

    #region Dispel
    [Test]
    public void Dispel_ShouldRemoveEffect()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock();
        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Dispel("TestEffect");

        effectsBar.Contains("TestEffect")
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void Dispel_ShouldCallOnDispelled()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock();
        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Dispel("TestEffect");

        effectMock.Verify(e => e.OnDispelled(), Times.Once);
    }

    [Test]
    public void Dispel_ShouldDoNothing_WhenEffectNotPresent()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);

        // Should not throw
        effectsBar.Dispel("Nonexistent");
    }
    #endregion

    #region Terminate
    [Test]
    public void Terminate_ShouldRemoveEffect()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock();
        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Terminate("TestEffect");

        effectsBar.Contains("TestEffect")
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void Terminate_ShouldCallOnTerminated()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock();
        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Terminate("TestEffect");

        effectMock.Verify(e => e.OnTerminated(), Times.Once);
    }

    [Test]
    public void Terminate_ShouldDoNothing_WhenEffectNotPresent()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);

        // Should not throw
        effectsBar.Terminate("Nonexistent");
    }
    #endregion

    #region TryGetEffect
    [Test]
    public void TryGetEffect_ShouldReturnTrue_WhenFoundByName()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("Poison");
        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.TryGetEffect("Poison", out var found)
                  .Should()
                  .BeTrue();

        found.Should()
             .BeSameAs(effectMock.Object);
    }

    [Test]
    public void TryGetEffect_ShouldReturnTrue_WhenFoundByScriptKey()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("Poison", "poisonScript");
        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.TryGetEffect("poisonScript", out var found)
                  .Should()
                  .BeTrue();

        found.Should()
             .BeSameAs(effectMock.Object);
    }

    [Test]
    public void TryGetEffect_ShouldReturnFalse_WhenNotFound()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);

        effectsBar.TryGetEffect("Nonexistent", out var found)
                  .Should()
                  .BeFalse();

        found.Should()
             .BeNull();
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldTerminateExpiredEffects()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock(remaining: TimeSpan.FromSeconds(1));

        effectMock.Setup(e => e.Update(It.IsAny<TimeSpan>()))
                  .Callback<TimeSpan>(_ => effectMock.Object.Remaining = TimeSpan.Zero);

        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Update(TimeSpan.FromSeconds(2));

        effectsBar.Contains("TestEffect")
                  .Should()
                  .BeFalse();

        effectMock.Verify(e => e.OnTerminated(), Times.Once);
    }

    [Test]
    public void Update_ShouldUpdateNonExpiredEffects()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock(remaining: TimeSpan.FromSeconds(30));

        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Update(TimeSpan.FromSeconds(1));

        effectMock.Verify(e => e.Update(TimeSpan.FromSeconds(1)), Times.Once);

        effectsBar.Contains("TestEffect")
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Update_ShouldDoNothing_WhenNoEffects()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);

        // Should not throw
        effectsBar.Update(TimeSpan.FromSeconds(1));
    }
    #endregion

    #region Enumeration
    [Test]
    public void GetEnumerator_ShouldReturnAllEffects()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);

        effectsBar.Apply(
            monster,
            CreateEffectMock("Effect1")
                .Object);

        effectsBar.Apply(
            monster,
            CreateEffectMock("Effect2")
                .Object);

        effectsBar.ToList()
                  .Should()
                  .HaveCount(2);
    }

    [Test]
    public void GetEnumerator_ShouldReturnEmpty_WhenNoEffects()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);

        effectsBar.ToList()
                  .Should()
                  .BeEmpty();
    }
    #endregion

    #region Apply with SetSource
    [Test]
    public void Apply_ShouldSetEffectColor_FromGetColor()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock(remaining: TimeSpan.FromSeconds(45));

        effectsBar.Apply(monster, effectMock.Object);

        // 45 seconds: >= 30 → Red
        effectMock.Object
                  .Color
                  .Should()
                  .Be(EffectColor.Red);
    }

    [Test]
    public void Apply_ShouldSetSourceProperty_OnEffect()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(monster, effectMock.Object);

        effectMock.Object
                  .Source
                  .Should()
                  .BeSameAs(monster);
    }

    [Test]
    public void Apply_ShouldSetUnknownVars_WhenNoSourceScript()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(monster, effectMock.Object);

        effectMock.Verify(e => e.SetVar("activatorType", "unknown"), Times.Once);
        effectMock.Verify(e => e.SetVar("activatorKey", "unknown"), Times.Once);
        effectMock.Verify(e => e.SetVar("scriptKey", "unknown"), Times.Once);
    }

    [Test]
    public void Apply_ShouldSetSourceIdentifier_WhenAislingSource()
    {
        var aisling = MockAisling.Create(name: "PlayerOne");
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(aisling, effectMock.Object);

        effectMock.Verify(e => e.SetVar("sourceType", CreatureType.Aisling), Times.Once);
        effectMock.Verify(e => e.SetVar("sourceIdentifier", "PlayerOne"), Times.Once);
    }

    [Test]
    public void Apply_ShouldSetSourceIdentifier_WhenMonsterSource()
    {
        var monster = MockMonster.Create(name: "Goblin");
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(monster, effectMock.Object);

        effectMock.Verify(e => e.SetVar("sourceType", CreatureType.Normal), Times.Once);
        effectMock.Verify(e => e.SetVar("sourceIdentifier", "goblin"), Times.Once);
    }

    [Test]
    public void Apply_ShouldSetSourceIdentifier_WhenMerchantSource()
    {
        var merchant = MockMerchant.Create(name: "ShopKeep");
        var effectsBar = new EffectsBar(merchant);
        var effectMock = CreateEffectMock();

        effectsBar.Apply(merchant, effectMock.Object);

        effectMock.Verify(e => e.SetVar("sourceType", CreatureType.Merchant), Times.Once);
        effectMock.Verify(e => e.SetVar("sourceIdentifier", "shopkeep"), Times.Once);
    }
    #endregion

    #region Dispel with Aisling
    [Test]
    public void Dispel_ShouldSendClearEffect_WhenAisling()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock(icon: 5);
        effectsBar.Apply(aisling, effectMock.Object);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        effectsBar.Dispel("TestEffect");

        clientMock.Verify(c => c.SendEffect(EffectColor.None, 5), Times.AtLeastOnce);
    }

    [Test]
    public void Dispel_ShouldReorderDisplay_WhenMultipleEffects()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effect1 = CreateEffectMock("E1", icon: 1, remaining: TimeSpan.FromSeconds(5));
        var effect2 = CreateEffectMock("E2", icon: 2, remaining: TimeSpan.FromSeconds(10));
        var effect3 = CreateEffectMock("E3", icon: 3, remaining: TimeSpan.FromSeconds(20));
        effectsBar.Apply(aisling, effect1.Object);
        effectsBar.Apply(aisling, effect2.Object);
        effectsBar.Apply(aisling, effect3.Object);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Dispel the shortest-remaining effect
        effectsBar.Dispel("E1");

        // Should clear E1's icon and reorder remaining effects above it
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 1), Times.AtLeastOnce);
    }
    #endregion

    #region ResetDisplay
    [Test]
    public void ResetDisplay_ShouldClearAndReapply_WhenAisling()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effect1 = CreateEffectMock("E1", icon: 1, remaining: TimeSpan.FromSeconds(5));
        var effect2 = CreateEffectMock("E2", icon: 2, remaining: TimeSpan.FromSeconds(10));
        effectsBar.Apply(aisling, effect1.Object);
        effectsBar.Apply(aisling, effect2.Object);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        effectsBar.ResetDisplay();

        // Should clear both icons then reapply both
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 1), Times.AtLeastOnce);
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 2), Times.AtLeastOnce);
        clientMock.Verify(c => c.SendEffect(It.IsNotIn(EffectColor.None), 1), Times.AtLeastOnce);
        clientMock.Verify(c => c.SendEffect(It.IsNotIn(EffectColor.None), 2), Times.AtLeastOnce);
    }

    [Test]
    public void ResetDisplay_ShouldDoNothing_WhenMonster()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock();
        effectsBar.Apply(monster, effectMock.Object);

        // Should not throw — null-conditional on AffectedAisling
        effectsBar.ResetDisplay();
    }

    [Test]
    public void ResetDisplay_Removed_ShouldNotReorder_WhenEffectHadLongestRemaining()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var shortEffect = CreateEffectMock("Short", icon: 1, remaining: TimeSpan.FromSeconds(5));
        var longEffect = CreateEffectMock("Long", icon: 2, remaining: TimeSpan.FromSeconds(60));
        effectsBar.Apply(aisling, shortEffect.Object);
        effectsBar.Apply(aisling, longEffect.Object);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Dispel the longest-remaining effect
        // After removal, only shortEffect remains with 5s
        // wasBeingDisplayed = Max(5s) > 60s → false → no reorder
        effectsBar.Dispel("Long");

        // Verify initial clear of the icon but no reorder SendEffect calls for shortEffect
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 2), Times.Once);

        // shortEffect should NOT be cleared and re-added (no reorder needed)
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 1), Times.Never);
    }

    [Test]
    public void ResetDisplay_Removed_ShouldReorder_WhenEffectsAboveRemoved()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effect1 = CreateEffectMock("E1", icon: 1, remaining: TimeSpan.FromSeconds(5));
        var effect2 = CreateEffectMock("E2", icon: 2, remaining: TimeSpan.FromSeconds(15));
        var effect3 = CreateEffectMock("E3", icon: 3, remaining: TimeSpan.FromSeconds(30));
        effectsBar.Apply(aisling, effect1.Object);
        effectsBar.Apply(aisling, effect2.Object);
        effectsBar.Apply(aisling, effect3.Object);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Dispel E1 (shortest remaining)
        // After removal: [E2(15s), E3(30s)]
        // wasBeingDisplayed = Max(30s) > 5s → true
        // E2.Remaining(15s) > E1.Remaining(5s) → clear and re-add E2
        // E3.Remaining(30s) > E1.Remaining(5s) → clear and re-add E3
        effectsBar.Dispel("E1");

        // E2 and E3 should be re-ordered (cleared and re-added)
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 2), Times.AtLeastOnce);
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 3), Times.AtLeastOnce);
    }

    [Test]
    public void ResetDisplay_Added_ShouldSendEffect_WhenInTopNine()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);

        var clientMock = Mock.Get(aisling.Client);

        var effectMock = CreateEffectMock("E1", icon: 1, remaining: TimeSpan.FromSeconds(10));
        effectsBar.Apply(aisling, effectMock.Object);

        // Apply triggers ResetDisplay(effect, false) which should send the effect
        clientMock.Verify(c => c.SendEffect(It.IsNotIn(EffectColor.None), 1), Times.AtLeastOnce);
    }

    [Test]
    public void ResetDisplay_Added_ShouldNotSendEffect_WhenNotInTopNine()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);

        // Add 9 effects with short remaining (they fill the display)
        for (var i = 1; i <= 9; i++)
        {
            var eff = CreateEffectMock($"E{i}", icon: (byte)i, remaining: TimeSpan.FromSeconds(i));
            effectsBar.Apply(aisling, eff.Object);
        }

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Add a 10th effect with very long remaining (not in top 9)
        var longEffect = CreateEffectMock("E10", icon: 10, remaining: TimeSpan.FromSeconds(600));
        effectsBar.Apply(aisling, longEffect.Object);

        // The 10th effect should not be displayed (icon 10 should not appear)
        clientMock.Verify(c => c.SendEffect(It.IsNotIn(EffectColor.None), 10), Times.Never);
    }

    [Test]
    public void ResetDisplay_Added_ShouldRemoveTenthEffect_WhenAtCapacity()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);

        // Add 10 effects with ascending remaining, using distinct icons
        for (var i = 1; i <= 10; i++)
        {
            var eff = CreateEffectMock($"E{i}", icon: (byte)i, remaining: TimeSpan.FromSeconds(i * 10));
            effectsBar.Apply(aisling, eff.Object);
        }

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Add an 11th effect with short remaining (will be in top 9, displacing the 10th displayed)
        // Top 10 by remaining: E11(5s), E1(10s)..E9(90s). E9 is at index 9 (10th) and gets removed.
        var newEffect = CreateEffectMock("E11", icon: 11, remaining: TimeSpan.FromSeconds(5));
        effectsBar.Apply(aisling, newEffect.Object);

        // The new effect (icon 11) should be displayed
        clientMock.Verify(c => c.SendEffect(It.IsNotIn(EffectColor.None), 11), Times.AtLeastOnce);

        // E9 (icon 9) was the 10th in the new display ordering — should be removed
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 9), Times.AtLeastOnce);
    }

    [Test]
    public void ResetDisplay_Removed_ShouldHandleEmptyCollection()
    {
        var aisling = MockAisling.Create();
        var effectsBar = new EffectsBar(aisling);
        var effectMock = CreateEffectMock(icon: 5);
        effectsBar.Apply(aisling, effectMock.Object);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Dispel the only effect — collection becomes empty
        // wasBeingDisplayed = true (default when Count == 0)
        // Loop does nothing (no remaining effects)
        effectsBar.Dispel("TestEffect");

        // Only the initial clear should happen
        clientMock.Verify(c => c.SendEffect(EffectColor.None, 5), Times.Once);
    }
    #endregion

    #region Apply with SetSource — Script Type Branches
    [Test]
    public void Apply_ShouldSetSpellVars_WhenSourceScriptIsSpellScript()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("SpellEffect");
        var spell = MockSpell.Create();
        var spellScript = new Mock<SubjectiveScriptBase<Spell>>(spell);

        effectsBar.Apply(monster, effectMock.Object, spellScript.Object);

        effectMock.Verify(e => e.SetVar("activatorType", "spell"), Times.Once);
        effectMock.Verify(e => e.SetVar("activatorKey", "testspell"), Times.Once);
        effectMock.Verify(e => e.SetVar("scriptKey", spellScript.Object.ScriptKey), Times.Once);
    }

    [Test]
    public void Apply_ShouldSetSkillVars_WhenSourceScriptIsSkillScript()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("SkillEffect");
        var skill = MockSkill.Create();
        var skillScript = new Mock<SubjectiveScriptBase<Skill>>(skill);

        effectsBar.Apply(monster, effectMock.Object, skillScript.Object);

        effectMock.Verify(e => e.SetVar("activatorType", "skill"), Times.Once);
        effectMock.Verify(e => e.SetVar("activatorKey", "testskill"), Times.Once);
        effectMock.Verify(e => e.SetVar("scriptKey", skillScript.Object.ScriptKey), Times.Once);
    }

    [Test]
    public void Apply_ShouldSetItemVars_WhenSourceScriptIsItemScript()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("ItemEffect");
        var item = MockItem.Create();
        var itemScript = new Mock<SubjectiveScriptBase<Item>>(item);

        effectsBar.Apply(monster, effectMock.Object, itemScript.Object);

        effectMock.Verify(e => e.SetVar("activatorType", "item"), Times.Once);
        effectMock.Verify(e => e.SetVar("activatorKey", "testitem"), Times.Once);
        effectMock.Verify(e => e.SetVar("scriptKey", itemScript.Object.ScriptKey), Times.Once);
    }

    [Test]
    public void Apply_ShouldUnwrapReactorTile_WhenSourceScriptIsReactorTileScript()
    {
        var monster = MockMonster.Create();
        var map = MockMapInstance.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("ReactorEffect");

        // Create a spell script as the reactor's inner SourceScript
        var spell = MockSpell.Create();
        var innerSpellScript = new Mock<SubjectiveScriptBase<Spell>>(spell);

        // Create a ReactorTile with SourceScript set to the inner spell script
        var reactor = new ReactorTile(
            map,
            new Point(1, 1),
            false,
            MockScriptProvider.Instance.Object,
            new List<string>(),
            new Dictionary<string, IScriptVars>(),
            null,
            innerSpellScript.Object);

        // Create a SubjectiveScriptBase<ReactorTile> wrapping the reactor
        var reactorScript = new Mock<SubjectiveScriptBase<ReactorTile>>(reactor);

        effectsBar.Apply(monster, effectMock.Object, reactorScript.Object);

        // SetSource should unwrap the ReactorTile and use the inner spell script
        effectMock.Verify(e => e.SetVar("activatorType", "spell"), Times.Once);
        effectMock.Verify(e => e.SetVar("activatorKey", "testspell"), Times.Once);
    }

    [Test]
    public void Apply_ShouldSetUnknownVars_WhenReactorTileHasNoSourceScript()
    {
        var monster = MockMonster.Create();
        var map = MockMapInstance.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("ReactorNoSource");

        // Create a ReactorTile with no SourceScript (null)
        var reactor = new ReactorTile(
            map,
            new Point(1, 1),
            false,
            MockScriptProvider.Instance.Object,
            new List<string>(),
            new Dictionary<string, IScriptVars>());

        var reactorScript = new Mock<SubjectiveScriptBase<ReactorTile>>(reactor);

        effectsBar.Apply(monster, effectMock.Object, reactorScript.Object);

        // With null SourceScript after unwrap, should hit the default arm
        effectMock.Verify(e => e.SetVar("activatorType", "unknown"), Times.Once);
        effectMock.Verify(e => e.SetVar("activatorKey", "unknown"), Times.Once);
    }
    #endregion

    #region ResetDisplay with Monster (null AffectedAisling paths)
    [Test]
    public void ResetDisplay_Removed_ShouldNotThrow_WhenMonsterHasMultipleEffects()
    {
        // Tests the null AffectedAisling path in the reorder loop (lines 211-212)
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effect1 = CreateEffectMock("E1", icon: 1, remaining: TimeSpan.FromSeconds(5));
        var effect2 = CreateEffectMock("E2", icon: 2, remaining: TimeSpan.FromSeconds(15));
        var effect3 = CreateEffectMock("E3", icon: 3, remaining: TimeSpan.FromSeconds(30));
        effectsBar.Apply(monster, effect1.Object);
        effectsBar.Apply(monster, effect2.Object);
        effectsBar.Apply(monster, effect3.Object);

        // Dispel shortest — reorder loop fires but null-conditional skips SendEffect
        effectsBar.Dispel("E1");

        effectsBar.Contains("E1")
                  .Should()
                  .BeFalse();

        effectsBar.Contains("E2")
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void ResetDisplay_Added_ShouldNotThrow_WhenMonsterAtCapacity()
    {
        // Tests the null AffectedAisling paths in the add-side loops (lines 242, 256, 264)
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);

        // Add 10 effects with ascending remaining
        for (var i = 1; i <= 10; i++)
        {
            var eff = CreateEffectMock($"E{i}", icon: (byte)i, remaining: TimeSpan.FromSeconds(i * 10));
            effectsBar.Apply(monster, eff.Object);
        }

        // Add an 11th effect with short remaining — triggers 10th removal + reorder
        var newEffect = CreateEffectMock("E11", icon: 11, remaining: TimeSpan.FromSeconds(5));
        effectsBar.Apply(monster, newEffect.Object);

        effectsBar.Contains("E11")
                  .Should()
                  .BeTrue();
    }
    #endregion

    #region Contains Case Insensitive
    [Test]
    public void Contains_ShouldBeCaseInsensitive_WhenSearchingByName()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("Poison");
        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Contains("POISON")
                  .Should()
                  .BeTrue();

        effectsBar.Contains("poison")
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Contains_ShouldBeCaseInsensitive_WhenSearchingByScriptKey()
    {
        var monster = MockMonster.Create();
        var effectsBar = new EffectsBar(monster);
        var effectMock = CreateEffectMock("Poison", "poisonScript");
        effectsBar.Apply(monster, effectMock.Object);

        effectsBar.Contains("POISONSCRIPT")
                  .Should()
                  .BeTrue();

        effectsBar.Contains("PoisonScript")
                  .Should()
                  .BeTrue();
    }
    #endregion
}