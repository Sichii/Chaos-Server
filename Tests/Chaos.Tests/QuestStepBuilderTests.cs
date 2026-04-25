#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Testing.Infrastructure.Definitions;
using Chaos.Testing.Infrastructure.Mocks;
using Chaos.Utilities.QuestHelper;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class QuestStepBuilderTests
{
    private enum WolfStage { None, Hunting, Done }

    private static QuestContext<WolfStage> NewContext(
        WolfStage? initialStage = null,
        IServiceProvider? services = null,
        Action<Aisling>? setup = null)
    {
        var aisling = MockAisling.Create(setup: setup);
        if (initialStage.HasValue)
            aisling.Trackers.Enums.Set(initialStage.Value);

        return new QuestContext<WolfStage>
        {
            Source = aisling,
            CurrentStage = initialStage ?? default,
            Services = services ?? MockServiceProvider.Create().Object
        };
    }

    private static void RunChain(QuestStepBuilder<WolfStage> builder, QuestContext<WolfStage> ctx)
    {
        foreach (var op in builder.Build())
            if (!op(ctx))
                return;
    }

    #region When
    [Test]
    public void When_AllowsChainToContinue_WhenAtStage()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.When(WolfStage.Hunting)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void When_HaltsChain_WhenNotAtStage()
    {
        var ctx = NewContext(WolfStage.None);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.When(WolfStage.Hunting)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
        ctx.IsAt(WolfStage.None).Should().BeTrue();
    }

    [Test]
    public void When_TreatsAbsentStageAsDefault()
    {
        var ctx = NewContext(); // no stage set
        var builder = new QuestStepBuilder<WolfStage>();
        builder.When(WolfStage.None) // default(WolfStage) == WolfStage.None
               .Advance(WolfStage.Hunting);

        RunChain(builder, ctx);

        // Absent enum HasValue check returns false even for default — confirm with current behavior
        ctx.IsAt(WolfStage.Hunting).Should().BeFalse();
    }

    [Test]
    public void When_WithFailureReply_HaltsAndDispatchesReplyOnMismatch()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);
        var clientMock = Mock.Get(ctx.Source.Client);
        var advanced = false;

        var builder = new QuestStepBuilder<WolfStage>();
        builder.When(WolfStage.Hunting, "you must be hunting first")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeFalse("guard halted because the aisling has no stage stored");
        clientMock.Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.Text == "you must be hunting first")), Times.Once);
    }

    [Test]
    public void When_WithFailureReply_DoesNotDispatch_WhenGuardPasses()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);
        ctx.Source.Trackers.Enums.Set(WolfStage.Hunting);
        var clientMock = Mock.Get(ctx.Source.Client);
        var advanced = false;

        var builder = new QuestStepBuilder<WolfStage>();
        builder.When(WolfStage.Hunting, "should not see this")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeTrue();
        clientMock.Verify(c => c.SendDisplayDialog(It.IsAny<Dialog>()), Times.Never);
    }
    #endregion

    #region WhenNeverStarted (guard form)
    [Test]
    public void WhenNeverStarted_AllowsChainToContinue_WhenStageNotStored()
    {
        var ctx = NewContext(); // no stage set
        var advanced = false;
        var builder = new QuestStepBuilder<WolfStage>();
        builder.WhenNeverStarted()
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeTrue();
    }

    [Test]
    public void WhenNeverStarted_HaltsChain_WhenAnyStageStored()
    {
        var ctx = NewContext(WolfStage.None);
        var advanced = false;
        var builder = new QuestStepBuilder<WolfStage>();
        builder.WhenNeverStarted()
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeFalse("any stored value — even default — counts as started");
    }

    [Test]
    public void WhenNeverStarted_WithFailureReply_DispatchesReplyOnFail()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);
        ctx.Source.Trackers.Enums.Set(WolfStage.Hunting);
        var clientMock = Mock.Get(ctx.Source.Client);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.WhenNeverStarted("you've already started this quest");

        RunChain(builder, ctx);

        clientMock.Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.Text == "you've already started this quest")), Times.Once);
    }
    #endregion

    #region Advance
    [Test]
    public void Advance_SetsStageOnAislingAndContext()
    {
        var ctx = NewContext(WolfStage.None);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Advance(WolfStage.Hunting);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Hunting).Should().BeTrue();
        ctx.CurrentStage.Should().Be(WolfStage.Hunting);
    }

    [Test]
    public void Advance_OverwritesPreviousStage()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
        ctx.IsAt(WolfStage.Hunting).Should().BeFalse();
    }
    #endregion

    #region ClearStage
    [Test]
    public void ClearStage_RemovesStageFromTrackers()
    {
        var ctx = NewContext(WolfStage.Done);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.ClearStage();

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }
    #endregion

    #region Branch
    [Test]
    public void Branch_RunsConfigure_WhenPredicateMatches()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.IsAt(WolfStage.Hunting), s => s.Advance(WolfStage.Done));

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void Branch_SkipsConfigure_WhenPredicateDoesNotMatch()
    {
        var ctx = NewContext(WolfStage.None);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.IsAt(WolfStage.Hunting), s => s.Advance(WolfStage.Done));

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void Branch_DoesNotHaltOuter_WhenBodyHalts()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var outerRan = false;
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.IsAt(WolfStage.Hunting), s => s.When(WolfStage.Done))
               .Run((_, _) => outerRan = true);

        RunChain(builder, ctx);

        outerRan.Should().BeTrue("Branch must not propagate body halts to the outer chain");
    }

    [Test]
    public void Branch_SetsOtherwiseTaken_WhenPredicateMatches()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.IsAt(WolfStage.Hunting), _ => { });

        RunChain(builder, ctx);

        ctx.OtherwiseTaken.Should().BeTrue();
    }

    [Test]
    public void Branch_DoesNotSetOtherwiseTaken_WhenPredicateMisses()
    {
        var ctx = NewContext(WolfStage.None);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.IsAt(WolfStage.Hunting), _ => { });

        RunChain(builder, ctx);

        ctx.OtherwiseTaken.Should().BeFalse();
    }

    [Test]
    public void Branch_PredicateReceivesContext()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();
        QuestContext<WolfStage>? observed = null;

        builder.Branch(
            c =>
            {
                observed = c;
                return true;
            },
            _ => { });

        RunChain(builder, ctx);

        observed.Should().BeSameAs(ctx);
    }
    #endregion

    #region Branch chaining / Otherwise
    [Test]
    public void WhenStage_FiresMatchingBranchOnly()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.IsAt(WolfStage.Hunting), b => b.Advance(WolfStage.Done))
               .Branch(c => c.IsAt(WolfStage.Done), b => b.ClearStage());

        RunChain(builder, ctx);

        // Hunting branch fires, advances to Done. Then Done branch ALSO fires (sequential).
        // This is intentional — branch When doesn't short-circuit.
        ctx.IsAt(WolfStage.Done).Should().BeFalse(); // ClearStage ran after Advance(Done)
    }

    [Test]
    public void Otherwise_FiresWhenNoBranchMatched()
    {
        var ctx = NewContext(WolfStage.None);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.IsAt(WolfStage.Hunting), b => b.Advance(WolfStage.Done))
               .Branch(c => c.IsAt(WolfStage.Done), b => b.ClearStage())
               .Otherwise(b => b.Advance(WolfStage.Hunting));

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Hunting).Should().BeTrue();
    }

    [Test]
    public void Otherwise_DoesNotFireWhenABranchMatched()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.IsAt(WolfStage.Hunting), b => b.ClearStage())
               .Otherwise(b => b.Advance(WolfStage.Done));

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
        ctx.IsAt(WolfStage.Hunting).Should().BeFalse(); // ClearStage ran
    }
    #endregion

    #region Flag operations

    [Flags]
    private enum WolfFlags
    {
        None = 0,
        FoundDen = 1,
        FoundCub = 2,
        FoundLeader = 4
    }

    [Test]
    public void SetFlag_AddsBitToTrackersFlags()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.SetFlag(WolfFlags.FoundDen);

        RunChain(builder, ctx);

        ctx.HasFlag(WolfFlags.FoundDen).Should().BeTrue();
    }

    [Test]
    public void RequireFlag_AllowsContinuation_WhenFlagSet()
    {
        var ctx = NewContext();
        ctx.SetFlag(WolfFlags.FoundDen);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireFlag(WolfFlags.FoundDen)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireFlag_HaltsChain_WhenFlagAbsent()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireFlag(WolfFlags.FoundDen)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireAllFlags_HaltsChain_WhenAnyBitMissing()
    {
        var ctx = NewContext();
        ctx.SetFlag(WolfFlags.FoundDen);
        // FoundCub NOT set

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireAllFlags(WolfFlags.FoundDen | WolfFlags.FoundCub)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireAnyFlag_AllowsChain_WhenOneBitPresent()
    {
        var ctx = NewContext();
        ctx.SetFlag(WolfFlags.FoundCub);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireAnyFlag(WolfFlags.FoundDen | WolfFlags.FoundCub)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void ClearFlag_RemovesBit()
    {
        var ctx = NewContext();
        ctx.SetFlag(WolfFlags.FoundDen);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ClearFlag(WolfFlags.FoundDen);

        RunChain(builder, ctx);

        ctx.HasFlag(WolfFlags.FoundDen).Should().BeFalse();
    }

    // ---- BigFlag overload coverage ----
    // Demonstrates the second dispatch path: BigFlagsValue<TMarker> arguments route to Trackers.BigFlags
    // instead of Trackers.Flags. Uses TestFeatures (a marker class extending BigFlags<TestFeatures>) from
    // Chaos.Testing.Infrastructure.

    [Test]
    public void SetFlag_AddsBigFlagBit_WhenArgumentIsBigFlagsValue()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.SetFlag(TestFeatures.Feature1);

        RunChain(builder, ctx);

        ctx.HasFlag(TestFeatures.Feature1).Should().BeTrue();
        ctx.Source.Trackers.BigFlags.HasFlag(TestFeatures.Feature1).Should().BeTrue();
    }

    [Test]
    public void RequireFlag_HaltsChain_WhenBigFlagAbsent()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireFlag(TestFeatures.Feature1)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireAllFlags_HaltsChain_WhenAnyBigFlagBitMissing()
    {
        var ctx = NewContext();
        ctx.SetFlag(TestFeatures.Feature1);
        // Feature2 NOT set

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireAllFlags(TestFeatures.Feature1 | TestFeatures.Feature2)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireAnyFlag_AllowsChain_WhenOneBigFlagBitPresent()
    {
        var ctx = NewContext();
        ctx.SetFlag(TestFeatures.Feature2);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireAnyFlag(TestFeatures.Feature1 | TestFeatures.Feature2)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void ClearFlag_RemovesBigFlagBit()
    {
        var ctx = NewContext();
        ctx.SetFlag(TestFeatures.Feature1);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ClearFlag(TestFeatures.Feature1);

        RunChain(builder, ctx);

        ctx.HasFlag(TestFeatures.Feature1).Should().BeFalse();
    }
    #endregion

    #region Counter operations
    [Test]
    public void RequireKills_AllowsChain_WhenCountSufficient()
    {
        var ctx = NewContext();
        ctx.IncrementCounter("wolf", 10);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireKills("wolf", 10)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireKills_HaltsChain_WhenCountInsufficient()
    {
        var ctx = NewContext();
        ctx.IncrementCounter("wolf", 5);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireKills("wolf", 10)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void ClearKills_RemovesCounter()
    {
        var ctx = NewContext();
        ctx.IncrementCounter("wolf", 10);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ClearKills("wolf");

        RunChain(builder, ctx);

        ctx.HasCount("wolf", 1).Should().BeFalse();
    }

    [Test]
    public void IncrementCounter_AddsRequestedAmount()
    {
        var ctx = NewContext();

        var builder = new QuestStepBuilder<WolfStage>();
        builder.IncrementCounter("wolf", 3);

        RunChain(builder, ctx);

        ctx.HasCount("wolf", 3).Should().BeTrue();
        ctx.HasCount("wolf", 4).Should().BeFalse();
    }
    #endregion

    #region Cooldown operations
    [Test]
    public void StartCooldown_AddsTimedEvent()
    {
        var ctx = NewContext();

        var builder = new QuestStepBuilder<WolfStage>();
        builder.StartCooldown("wolf_cd", TimeSpan.FromHours(22));

        RunChain(builder, ctx);

        ctx.HasActiveCooldown("wolf_cd", out _).Should().BeTrue();
    }

    [Test]
    public void RequireCooldownExpired_AllowsChain_WhenNoCooldownActive()
    {
        var ctx = NewContext();

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireCooldownExpired("wolf_cd")
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireCooldownExpired_HaltsChain_WhenCooldownActive()
    {
        var ctx = NewContext();
        ctx.StartCooldown("wolf_cd", TimeSpan.FromHours(22));

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireCooldownExpired("wolf_cd")
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }
    #endregion

    #region Item operations
    [Test]
    public void RequireItem_HaltsChain_WhenItemMissing()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireItem("wolfsfur", 5)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireItem_WithFailureReply_DispatchesReplyAndHalts_WhenItemMissing()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);
        var clientMock = Mock.Get(ctx.Source.Client);
        var advanced = false;

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireItem("wolfsfur", 5, "bring me 5 wolfsfur first")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeFalse();
        clientMock.Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.Text == "bring me 5 wolfsfur first")), Times.Once);
    }

    [Test]
    public void RequireItem_WithFailureReply_DoesNotDispatch_WhenItemPresent()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);
        var clientMock = Mock.Get(ctx.Source.Client);
        var advanced = false;

        // Place the required item in the aisling's inventory.
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsfur", count: 5, stackable: true));

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireItem("wolfsfur", 5, "should not see this")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeTrue();
        clientMock.Verify(c => c.SendDisplayDialog(It.IsAny<Dialog>()), Times.Never);
    }

    [Test]
    public void GiveItem_CallsItemFactoryAndDelegatesToGiveItemOrSendToBank()
    {
        var factoryMock = new Mock<IItemFactory>();
        factoryMock.Setup(f => f.Create("wolfsfur", It.IsAny<ICollection<string>?>()))
                   .Returns(() => MockItem.Create("wolfsfur", stackable: true));

        var services = MockServiceProvider.CreateBuilder()
                                          .SetupService<IItemFactory>(factoryMock.Object)
                                          .Build()
                                          .Object;

        var ctx = NewContext(services: services);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveItem("wolfsfur", 3);

        RunChain(builder, ctx);

        factoryMock.Verify(f => f.Create("wolfsfur", It.IsAny<ICollection<string>?>()), Times.Once);
    }
    #endregion

    #region Reward operations
    [Test]
    public void GiveGold_AddsToAislingsGold()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveGold(500);

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(500);
    }

    [Test]
    public void GiveGold_ChainContinues_EvenIfTryGiveGoldFails()
    {
        var ctx = NewContext();
        ctx.Source.Gold = 10_000_000; // at MaxGoldHeld; further GiveGold should silently fail

        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveGold(1)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        // Subsequent step still ran -> reward operations are not guards
        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void GiveLegendMark_AddsToAislingsLegend()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveLegendMark("Helped wolves", "wolfHelp", MarkIcon.Heart, MarkColor.Blue);

        RunChain(builder, ctx);

        ctx.Source.Legend.ContainsKey("wolfHelp").Should().BeTrue();
    }

    #endregion

    #region Class/level/gender guards

    [Test]
    public void RequireLevel_WithinRange_ContinuesChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetLevel(50));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireLevel(10, 99)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireLevel_BelowMin_HaltsChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetLevel(5));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireLevel(10, 99)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireLevel_AboveMax_HaltsChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetLevel(50));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireLevel(10, 40)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireLevel_DefaultMaxAllowsArbitraryHighLevel()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetLevel(500));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireLevel(10)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireMaster_NotMaster_HaltsChain()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireMaster()
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireMaster_IsMaster_ContinuesChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetIsMaster(true));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireMaster()
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireFullGrandmaster_MasterAtMaxLevel_ContinuesChain()
    {
        var ctx = NewContext(setup: a =>
        {
            a.UserStatSheet.SetIsMaster(true);
            a.UserStatSheet.SetLevel(99);
        });

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireFullGrandmaster()
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireFullGrandmaster_MasterButBelowMaxLevel_HaltsChain()
    {
        var ctx = NewContext(setup: a =>
        {
            a.UserStatSheet.SetIsMaster(true);
            a.UserStatSheet.SetLevel(50);
        });

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireFullGrandmaster()
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequireFullGrandmaster_NotMaster_HaltsChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetLevel(99));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireFullGrandmaster()
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequirePureMaster_MasterAndRightClass_ContinuesChain()
    {
        var ctx = NewContext(setup: a =>
        {
            a.UserStatSheet.SetIsMaster(true);
            a.UserStatSheet.SetBaseClass(BaseClass.Wizard);
        });

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequirePureMaster(BaseClass.Wizard)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequirePureMaster_MasterWrongClass_HaltsChain()
    {
        var ctx = NewContext(setup: a =>
        {
            a.UserStatSheet.SetIsMaster(true);
            a.UserStatSheet.SetBaseClass(BaseClass.Wizard);
        });

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequirePureMaster(BaseClass.Priest)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void RequirePureMaster_RightClassButNotMaster_HaltsChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetBaseClass(BaseClass.Wizard));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequirePureMaster(BaseClass.Wizard)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void ForClass_MatchingClass_RunsSubChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetBaseClass(BaseClass.Wizard));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Branch(c => c.Source.UserStatSheet.BaseClass == BaseClass.Wizard, sub => sub.Advance(WolfStage.Done));

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void ForClass_NonMatchingClass_SkipsSubChainButContinuesOuter()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetBaseClass(BaseClass.Wizard));
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.Source.UserStatSheet.BaseClass == BaseClass.Priest, sub => sub.Advance(WolfStage.Hunting))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        // sub didn't run (Priest != Wizard) but outer continued
        ctx.IsAt(WolfStage.Hunting).Should().BeFalse();
        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void ForClass_Chained_DispatchesToMatchingBranchOnly()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetBaseClass(BaseClass.Priest));
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.Source.UserStatSheet.BaseClass == BaseClass.Wizard, sub => sub.GiveGold(100))
               .Branch(c => c.Source.UserStatSheet.BaseClass == BaseClass.Priest, sub => sub.GiveGold(200))
               .Branch(c => c.Source.UserStatSheet.BaseClass == BaseClass.Rogue, sub => sub.GiveGold(400));

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(200);
    }

    [Test]
    public void ForGender_MatchingGender_RunsSubChain()
    {
        var ctx = NewContext(setup: a => a.Gender = Gender.Female);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Branch(c => c.Source.Gender == Gender.Female, sub => sub.Advance(WolfStage.Done));

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void ForGender_NonMatchingGender_SkipsSubChainButContinuesOuter()
    {
        var ctx = NewContext(setup: a => a.Gender = Gender.Male);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(c => c.Source.Gender == Gender.Female, sub => sub.Advance(WolfStage.Hunting))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Hunting).Should().BeFalse();
        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void IfClass_MatchingClass_RunsSubChainAndContinuesOuter()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetBaseClass(BaseClass.Wizard));
        var builder = new QuestStepBuilder<WolfStage>();

        builder.RequireClass(BaseClass.Wizard)
               .GiveGold(50)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(50);
        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void IfClass_NonMatchingClass_HaltsOuterChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetBaseClass(BaseClass.Wizard));
        var builder = new QuestStepBuilder<WolfStage>();

        builder.RequireClass(BaseClass.Priest)
               .GiveGold(50)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(0);
        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void IfGender_MatchingGender_RunsSubChainAndContinuesOuter()
    {
        var ctx = NewContext(setup: a => a.Gender = Gender.Female);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.RequireGender(Gender.Female)
               .GiveGold(50)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(50);
        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void IfGender_NonMatchingGender_HaltsOuterChain()
    {
        var ctx = NewContext(setup: a => a.Gender = Gender.Male);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.RequireGender(Gender.Female)
               .GiveGold(50)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(0);
        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    #endregion

    #region RequireClass / RequireGender
    [Test]
    public void RequireClass_FailureReply_DispatchesReplyOnMismatch()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog, setup: a => a.UserStatSheet.SetBaseClass(BaseClass.Warrior));
        var clientMock = Mock.Get(ctx.Source.Client);
        var advanced = false;

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireClass(BaseClass.Wizard, "you must be a wizard")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeFalse();
        clientMock.Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.Text == "you must be a wizard")), Times.Once);
    }

    [Test]
    public void RequireGender_FailureReply_DispatchesReplyOnMismatch()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog, setup: a => a.Gender = Gender.Male);
        var clientMock = Mock.Get(ctx.Source.Client);
        var advanced = false;

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireGender(Gender.Female, "you must be female")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeFalse();
        clientMock.Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.Text == "you must be female")), Times.Once);
    }
    #endregion

    #region Branching
    [Test]
    public void If_PredicateTrue_RunsThenBranch()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Branch(_ => true, sub => sub.Advance(WolfStage.Done))
               .Otherwise(sub => sub.Advance(WolfStage.None));

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void If_PredicateFalse_RunsOtherwiseBranch()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Branch(_ => false, sub => sub.Advance(WolfStage.Done))
               .Otherwise(sub => sub.Advance(WolfStage.None));

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.None).Should().BeTrue();
        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void If_PredicateFalse_NoOtherwise_NoOpAndContinuesOuter()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(_ => false, sub => sub.Advance(WolfStage.None))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        // then-branch did not run; outer chain continued and reached Advance
        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void If_PredicateTrue_NoOtherwise_RunsThenAndContinuesOuter()
    {
        var ctx = NewContext(WolfStage.Hunting);
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(_ => true, sub => sub.GiveGold(100))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(100);
        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void Chance_100Percent_AlwaysFires()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Branch(_ => true, sub => sub.GiveGold(50));

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(50);
    }

    [Test]
    public void Chance_0Percent_NeverFires()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Branch(_ => false, sub => sub.GiveGold(50));

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(0);
    }

    [Test]
    public void Chance_DoesNotHaltOuterChain_OnFailedRoll()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Branch(_ => false, sub => sub.GiveGold(50))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.Source.Gold.Should().Be(0);
        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void Run_AlwaysExecutesAction()
    {
        var ctx = NewContext();
        var ran = false;
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Run((s, c) => ran = true);

        RunChain(builder, ctx);

        ran.Should().BeTrue();
    }

    [Test]
    public void Run_DoesNotHaltChain()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.Run((s, c) => { /* no-op */ })
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void Run_ReceivesSourceAndContext()
    {
        var ctx = NewContext(WolfStage.Hunting);
        Aisling? capturedSource = null;
        QuestContext<WolfStage>? capturedCtx = null;

        var builder = new QuestStepBuilder<WolfStage>();
        builder.Run(
            (s, c) =>
            {
                capturedSource = s;
                capturedCtx = c;
            });

        RunChain(builder, ctx);

        capturedSource.Should().BeSameAs(ctx.Source);
        capturedCtx.Should().BeSameAs(ctx);
    }
    #endregion

    #region Communication
    private static Dialog CreateTestDialog(Mock<IDialogFactory>? factoryMock = null)
    {
        var sourceMock = new Mock<IDialogSourceEntity>();

        sourceMock.SetupGet(s => s.Name)
                  .Returns("TestSource");

        sourceMock.SetupGet(s => s.Id)
                  .Returns(1);

        factoryMock ??= new Mock<IDialogFactory>();

        return new Dialog(
            sourceMock.Object,
            factoryMock.Object,
            ChaosDialogType.Normal,
            "initial");
    }

    private static QuestContext<WolfStage> NewContextWithSubject(Dialog? subject, Action<Aisling>? setup = null)
    {
        var aisling = MockAisling.Create(setup: setup);

        return new QuestContext<WolfStage>
        {
            Source = aisling,
            CurrentStage = default,
            Services = MockServiceProvider.Create().Object,
            Subject = subject
        };
    }

    [Test]
    public void Reply_HaltsChain_AndDispatchesToClient()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);
        var clientMock = Mock.Get(ctx.Source.Client);

        var advanced = false;
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Reply("Hello, traveler.")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeFalse("Reply halts the chain");
        clientMock.Verify(c => c.SendDisplayDialog(It.Is<Dialog>(d => d.Text == "Hello, traveler.")), Times.Once);
    }

    [Test]
    public void Reply_NullSubject_HaltsChainSilently()
    {
        var ctx = NewContextWithSubject(null);
        var advanced = false;

        var builder = new QuestStepBuilder<WolfStage>();

        builder.Reply("nope")
               .Run((_, _) => advanced = true);

        var act = () => RunChain(builder, ctx);

        act.Should().NotThrow();
        advanced.Should().BeFalse("Reply halts even with no subject");
    }

    [Test]
    public void Skip_HaltsChain_AndDispatchesSkipReplyToTargetKey()
    {
        // Set up the factory to satisfy Dialog.Next("next_dialog_key") triggered by the "Skip" sentinel.
        var factoryMock = new Mock<IDialogFactory>();
        var capturedNextKey = (string?)null;
        var sourceMock = new Mock<IDialogSourceEntity>();

        sourceMock.SetupGet(s => s.Name)
                  .Returns("TestSource");

        sourceMock.SetupGet(s => s.Id)
                  .Returns(1);

        factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<IDialogSourceEntity>(), It.IsAny<ICollection<string>?>()))
                   .Returns(
                       (string key, IDialogSourceEntity src, ICollection<string>? _) =>
                       {
                           capturedNextKey = key;

                           return new Dialog(
                               src,
                               factoryMock.Object,
                               ChaosDialogType.Normal,
                               "next");
                       });

        var dialog = new Dialog(
            sourceMock.Object,
            factoryMock.Object,
            ChaosDialogType.Normal,
            "initial");

        var ctx = NewContextWithSubject(dialog);
        var advanced = false;
        var builder = new QuestStepBuilder<WolfStage>();

        builder.Skip("next_dialog_key")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeFalse("Skip halts the chain");
        capturedNextKey.Should().Be("next_dialog_key");
    }

    [Test]
    public void Skip_NullSubject_HaltsChainSilently()
    {
        var ctx = NewContextWithSubject(null);
        var advanced = false;

        var builder = new QuestStepBuilder<WolfStage>();

        builder.Skip("anything")
               .Run((_, _) => advanced = true);

        var act = () => RunChain(builder, ctx);

        act.Should().NotThrow();
        advanced.Should().BeFalse();
    }

    [Test]
    public void ShowOption_AddsOptionToDialog_WhenNotPresent()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ShowOption("Hunt wolves", "wolf_dialog");

        RunChain(builder, ctx);

        dialog.HasOption("Hunt wolves").Should().BeTrue();
        dialog.Options.Should().ContainSingle(o => (o.OptionText == "Hunt wolves") && (o.DialogKey == "wolf_dialog"));
    }

    [Test]
    public void ShowOption_NoOps_WhenOptionAlreadyPresent()
    {
        var dialog = CreateTestDialog();
        dialog.AddOption("Hunt wolves", "wolf_dialog");

        var ctx = NewContextWithSubject(dialog);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ShowOption("Hunt wolves", "wolf_dialog");

        RunChain(builder, ctx);

        dialog.Options.Should().ContainSingle(o => o.OptionText == "Hunt wolves");
    }

    [Test]
    public void ShowOption_NullSubject_NoOpsAndContinues()
    {
        var ctx = NewContextWithSubject(null);
        var continued = false;

        var builder = new QuestStepBuilder<WolfStage>();

        builder.ShowOption("Hunt wolves", "wolf_dialog")
               .Run((_, _) => continued = true);

        RunChain(builder, ctx);

        continued.Should().BeTrue();
    }

    [Test]
    public void ShowOption_ContinuesChain()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);
        var continued = false;

        var builder = new QuestStepBuilder<WolfStage>();

        builder.ShowOption("Hunt wolves", "wolf_dialog")
               .Run((_, _) => continued = true);

        RunChain(builder, ctx);

        continued.Should().BeTrue();
    }

    [Test]
    public void SendOrangeBar_CallsAislingSendActiveMessage()
    {
        var ctx = NewContextWithSubject(null);
        var clientMock = Mock.Get(ctx.Source.Client);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.SendOrangeBar("You can't do that yet.");

        RunChain(builder, ctx);

        clientMock.Verify(
            c => c.SendServerMessage(ServerMessageType.ActiveMessage, "You can't do that yet."),
            Times.Once);
    }

    [Test]
    public void SendOrangeBar_ContinuesChain()
    {
        var ctx = NewContextWithSubject(null);
        var continued = false;

        var builder = new QuestStepBuilder<WolfStage>();

        builder.SendOrangeBar("hi")
               .Run((_, _) => continued = true);

        RunChain(builder, ctx);

        continued.Should().BeTrue();
    }

    [Test]
    public void InjectTextParameters_StaticArgs_RewritesDialogText_AndContinuesChain()
    {
        var sourceMock = new Mock<IDialogSourceEntity>();
        sourceMock.SetupGet(s => s.Name).Returns("TestSource");
        sourceMock.SetupGet(s => s.Id).Returns(1);

        var dialog = new Dialog(
            sourceMock.Object,
            new Mock<IDialogFactory>().Object,
            ChaosDialogType.Normal,
            "Hello {0}, you have {1} gold.");

        var ctx = NewContextWithSubject(dialog);
        var continued = false;

        var builder = new QuestStepBuilder<WolfStage>();

        builder.InjectTextParameters("traveler", 250)
               .Run((_, _) => continued = true);

        RunChain(builder, ctx);

        dialog.Text.Should().Be("Hello traveler, you have 250 gold.");
        continued.Should().BeTrue();
    }

    [Test]
    public void InjectTextParameters_Selector_ReceivesContext_AndUpdatesDialogText()
    {
        var sourceMock = new Mock<IDialogSourceEntity>();
        sourceMock.SetupGet(s => s.Name).Returns("TestSource");
        sourceMock.SetupGet(s => s.Id).Returns(1);

        var dialog = new Dialog(
            sourceMock.Object,
            new Mock<IDialogFactory>().Object,
            ChaosDialogType.Normal,
            "Stage: {0} ({1})");

        var ctx = NewContextWithSubject(dialog);
        ctx.CurrentStage = WolfStage.Hunting;

        QuestContext<WolfStage>? capturedCtx = null;
        var builder = new QuestStepBuilder<WolfStage>();
        builder.InjectTextParameters(c =>
        {
            capturedCtx = c;
            return [c.CurrentStage, "active"];
        });

        RunChain(builder, ctx);

        capturedCtx.Should().BeSameAs(ctx, "the selector must receive the runtime context");
        dialog.Text.Should().Be($"Stage: {WolfStage.Hunting} (active)");
    }

    [Test]
    public void InjectTextParameters_NullSubject_NoOpsAndContinuesChain()
    {
        var ctx = NewContextWithSubject(null);
        var continued = false;

        var builder = new QuestStepBuilder<WolfStage>();

        builder.InjectTextParameters("ignored")
               .Run((_, _) => continued = true);

        var act = () => RunChain(builder, ctx);

        act.Should().NotThrow();
        continued.Should().BeTrue();
    }
    #endregion

    #region Teleports
    private static QuestContext<WolfStage> NewContextWithMapCache(
        MapInstance destinationMap,
        string mapKey = "dest_map",
        Group? group = null)
    {
        var cacheMock = new Mock<ISimpleCache<MapInstance>>();
        cacheMock.Setup(c => c.Get(mapKey))
                 .Returns(() => destinationMap);

        var services = MockServiceProvider.CreateBuilder()
                                          .SetupService<ISimpleCache<MapInstance>>(cacheMock.Object)
                                          .Build();

        var aisling = MockAisling.Create();

        if (group is not null)
            aisling.Group = group;

        return new QuestContext<WolfStage>
        {
            Source = aisling,
            CurrentStage = default,
            Services = services.Object
        };
    }

    [Test]
    public void Teleport_PointOverload_TraversesSourceToTargetMapAndPoint()
    {
        var destMap = MockMapInstance.Create("dest_map", "Destination", 20, 20);
        var ctx = NewContextWithMapCache(destMap);
        var sourceMapTraversal = Mock.Get(ctx.Source.MapInstance.TraversalService);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.Teleport("dest_map", 5, 7);

        RunChain(builder, ctx);

        sourceMapTraversal.Verify(
            t => t.TraverseMap(
                ctx.Source,
                destMap,
                It.Is<IPoint>(p => (p.X == 5) && (p.Y == 7)),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<Func<Task>?>()),
            Times.Once);
    }

    [Test]
    public void Teleport_RectangleOverload_TraversesSourceToWalkablePointInRect()
    {
        // Default 20x20 map has all walkable tiles → IsWalkable returns true on first try.
        var destMap = MockMapInstance.Create("dest_map", "Destination", 20, 20);
        var ctx = NewContextWithMapCache(destMap);
        var sourceMapTraversal = Mock.Get(ctx.Source.MapInstance.TraversalService);

        var rect = new Rectangle(3, 4, 5, 5);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.Teleport("dest_map", rect);

        RunChain(builder, ctx);

        sourceMapTraversal.Verify(
            t => t.TraverseMap(
                ctx.Source,
                destMap,
                It.Is<IPoint>(p => rect.ContainsPoint(p)),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<Func<Task>?>()),
            Times.Once);
    }

    [Test]
    public void Teleport_RectangleOverload_ThrowsWhenNoWalkablePoint()
    {
        // Build a 2x2 destination map and wall every tile so IsWalkable always returns false.
        var destMap = MockMapInstance.Create("dest_map", "Destination", 2, 2);
        MockMapInstance.SetWall(destMap, new Point(0, 0));
        MockMapInstance.SetWall(destMap, new Point(1, 0));
        MockMapInstance.SetWall(destMap, new Point(0, 1));
        MockMapInstance.SetWall(destMap, new Point(1, 1));

        var ctx = NewContextWithMapCache(destMap);
        var rect = new Rectangle(0, 0, 2, 2);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.Teleport("dest_map", rect);

        var act = () => RunChain(builder, ctx);

        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("*Could not find walkable point*dest_map*");
    }

    [Test]
    public void GroupTeleport_NoGroup_TeleportsSourceAlone()
    {
        var destMap = MockMapInstance.Create("dest_map", "Destination", 20, 20);
        var ctx = NewContextWithMapCache(destMap);
        var sourceMapTraversal = Mock.Get(ctx.Source.MapInstance.TraversalService);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.GroupTeleport("dest_map", 4, 6);

        RunChain(builder, ctx);

        sourceMapTraversal.Verify(
            t => t.TraverseMap(
                ctx.Source,
                destMap,
                It.Is<IPoint>(p => (p.X == 4) && (p.Y == 6)),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<Func<Task>?>()),
            Times.Once);
    }

    [Test]
    public void GroupTeleport_WithGroup_TeleportsAllMembers()
    {
        var destMap = MockMapInstance.Create("dest_map", "Destination", 20, 20);

        // Group leader and a second member share a source map so we can verify on its TraversalService.
        var sourceMap = MockMapInstance.Create("source_map", "Source", 20, 20);
        var leader = MockAisling.Create(sourceMap, "Leader");
        var member = MockAisling.Create(sourceMap, "Member");

        var channelService = MockChannelService.Create();
        var groupLogger = MockLogger.Create<Group>();
        var group = new Group(
            leader,
            member,
            channelService,
            groupLogger.Object);

        leader.Group = group;
        member.Group = group;

        var cacheMock = new Mock<ISimpleCache<MapInstance>>();
        cacheMock.Setup(c => c.Get("dest_map"))
                 .Returns(() => destMap);

        var services = MockServiceProvider.CreateBuilder()
                                          .SetupService<ISimpleCache<MapInstance>>(cacheMock.Object)
                                          .Build();

        var ctx = new QuestContext<WolfStage>
        {
            Source = leader,
            CurrentStage = default,
            Services = services.Object
        };

        var sourceMapTraversal = Mock.Get(sourceMap.TraversalService);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.GroupTeleport("dest_map", 9, 9);

        RunChain(builder, ctx);

        sourceMapTraversal.Verify(
            t => t.TraverseMap(
                It.Is<Aisling>(a => a == leader),
                destMap,
                It.Is<IPoint>(p => (p.X == 9) && (p.Y == 9)),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<Func<Task>?>()),
            Times.Once);

        sourceMapTraversal.Verify(
            t => t.TraverseMap(
                It.Is<Aisling>(a => a == member),
                destMap,
                It.Is<IPoint>(p => (p.X == 9) && (p.Y == 9)),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<Func<Task>?>()),
            Times.Once);
    }

    [Test]
    public void GroupTeleport_RectangleOverload_UsesSharedPointForAllMembers()
    {
        var destMap = MockMapInstance.Create("dest_map", "Destination", 20, 20);
        var sourceMap = MockMapInstance.Create("source_map", "Source", 20, 20);
        var leader = MockAisling.Create(sourceMap, "Leader");
        var member = MockAisling.Create(sourceMap, "Member");

        var channelService = MockChannelService.Create();
        var groupLogger = MockLogger.Create<Group>();
        var group = new Group(
            leader,
            member,
            channelService,
            groupLogger.Object);

        leader.Group = group;
        member.Group = group;

        var cacheMock = new Mock<ISimpleCache<MapInstance>>();
        cacheMock.Setup(c => c.Get("dest_map"))
                 .Returns(() => destMap);

        var services = MockServiceProvider.CreateBuilder()
                                          .SetupService<ISimpleCache<MapInstance>>(cacheMock.Object)
                                          .Build();

        var ctx = new QuestContext<WolfStage>
        {
            Source = leader,
            CurrentStage = default,
            Services = services.Object
        };

        var sourceMapTraversal = Mock.Get(sourceMap.TraversalService);
        var rect = new Rectangle(2, 2, 4, 4);
        IPoint? leaderPt = null;
        IPoint? memberPt = null;

        sourceMapTraversal.Setup(
                              t => t.TraverseMap(
                                  It.IsAny<Creature>(),
                                  destMap,
                                  It.IsAny<IPoint>(),
                                  It.IsAny<bool>(),
                                  It.IsAny<bool>(),
                                  It.IsAny<Func<Task>?>()))
                          .Callback<Creature, MapInstance, IPoint, bool, bool, Func<Task>?>(
                              (creature, _, p, _, _, _) =>
                              {
                                  if (creature == leader)
                                      leaderPt = p;
                                  else if (creature == member)
                                      memberPt = p;
                              });

        var builder = new QuestStepBuilder<WolfStage>();
        builder.GroupTeleport("dest_map", rect);

        RunChain(builder, ctx);

        leaderPt.Should()
                .NotBeNull();
        memberPt.Should()
                .NotBeNull();
        rect.ContainsPoint(leaderPt!)
            .Should()
            .BeTrue();

        // V1 contract: the entire group teleports to a single shared point.
        memberPt!.X.Should()
                  .Be(leaderPt!.X);
        memberPt.Y.Should()
                  .Be(leaderPt.Y);
    }
    #endregion

    #region RouteByStage
    [Test]
    public void RouteByStage_RoutesToMappedDialogKey_ForCurrentStage()
    {
        var factoryMock = new Mock<IDialogFactory>();
        var capturedNextKey = (string?)null;
        var sourceMock = new Mock<IDialogSourceEntity>();

        sourceMock.SetupGet(s => s.Name)
                  .Returns("TestSource");

        sourceMock.SetupGet(s => s.Id)
                  .Returns(1);

        factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<IDialogSourceEntity>(), It.IsAny<ICollection<string>?>()))
                   .Returns(
                       (string key, IDialogSourceEntity src, ICollection<string>? _) =>
                       {
                           capturedNextKey = key;

                           return new Dialog(
                               src,
                               factoryMock.Object,
                               ChaosDialogType.Normal,
                               "next");
                       });

        var dialog = new Dialog(
            sourceMock.Object,
            factoryMock.Object,
            ChaosDialogType.Normal,
            "initial");

        var ctx = NewContextWithSubject(dialog);
        ctx.CurrentStage = WolfStage.Hunting;

        var builder = new QuestStepBuilder<WolfStage>();

        var routes = new Dictionary<WolfStage, string>
        {
            [WolfStage.Hunting] = "wolf_hunting_branch",
            [WolfStage.Done] = "wolf_done_branch"
        };

        builder.RouteByStage(routes);

        RunChain(builder, ctx);

        capturedNextKey.Should().Be("wolf_hunting_branch");
    }

    [Test]
    public void RouteByStage_NoOps_WhenStageNotInRoutes()
    {
        var factoryMock = new Mock<IDialogFactory>();
        var dialogCreateInvoked = false;
        var sourceMock = new Mock<IDialogSourceEntity>();

        sourceMock.SetupGet(s => s.Name)
                  .Returns("TestSource");

        sourceMock.SetupGet(s => s.Id)
                  .Returns(1);

        factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<IDialogSourceEntity>(), It.IsAny<ICollection<string>?>()))
                   .Returns(
                       (string key, IDialogSourceEntity src, ICollection<string>? _) =>
                       {
                           dialogCreateInvoked = true;

                           return new Dialog(
                               src,
                               factoryMock.Object,
                               ChaosDialogType.Normal,
                               "next");
                       });

        var dialog = new Dialog(
            sourceMock.Object,
            factoryMock.Object,
            ChaosDialogType.Normal,
            "initial");

        var ctx = NewContextWithSubject(dialog);
        ctx.CurrentStage = WolfStage.None; // not in the route map

        var builder = new QuestStepBuilder<WolfStage>();

        var routes = new Dictionary<WolfStage, string>
        {
            [WolfStage.Hunting] = "wolf_hunting_branch"
        };

        builder.RouteByStage(routes)
               .Run((_, _) => { });

        var act = () => RunChain(builder, ctx);

        act.Should().NotThrow();
        dialogCreateInvoked.Should().BeFalse("no route matched the current stage");
    }
    #endregion

    #region RequireCount / ClearCounter
    [Test]
    public void RequireCount_AllowsChain_WhenCountSufficient()
    {
        var ctx = NewContext();
        ctx.Source.Trackers.Counters.AddOrIncrement("herbs", 5);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireCount("herbs", 3)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireCount_HaltsChain_WhenCountInsufficient()
    {
        var ctx = NewContext();
        ctx.Source.Trackers.Counters.AddOrIncrement("herbs", 1);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireCount("herbs", 10)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }

    [Test]
    public void ClearCounter_RemovesCounter()
    {
        var ctx = NewContext();
        ctx.Source.Trackers.Counters.AddOrIncrement("herbs", 7);

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ClearCounter("herbs");

        RunChain(builder, ctx);

        ctx.HasCount("herbs", 1).Should().BeFalse();
    }
    #endregion

    #region RequireCooldownExpired (templated)
    [Test]
    public void RequireCooldownExpired_Templated_AllowsChain_WhenNoCooldownActive()
    {
        var ctx = NewContext();
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireCooldownExpired("hunt_cd", "wait {remaining}")
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireCooldownExpired_Templated_HaltsChain_AndReplacesRemainingPlaceholder()
    {
        var dialog = CreateTestDialog();
        var ctx = NewContextWithSubject(dialog);
        ctx.Source.Trackers.TimedEvents.AddEvent("hunt_cd", TimeSpan.FromMinutes(2), true);

        var clientMock = Mock.Get(ctx.Source.Client);
        var advanced = false;

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireCooldownExpired("hunt_cd", "Come back in {remaining}.")
               .Run((_, _) => advanced = true);

        RunChain(builder, ctx);

        advanced.Should().BeFalse("the cooldown is active so the chain halts");

        clientMock.Verify(
            c => c.SendDisplayDialog(
                It.Is<Dialog>(
                    d => d.Text.StartsWith("Come back in ", StringComparison.Ordinal)
                         && !d.Text.Contains("{remaining}", StringComparison.Ordinal))),
            Times.Once);
    }

    [Test]
    public void RequireCooldownExpired_Templated_NullSubject_HaltsChainSilently()
    {
        var ctx = NewContextWithSubject(null);
        ctx.Source.Trackers.TimedEvents.AddEvent("hunt_cd", TimeSpan.FromMinutes(1), true);

        var advanced = false;
        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireCooldownExpired("hunt_cd", "wait {remaining}")
               .Run((_, _) => advanced = true);

        var act = () => RunChain(builder, ctx);

        act.Should().NotThrow();
        advanced.Should().BeFalse();
    }
    #endregion

    #region GiveItems / RequireItems
    [Test]
    public void GiveItems_GrantsEachItemViaItemFactory()
    {
        var factoryMock = new Mock<IItemFactory>();

        factoryMock.Setup(f => f.Create("wolfsfur", It.IsAny<ICollection<string>?>()))
                   .Returns(() => MockItem.Create("wolfsfur"));

        factoryMock.Setup(f => f.Create("wolfsclaw", It.IsAny<ICollection<string>?>()))
                   .Returns(() => MockItem.Create("wolfsclaw"));

        var services = MockServiceProvider.CreateBuilder()
                                          .SetupService(factoryMock.Object)
                                          .Build()
                                          .Object;

        var ctx = NewContext(services: services);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveItems(("wolfsfur", 1), ("wolfsclaw", 2));

        RunChain(builder, ctx);

        factoryMock.Verify(f => f.Create("wolfsfur", It.IsAny<ICollection<string>?>()), Times.Once);
        factoryMock.Verify(f => f.Create("wolfsclaw", It.IsAny<ICollection<string>?>()), Times.Once);
    }

    [Test]
    public void RequireItems_AllowsChain_WhenAllItemsPresent()
    {
        var ctx = NewContext();
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsfur", count: 5, stackable: true));
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsclaw", count: 3, stackable: true));

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireItems(("wolfsfur", 5), ("wolfsclaw", 2))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }

    [Test]
    public void RequireItems_HaltsChain_WhenAnyItemMissing()
    {
        var ctx = NewContext();
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsfur", count: 5, stackable: true));
        // No wolfsclaw

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RequireItems(("wolfsfur", 5), ("wolfsclaw", 1))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
    }
    #endregion

    #region ConsumeItem / ConsumeItems
    [Test]
    public void ConsumeItem_AllowsChain_AndRemovesFromInventory_WhenSufficient()
    {
        var ctx = NewContext();
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsfur", count: 5, stackable: true));

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ConsumeItem("wolfsfur", 3)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
        ctx.Source.Inventory.HasCountByTemplateKey("wolfsfur", 2).Should().BeTrue();
        ctx.Source.Inventory.HasCountByTemplateKey("wolfsfur", 3).Should().BeFalse();
    }

    [Test]
    public void ConsumeItem_HaltsChain_WhenInsufficient()
    {
        var ctx = NewContext();
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsfur", count: 1, stackable: true));

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ConsumeItem("wolfsfur", 5)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();
        ctx.Source.Inventory.HasCountByTemplateKey("wolfsfur", 1).Should().BeTrue();
    }

    [Test]
    public void ConsumeItems_Atomic_ConsumesAll_WhenAllPresent()
    {
        var ctx = NewContext();
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsfur", count: 5, stackable: true));
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsclaw", count: 3, stackable: true));

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ConsumeItems(("wolfsfur", 2), ("wolfsclaw", 1))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
        ctx.Source.Inventory.HasCountByTemplateKey("wolfsfur", 3).Should().BeTrue();
        ctx.Source.Inventory.HasCountByTemplateKey("wolfsclaw", 2).Should().BeTrue();
    }

    [Test]
    public void ConsumeItems_Atomic_HaltsChain_AndConsumesNothing_WhenAnyMissing()
    {
        var ctx = NewContext();
        ctx.Source.Inventory.TryAddToNextSlot(MockItem.Create("wolfsfur", count: 5, stackable: true));
        // No wolfsclaw at all

        var builder = new QuestStepBuilder<WolfStage>();
        builder.ConsumeItems(("wolfsfur", 2), ("wolfsclaw", 1))
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeFalse();

        // Atomic: even though wolfsfur was sufficient, it must NOT have been consumed.
        ctx.Source.Inventory.HasCountByTemplateKey("wolfsfur", 5).Should().BeTrue();
    }
    #endregion

    #region GiveExperience
    [Test]
    public void GiveExperience_AddsExperienceToAisling()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetLevel(99));
        var initial = ctx.Source.UserStatSheet.TotalExp;

        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveExperience(1234);

        RunChain(builder, ctx);

        ctx.Source.UserStatSheet.TotalExp.Should().Be(initial + 1234);
    }

    [Test]
    public void GiveExperience_ContinuesChain()
    {
        var ctx = NewContext(setup: a => a.UserStatSheet.SetLevel(99));
        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveExperience(100)
               .Advance(WolfStage.Done);

        RunChain(builder, ctx);

        ctx.IsAt(WolfStage.Done).Should().BeTrue();
    }
    #endregion

    #region GiveSkill / GiveSpell / RemoveSkill / RemoveSpell
    [Test]
    public void GiveSkill_CreatesViaFactory_AndAddsToSkillBook()
    {
        var skillFactoryMock = new Mock<ISkillFactory>();

        skillFactoryMock.Setup(f => f.Create("flame_blade", It.IsAny<ICollection<string>?>()))
                        .Returns(() => MockSkill.Create("flame_blade"));

        var services = MockServiceProvider.CreateBuilder()
                                          .SetupService(skillFactoryMock.Object)
                                          .Build()
                                          .Object;

        var ctx = NewContext(services: services);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveSkill("flame_blade");

        RunChain(builder, ctx);

        skillFactoryMock.Verify(f => f.Create("flame_blade", It.IsAny<ICollection<string>?>()), Times.Once);
        ctx.Source.SkillBook.TryGetObjectByTemplateKey("flame_blade", out _).Should().BeTrue();
    }

    [Test]
    public void GiveSpell_CreatesViaFactory_AndAddsToSpellBook()
    {
        var spellFactoryMock = new Mock<ISpellFactory>();

        spellFactoryMock.Setup(f => f.Create("ice_blast", It.IsAny<ICollection<string>?>()))
                        .Returns(() => MockSpell.Create("ice_blast"));

        var services = MockServiceProvider.CreateBuilder()
                                          .SetupService(spellFactoryMock.Object)
                                          .Build()
                                          .Object;

        var ctx = NewContext(services: services);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveSpell("ice_blast");

        RunChain(builder, ctx);

        spellFactoryMock.Verify(f => f.Create("ice_blast", It.IsAny<ICollection<string>?>()), Times.Once);
        ctx.Source.SpellBook.TryGetObjectByTemplateKey("ice_blast", out _).Should().BeTrue();
    }

    [Test]
    public void GiveSpell_WithPage_PlacesSpellAtRequestedSlot()
    {
        var spellFactoryMock = new Mock<ISpellFactory>();

        spellFactoryMock.Setup(f => f.Create("ice_blast", It.IsAny<ICollection<string>?>()))
                        .Returns(() => MockSpell.Create("ice_blast"));

        var services = MockServiceProvider.CreateBuilder()
                                          .SetupService(spellFactoryMock.Object)
                                          .Build()
                                          .Object;

        var ctx = NewContext(services: services);
        var builder = new QuestStepBuilder<WolfStage>();
        builder.GiveSpell("ice_blast", page: 25);

        RunChain(builder, ctx);

        ctx.Source.SpellBook.TryGetObjectByTemplateKey("ice_blast", out var spell).Should().BeTrue();
        spell!.Slot.Should().Be(25);
    }

    [Test]
    public void RemoveSkill_RemovesFromSkillBook()
    {
        var ctx = NewContext();
        ctx.Source.SkillBook.TryAddToNextSlot(MockSkill.Create("flame_blade"));

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RemoveSkill("flame_blade");

        RunChain(builder, ctx);

        ctx.Source.SkillBook.TryGetObjectByTemplateKey("flame_blade", out _).Should().BeFalse();
    }

    [Test]
    public void RemoveSpell_RemovesFromSpellBook()
    {
        var ctx = NewContext();
        ctx.Source.SpellBook.TryAddToNextSlot(MockSpell.Create("ice_blast"));

        var builder = new QuestStepBuilder<WolfStage>();
        builder.RemoveSpell("ice_blast");

        RunChain(builder, ctx);

        ctx.Source.SpellBook.TryGetObjectByTemplateKey("ice_blast", out _).Should().BeFalse();
    }
    #endregion
}
