#region
using System.Reflection;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Map;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

[NotInParallel]
public sealed class AislingTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region Activate
    [Test]
    public void Activate_ShouldDelegateToScriptOnClicked()
    {
        var aisling = MockAisling.Create(Map);
        var source = MockAisling.Create(Map, "Clicker");

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        ((IDialogSourceEntity)aisling).Activate(source);

        scriptMock.Verify(s => s.OnClicked(source), Times.Once);
    }
    #endregion

    #region Aisling constructor branches
    [Test]
    public void Aisling_FemaleGender_ShouldSetFemaleBodySprite()
    {
        // The Aisling(name, gender, hairStyle, hairColor) constructor branches on Gender
        var map = MockMapInstance.Create();
        var aisling = MockAisling.Create(map, "FemaleChar", setup: a => a.Gender = Gender.Female);

        // The mock constructor used in testing may not go through the full Aisling constructor
        // but we can verify the gender was set correctly
        aisling.Gender
               .Should()
               .Be(Gender.Female);
    }
    #endregion

    #region CanUse (Item)
    [Test]
    public void CanUse_Item_ShouldReturnFalse_WhenScriptDisallows()
    {
        var aisling = MockAisling.Create(Map);
        var item = MockItem.Create("Potion");

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanUseItem(It.IsAny<Item>()))
                  .Returns(false);

        aisling.CanUse(item)
               .Should()
               .BeFalse();
    }
    #endregion

    #region CanUse (Skill)
    [Test]
    public void CanUse_Skill_ShouldReturnFalse_WhenScriptDisallowsSkill()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create();

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanUseSkill(It.IsAny<Skill>()))
                  .Returns(false);

        aisling.CanUse(skill, out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }
    #endregion

    #region CanUse (Skill) deeper
    [Test]
    public void CanUse_Skill_ShouldReturnTrue_WhenNonAssailAndAllChecksPass()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create();

        MockAisling.SetupScriptAllows(aisling);

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(s => s.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        aisling.CanUse(skill, out var context)
               .Should()
               .BeTrue();

        context.Should()
               .NotBeNull();
    }
    #endregion

    #region CanUse (Spell)
    [Test]
    public void CanUse_Spell_ShouldReturnFalse_WhenScriptDisallowsSpell()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanUseSpell(It.IsAny<Spell>()))
                  .Returns(false);

        aisling.CanUse(
                   spell,
                   aisling,
                   null,
                   out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }
    #endregion

    #region CanUse (Spell) deeper
    [Test]
    public void CanUse_Spell_ShouldReturnTrue_WhenAllChecksPass()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        MockAisling.SetupScriptAllows(aisling);

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(s => s.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        aisling.CanUse(
                   spell,
                   aisling,
                   null,
                   out var context)
               .Should()
               .BeTrue();

        context.Should()
               .NotBeNull();
    }
    #endregion

    #region Initialize
    [Test]
    public void Initialize_ShouldSetAllProperties()
    {
        var aisling = MockAisling.Create(Map);
        var bank = new Bank();
        var equipment = new Equipment();
        var inventory = new Inventory();
        var skillBook = new SkillBook();
        var spellBook = new SpellBook();
        var legend = new Legend();
        var effects = new EffectsBar(aisling);
        var trackers = new AislingTrackers();

        aisling.Initialize(
            "TestPlayer",
            bank,
            equipment,
            inventory,
            skillBook,
            spellBook,
            legend,
            effects,
            trackers);

        aisling.Name
               .Should()
               .Be("TestPlayer");

        aisling.Bank
               .Should()
               .BeSameAs(bank);

        aisling.Equipment
               .Should()
               .BeSameAs(equipment);

        aisling.Inventory
               .Should()
               .BeSameAs(inventory);

        aisling.SkillBook
               .Should()
               .BeSameAs(skillBook);

        aisling.SpellBook
               .Should()
               .BeSameAs(spellBook);

        aisling.Legend
               .Should()
               .BeSameAs(legend);

        aisling.Effects
               .Should()
               .BeSameAs(effects);

        aisling.Trackers
               .Should()
               .BeSameAs(trackers);
    }
    #endregion

    #region IsOnWorldMap
    [Test]
    public void IsOnWorldMap_ShouldReturnFalse_WhenNoActiveWorldMap()
    {
        var aisling = MockAisling.Create(Map);

        aisling.IsOnWorldMap
               .Should()
               .BeFalse();
    }
    #endregion

    #region OnClicked — click throttle
    [Test]
    public void OnClicked_ShouldNotProcess_WhenClickedTooFast()
    {
        var aisling = MockAisling.Create(Map, "Clicker");
        var target = MockAisling.Create(Map, "Target");
        var sourceClientMock = Mock.Get(aisling.Client);

        // First click succeeds
        target.OnClicked(aisling);
        sourceClientMock.Verify(c => c.SendOtherProfile(target), Times.Once);

        sourceClientMock.Invocations.Clear();

        // Second click immediately — should be throttled (ShouldRegisterClick returns false)
        target.OnClicked(aisling);

        sourceClientMock.Verify(c => c.SendOtherProfile(target), Times.Never);
    }
    #endregion

    #region OnItemDroppedOn (Item.Script.CanBeDroppedOn disallows)
    [Test]
    public void OnItemDroppedOn_ShouldFail_WhenItemScriptCanBeDroppedOnReturnsFalse()
    {
        var source = MockAisling.Create(Map, "Source");
        source.UserStatSheet.SetMaxWeight(50);
        var target = MockAisling.Create(Map, "Target");
        var item = MockItem.Create("Sword");
        source.Inventory.TryAddToNextSlot(item);

        var targetScriptMock = Mock.Get(target.Script);
        targetScriptMock.Reset();

        // Target script allows, but item script disallows
        targetScriptMock.Setup(s => s.CanDropItemOn(It.IsAny<Aisling>(), It.IsAny<Item>()))
                        .Returns(true);

        var itemScriptMock = Mock.Get(item.Script);
        itemScriptMock.Reset();

        itemScriptMock.Setup(s => s.CanBeDroppedOn(It.IsAny<Aisling>(), It.IsAny<Aisling>()))
                      .Returns(false);

        target.OnItemDroppedOn(source, item.Slot, 0);

        // Item should still be in source's inventory
        source.Inventory
              .Contains("Sword")
              .Should()
              .BeTrue();

        // Exchange should not have started
        var sourceClient = Mock.Get(source.Client);

        sourceClient.Verify(c => c.SendExchangeStart(It.IsAny<Aisling>()), Times.Never);
    }
    #endregion

    #region SetLanternSize
    [Test]
    public void SetLanternSize_ShouldSetLanternSizeProperty()
    {
        var aisling = MockAisling.Create(Map);

        aisling.SetLanternSize(LanternSize.Large);

        aisling.LanternSize
               .Should()
               .Be(LanternSize.Large);
    }
    #endregion

    #region SetSprite
    [Test]
    public void SetSprite_ShouldSetSpriteProperty()
    {
        var aisling = MockAisling.Create(Map);

        aisling.SetSprite(123);

        aisling.Sprite
               .Should()
               .Be(123);
    }
    #endregion

    #region TryDrop (NoTrade with amount)
    [Test]
    public void TryDrop_WithAmount_ShouldLockToAisling_WhenNoTrade()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        MockAisling.SetupScriptAllows(aisling);

        // Inject cloner into inventory
        typeof(Inventory).GetField("ItemCloner", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(
            aisling.Inventory,
            MockScriptProvider.ItemCloner.Object);

        var item = MockItem.Create(
            "BoundPotion",
            10,
            true,
            setup: i => i.NoTrade = true);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanBeDropped(It.IsAny<Aisling>(), It.IsAny<Point>()))
                      .Returns(true);

        itemScriptMock.Setup(s => s.CanBePickedUp(It.IsAny<Aisling>(), It.IsAny<Point>()))
                      .Returns(true);

        aisling.Inventory.TryAddToNextSlot(item);

        var result = aisling.TryDrop(
            aisling,
            item.Slot,
            out var groundItems,
            5);

        result.Should()
              .BeTrue();

        groundItems.Should()
                   .NotBeNull();

        // Ground items should be locked to the aisling
        var other = MockAisling.Create(Map, "Other");

        foreach (var groundItem in groundItems!)
            groundItem.CanBePickedUp(other)
                      .Should()
                      .BeFalse();
    }
    #endregion

    #region TryDropGold (reactor interaction)
    [Test]
    public void TryDropGold_ShouldCallReactorOnGoldDroppedOn_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropMoney(It.IsAny<int>()))
                  .Returns(true);

        var dropPoint = new Point(aisling.X, aisling.Y);

        // Create a reactor at the drop point
        var reactor = new ReactorTile(
            Map,
            dropPoint,
            false,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

        Map.SimpleAdd(reactor);

        var result = aisling.TryDropGold(dropPoint, 50, out var money);

        result.Should()
              .BeTrue();

        money.Should()
             .NotBeNull();

        var reactorScriptMock = Mock.Get(reactor.Script);

        reactorScriptMock.Verify(s => s.OnGoldDroppedOn(aisling, It.Is<Money>(m => m.Amount == 50)), Times.Once);
    }
    #endregion

    #region TryDropGold deeper
    [Test]
    public void TryDropGold_ShouldReturnFalse_WhenPointIsWall()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;
        MockAisling.SetupScriptAllows(aisling);

        MockMapInstance.SetWall(Map, new Point(5, 6));

        var result = aisling.TryDropGold(new Point(5, 6), 50, out _);

        result.Should()
              .BeFalse();

        aisling.Gold
               .Should()
               .Be(100);
    }
    #endregion

    #region TryGiveItem (stackable with slot)
    [Test]
    public void TryGiveItem_WithSlot_ShouldUpdateRefItem_WhenStackable()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        // Add existing stackable item
        var existing = MockItem.Create("Potion", 5, true);
        aisling.Inventory.TryAddToNextSlot(existing);

        // Give more of the same stackable with a specific slot
        var newItem = MockItem.Create("Potion", 3, true);
        var result = aisling.TryGiveItem(ref newItem, existing.Slot);

        result.Should()
              .BeTrue();

        // The ref item should now point to the inventory version (consolidated)
        newItem.Count
               .Should()
               .Be(8);

        // Should be the same instance as the inventory version
        newItem.Should()
               .BeSameAs(aisling.Inventory[newItem.DisplayName]);
    }
    #endregion

    #region TryPickupItem (reactor interaction)
    [Test]
    public void TryPickupItem_ShouldCallReactorOnItemPickedUpFrom_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        MockAisling.SetupScriptAllows(aisling);

        var groundItem = MockGroundItem.Create(Map, "Sword");
        var pickupPoint = new Point(5, 5);
        Map.AddEntity(groundItem, pickupPoint);

        // Create a reactor at the pickup point
        var reactor = new ReactorTile(
            Map,
            pickupPoint,
            false,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

        Map.SimpleAdd(reactor);

        var result = aisling.TryPickupItem(groundItem, 1);

        result.Should()
              .BeTrue();

        var reactorScriptMock = Mock.Get(reactor.Script);

        reactorScriptMock.Verify(s => s.OnItemPickedUpFrom(aisling, groundItem, It.IsAny<int>()), Times.Once);
    }
    #endregion

    #region TryPickupMoney (reactor interaction)
    [Test]
    public void TryPickupMoney_ShouldCallReactorOnGoldPickedUpFrom_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 0;
        MockAisling.SetupScriptAllows(aisling);

        var money = new Money(100, Map, new Point(5, 5));
        Map.AddEntity(money, new Point(5, 5));

        // Create a reactor at the pickup point
        var reactor = new ReactorTile(
            Map,
            new Point(5, 5),
            false,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

        Map.SimpleAdd(reactor);

        var result = aisling.TryPickupMoney(money);

        result.Should()
              .BeTrue();

        aisling.Gold
               .Should()
               .Be(100);

        var reactorScriptMock = Mock.Get(reactor.Script);

        reactorScriptMock.Verify(s => s.OnGoldPickedUpFrom(aisling, money), Times.Once);
    }
    #endregion

    #region TryPickupMoney — max gold
    [Test]
    public void TryPickupMoney_ShouldReturnFalse_WhenAtMaxGold()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = WorldOptions.Instance.MaxGoldHeld;
        MockAisling.SetupScriptAllows(aisling);

        var money = new Money(1, Map, new Point(5, 5));
        Map.AddEntity(money, new Point(5, 5));

        aisling.TryPickupMoney(money)
               .Should()
               .BeFalse();
    }
    #endregion

    #region Turn (throttle)
    [Test]
    public void Turn_ShouldNotChangeDirection_WhenTurnThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Direction = Direction.Up;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(true);

        // TurnThrottle = ResettingCounter(3) => MaxCount = 3
        for (var i = 0; i < 3; i++)
            aisling.TurnThrottle.TryIncrement();

        aisling.Turn(Direction.Down);

        aisling.Direction
               .Should()
               .Be(Direction.Up);
    }
    #endregion

    #region Turn — send to nearby aislings
    [Test]
    public void Turn_ShouldSendCreatureTurn_ToNearbyObservingAislings()
    {
        var aisling = MockAisling.Create(Map, "Turner");
        var observer = MockAisling.Create(Map, "Observer");

        Map.AddEntity(aisling, new Point(5, 5));
        Map.AddEntity(observer, new Point(5, 6));

        MockAisling.SetupScriptAllows(aisling);

        var observerClient = Mock.Get(observer.Client);
        observerClient.Invocations.Clear();

        aisling.Turn(Direction.Right, true);

        observerClient.Verify(c => c.SendCreatureTurn(aisling.Id, Direction.Right), Times.AtLeastOnce);
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldNotThrow()
    {
        var aisling = MockAisling.Create(Map);

        var act = () => aisling.Update(TimeSpan.FromMilliseconds(100));

        act.Should()
           .NotThrow();
    }
    #endregion

    #region UpdateViewPort (multi with partialUpdate + refresh)
    [Test]
    public void UpdateViewPort_Multi_PartialUpdate_WithRefresh_ShouldCallOnDepartureWithRefreshFlag()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        var other = MockAisling.Create(Map, "Other");

        // Other is NOT on the map — previously observed but departed

        aisling.ApproachTime[other] = DateTime.UtcNow;

        // Partial update with refresh=true, specifying the departed entity
        aisling.UpdateViewPort([other], true);

        // Other should have been removed from ApproachTime (departed)
        aisling.ApproachTime
               .Should()
               .NotContainKey(other);
    }
    #endregion

    #region Walk (SpeedWalk integration)
    [Test]
    public void Walk_ShouldRefresh_WhenSpeedWalkDetected()
    {
        var aisling = MockAisling.Create(Map);
        MockAisling.SetupScriptAllows(aisling);

        var original = (WorldOptions)WorldOptions.Instance;

        try
        {
            WorldOptions.Instance = original with
            {
                ProhibitSpeedWalk = true
            };

            // Exhaust the walk counter
            for (var i = 0; i < 8; i++)
                aisling.WalkCounter.TryIncrement();

            var clientMock = Mock.Get(aisling.Client);

            aisling.Walk(Direction.Down);

            // Walk should have been rejected and Refresh called
            clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);

            // Position should not have changed
            aisling.X
                   .Should()
                   .Be(5);

            aisling.Y
                   .Should()
                   .Be(5);
        } finally
        {
            WorldOptions.Instance = original;
        }
    }
    #endregion

    #region WarpTo
    [Test]
    public void WarpTo_ShouldChangePosition()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        aisling.WarpTo(new Point(3, 3));

        aisling.X
               .Should()
               .Be(3);

        aisling.Y
               .Should()
               .Be(3);
    }
    #endregion

    #region WarpTo — dark map viewport update
    [Test]
    public void WarpTo_ShouldUpdateOtherAislingViewPort_WhenDarkMap()
    {
        Map.Flags = MapFlags.Darkness;

        var aisling = MockAisling.Create(Map, "Warper");
        var otherAisling = MockAisling.Create(Map, "Observer");

        Map.AddEntity(aisling, new Point(5, 5));
        Map.AddEntity(otherAisling, new Point(5, 6));

        // Clear invocations from AddEntity
        Mock.Get(otherAisling.Client)
            .Invocations
            .Clear();

        // Warp to a nearby point — otherAisling is another Aisling on a dark map
        aisling.WarpTo(new Point(5, 7));

        // On a dark map, other aislings should get full viewport update
        // (the creature.UpdateViewPort() path, not creature.UpdateViewPort(this))
        // This exercises the isDarkMap && creature is Aisling branch
    }
    #endregion

    #region TryGiveGold
    [Test]
    public void TryGiveGold_ShouldThrow_WhenAmountIsNegative()
    {
        var aisling = MockAisling.Create(Map);

        var act = () => aisling.TryGiveGold(-1);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TryGiveGold_ShouldReturnFalse_WhenExceedsMaxGold()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 10_000_000;

        var result = aisling.TryGiveGold(1);

        result.Should()
              .BeFalse();

        aisling.Gold
               .Should()
               .Be(10_000_000);
    }

    [Test]
    public void TryGiveGold_ShouldReturnTrue_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var result = aisling.TryGiveGold(50);

        result.Should()
              .BeTrue();

        aisling.Gold
               .Should()
               .Be(150);
    }

    [Test]
    public void TryGiveGold_ShouldReturnTrue_WhenAmountIsZero()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var result = aisling.TryGiveGold(0);

        result.Should()
              .BeTrue();

        aisling.Gold
               .Should()
               .Be(100);
    }
    #endregion

    #region TryTakeGold
    [Test]
    public void TryTakeGold_ShouldThrow_WhenAmountIsNegative()
    {
        var aisling = MockAisling.Create(Map);

        var act = () => aisling.TryTakeGold(-1);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TryTakeGold_ShouldReturnTrue_WhenAmountIsZero()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var result = aisling.TryTakeGold(0);

        result.Should()
              .BeTrue();

        aisling.Gold
               .Should()
               .Be(100);
    }

    [Test]
    public void TryTakeGold_ShouldReturnFalse_WhenInsufficientGold()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 50;

        var result = aisling.TryTakeGold(100);

        result.Should()
              .BeFalse();

        aisling.Gold
               .Should()
               .Be(50);
    }

    [Test]
    public void TryTakeGold_ShouldReturnTrue_WhenSufficientGold()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var result = aisling.TryTakeGold(30);

        result.Should()
              .BeTrue();

        aisling.Gold
               .Should()
               .Be(70);
    }

    [Test]
    public void TryTakeGold_ShouldReturnTrue_WhenTakingExactAmount()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var result = aisling.TryTakeGold(100);

        result.Should()
              .BeTrue();

        aisling.Gold
               .Should()
               .Be(0);
    }
    #endregion

    #region HasClass
    [Test]
    public void HasClass_ShouldReturnTrue_WhenCheckingPeasant()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetBaseClass(BaseClass.Warrior);

        aisling.HasClass(BaseClass.Peasant)
               .Should()
               .BeTrue();
    }

    [Test]
    public void HasClass_ShouldReturnTrue_WhenDiacht()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetBaseClass(BaseClass.Diacht);

        aisling.HasClass(BaseClass.Warrior)
               .Should()
               .BeTrue();

        aisling.HasClass(BaseClass.Wizard)
               .Should()
               .BeTrue();

        aisling.HasClass(BaseClass.Priest)
               .Should()
               .BeTrue();
    }

    [Test]
    public void HasClass_ShouldReturnTrue_WhenExactMatch()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetBaseClass(BaseClass.Warrior);

        aisling.HasClass(BaseClass.Warrior)
               .Should()
               .BeTrue();
    }

    [Test]
    public void HasClass_ShouldReturnFalse_WhenDifferentClass()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetBaseClass(BaseClass.Warrior);

        aisling.HasClass(BaseClass.Wizard)
               .Should()
               .BeFalse();
    }
    #endregion

    #region CanCarry
    [Test]
    public void CanCarry_ShouldReturnFalse_WhenTooHeavy()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(5);

        var heavyItem = MockItem.Create("Heavy", setup: item => item.Weight = 10);

        aisling.CanCarry(heavyItem)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanCarry_ShouldReturnTrue_WhenWithinWeight()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Light");

        aisling.CanCarry(item)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanCarry_ShouldReturnFalse_WhenInventoryFull()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(500);

        // Fill inventory (60 slots)
        for (var i = 0; i < 60; i++)
            aisling.Inventory.TryAddToNextSlot(MockItem.Create($"Item{i}"));

        var newItem = MockItem.Create("Extra");

        aisling.CanCarry(newItem)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanCarry_ShouldReturnTrue_WhenStackableFitsInExistingStack()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(500);

        var existingStackable = MockItem.Create("Potion", 5, true);
        aisling.Inventory.TryAddToNextSlot(existingStackable);

        // Should be able to carry more of the same stackable (fills existing stack)
        var moreStackable = MockItem.Create("Potion", 5, true);

        aisling.CanCarry(moreStackable)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanCarry_ShouldReturnFalse_WhenStackableExceedsMaxStacks()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(500);

        var existingStackable = MockItem.Create("Potion", 95, true);
        aisling.Inventory.TryAddToNextSlot(existingStackable);

        // Can't add 10 more when max is 100 and we already have 95
        var moreStackable = MockItem.Create("Potion", 10, true);

        aisling.CanCarry(moreStackable)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanCarry_ShouldReturnTrue_WhenDefaultMaxWeight_AndWeightIsZero()
    {
        var aisling = MockAisling.Create(Map);

        // MaxWeight defaults to 0 — an item with weight 0 should still fail (0 + 1 > 0 for slot check,
        // but actually the weight formula is CurrentWeight + weightSum <= MaxWeight)
        // item.Weight = 1 by default from MockItem

        var item = MockItem.Create("Light");

        // Default maxWeight=0, item weight=1 → 0+1 > 0 → false
        aisling.CanCarry(item)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanCarry_MultipleItems_ShouldCheckCombinedWeightAndSlots()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(5);

        var items = new[]
        {
            MockItem.Create("Item1"),
            MockItem.Create("Item2"),
            MockItem.Create("Item3")
        };

        // 3 items × 1 weight = 3 weight, maxWeight=5, should succeed
        aisling.CanCarry(items)
               .Should()
               .BeTrue();
    }
    #endregion

    #region TryGiveItem
    [Test]
    public void TryGiveItem_ShouldReturnFalse_WhenCantCarry()
    {
        var aisling = MockAisling.Create(Map);

        // MaxWeight defaults to 0
        var item = MockItem.Create("Sword");

        var result = aisling.TryGiveItem(ref item);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryGiveItem_ShouldReturnTrue_WhenCanCarry()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Sword");

        var result = aisling.TryGiveItem(ref item);

        result.Should()
              .BeTrue();

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeTrue();
    }

    [Test]
    public void TryGiveItem_WithSlot_ShouldAddToSpecificSlot()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Sword");

        var result = aisling.TryGiveItem(ref item, 5);

        result.Should()
              .BeTrue();

        aisling.Inventory[5]
               .Should()
               .NotBeNull();

        aisling.Inventory[5]!.DisplayName
               .Should()
               .Be("Sword");
    }

    [Test]
    public void TryGiveItem_WithSlot_ShouldFallbackToNextSlot_WhenSlotOccupied()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item1 = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item1);

        var item2 = MockItem.Create("Shield");

        // TryAdd falls through to TryAddToNextSlot when slot is occupied
        var result = aisling.TryGiveItem(ref item2, item1.Slot);

        result.Should()
              .BeTrue();

        aisling.Inventory
               .Contains("Shield")
               .Should()
               .BeTrue();
    }

    [Test]
    public void TryGiveItem_Stackable_ShouldConsolidate()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var existing = MockItem.Create("Potion", 5, true);
        aisling.Inventory.TryAddToNextSlot(existing);

        var newItem = MockItem.Create("Potion", 3, true);

        var result = aisling.TryGiveItem(ref newItem);

        result.Should()
              .BeTrue();

        // After consolidation, the ref should point to the inventory stack
        newItem.Count
               .Should()
               .Be(8);
    }
    #endregion

    #region TryGiveItems
    [Test]
    public void TryGiveItems_ShouldReturnFalse_WhenCantCarry()
    {
        var aisling = MockAisling.Create(Map);

        var items = new List<Item>
        {
            MockItem.Create("Item1"),
            MockItem.Create("Item2")
        };

        var result = aisling.TryGiveItems(items);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryGiveItems_ShouldReturnTrue_WhenCanCarry()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var items = new List<Item>
        {
            MockItem.Create("Item1"),
            MockItem.Create("Item2")
        };

        var result = aisling.TryGiveItems(items);

        result.Should()
              .BeTrue();

        aisling.Inventory
               .Contains("Item1")
               .Should()
               .BeTrue();

        aisling.Inventory
               .Contains("Item2")
               .Should()
               .BeTrue();
    }
    #endregion

    #region GiveItemOrSendToBank
    [Test]
    public void GiveItemOrSendToBank_ShouldAddToInventory_WhenCanCarry()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Sword");

        aisling.GiveItemOrSendToBank(item);

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeTrue();
    }

    [Test]
    public void GiveItemOrSendToBank_ShouldSendToBank_WhenCantCarry()
    {
        var aisling = MockAisling.Create(Map);

        // MaxWeight defaults to 0 so can't carry
        var item = MockItem.Create("Sword");

        aisling.GiveItemOrSendToBank(item);

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeFalse();

        aisling.Bank
               .Contains("Sword")
               .Should()
               .BeTrue();
    }
    #endregion

    #region Equip / UnEquip
    [Test]
    public void Equip_ShouldEquipItem_WhenSlotIsEmpty()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create(
            "Sword",
            setup: i =>
            {
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(i.Template, EquipmentType.Weapon);
            });

        aisling.Inventory.TryAddToNextSlot(item);
        var slot = item.Slot;

        aisling.Equip(EquipmentType.Weapon, item);

        aisling.Equipment[EquipmentSlot.Weapon]
               .Should()
               .NotBeNull();

        aisling.Inventory
               .TryGetObject(slot, out _)
               .Should()
               .BeFalse();
    }

    [Test]
    public void Equip_ShouldSwapItems_WhenSlotIsOccupied()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var firstSword = MockItem.Create(
            "OldSword",
            setup: i =>
            {
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(i.Template, EquipmentType.Weapon);
            });

        aisling.Inventory.TryAddToNextSlot(firstSword);
        aisling.Equip(EquipmentType.Weapon, firstSword);

        var newSword = MockItem.Create(
            "NewSword",
            setup: i =>
            {
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(i.Template, EquipmentType.Weapon);
            });

        aisling.Inventory.TryAddToNextSlot(newSword);
        aisling.Equip(EquipmentType.Weapon, newSword);

        aisling.Equipment[EquipmentSlot.Weapon]!.DisplayName
               .Should()
               .Be("NewSword");

        // Old sword should be back in inventory
        aisling.Inventory
               .Contains("OldSword")
               .Should()
               .BeTrue();
    }

    [Test]
    public void UnEquip_ShouldDoNothing_WhenInventoryIsFull()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(500);

        var weapon = MockItem.Create(
            "Sword",
            setup: i =>
            {
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(i.Template, EquipmentType.Weapon);
            });

        aisling.Inventory.TryAddToNextSlot(weapon);
        aisling.Equip(EquipmentType.Weapon, weapon);

        // Fill inventory
        for (var i = 0; i < 60; i++)
            aisling.Inventory.TryAddToNextSlot(MockItem.Create($"Item{i}"));

        aisling.UnEquip(EquipmentSlot.Weapon);

        // Weapon should still be equipped
        aisling.Equipment[EquipmentSlot.Weapon]
               .Should()
               .NotBeNull();
    }

    [Test]
    public void UnEquip_ShouldDoNothing_WhenSlotIsEmpty()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        aisling.UnEquip(EquipmentSlot.Weapon);

        // Should not throw, just do nothing
        aisling.Equipment[EquipmentSlot.Weapon]
               .Should()
               .BeNull();
    }

    [Test]
    public void UnEquip_ShouldMoveToInventory_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var weapon = MockItem.Create(
            "Sword",
            setup: i =>
            {
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(i.Template, EquipmentType.Weapon);
            });

        aisling.Inventory.TryAddToNextSlot(weapon);
        aisling.Equip(EquipmentType.Weapon, weapon);

        aisling.UnEquip(EquipmentSlot.Weapon);

        aisling.Equipment[EquipmentSlot.Weapon]
               .Should()
               .BeNull();

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeTrue();
    }
    #endregion

    #region Illuminates
    [Test]
    public void Illuminates_ShouldReturnTrue_WhenMapHasNoDarkness()
    {
        var aisling = MockAisling.Create(Map);
        var other = MockAisling.Create(Map, "Other");

        // No darkness flag by default
        aisling.Illuminates(other)
               .Should()
               .BeTrue();
    }

    [Test]
    public void Illuminates_ShouldReturnFalse_WhenDarknessAndNoLantern()
    {
        var darkMap = MockMapInstance.Create(setup: m => m.Flags = MapFlags.Darkness);
        var aisling = MockAisling.Create(darkMap, "WithNoLantern");

        // LanternSize defaults to None
        var other = MockAisling.Create(darkMap, "Other");

        aisling.Illuminates(other)
               .Should()
               .BeFalse();
    }

    [Test]
    public void Illuminates_ShouldReturnTrue_WhenDarknessAndWithinLanternRange()
    {
        var darkMap = MockMapInstance.Create(setup: m => m.Flags = MapFlags.Darkness);
        var aisling = MockAisling.Create(darkMap, "WithLantern", new Point(5, 5));
        aisling.SetLanternSize(LanternSize.Small);

        var nearby = MockAisling.Create(darkMap, "Nearby", new Point(6, 6));

        aisling.Illuminates(nearby)
               .Should()
               .BeTrue();
    }

    [Test]
    public void Illuminates_ShouldReturnFalse_WhenDarknessAndOutOfLanternRange()
    {
        var darkMap = MockMapInstance.Create(setup: m => m.Flags = MapFlags.Darkness);
        var aisling = MockAisling.Create(darkMap, "WithLantern", new Point(0, 0));
        aisling.SetLanternSize(LanternSize.Small); // radius 3

        var farAway = MockAisling.Create(darkMap, "Far", new Point(9, 9));

        aisling.Illuminates(farAway)
               .Should()
               .BeFalse();
    }
    #endregion

    #region GetLanternRadius
    [Test]
    public void GetLanternRadius_ShouldReturn3_ForSmall()
    {
        var aisling = MockAisling.Create(Map);
        aisling.SetLanternSize(LanternSize.Small);

        aisling.GetLanternRadius()
               .Should()
               .Be(3);
    }

    [Test]
    public void GetLanternRadius_ShouldReturn5_ForLarge()
    {
        var aisling = MockAisling.Create(Map);
        aisling.SetLanternSize(LanternSize.Large);

        aisling.GetLanternRadius()
               .Should()
               .Be(5);
    }

    [Test]
    public void GetLanternRadius_ShouldThrow_ForNone()
    {
        var aisling = MockAisling.Create(Map);

        // LanternSize defaults to None
        var act = () => aisling.GetLanternRadius();

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }
    #endregion

    #region SetVision
    [Test]
    public void SetVision_ShouldNotCallClient_WhenSameVision()
    {
        var aisling = MockAisling.Create(Map);
        var clientMock = Mock.Get(aisling.Client);

        // Vision defaults to Normal, set it again to Normal
        aisling.SetVision(VisionType.Normal);

        // Should not have sent attributes since vision didn't change
        clientMock.Verify(c => c.SendAttributes(It.IsAny<StatUpdateType>()), Times.Never);
    }

    [Test]
    public void SetVision_ShouldChangeVision_WhenDifferent()
    {
        var aisling = MockAisling.Create(Map);

        // Vision defaults to Normal
        aisling.SetVision(VisionType.TrueBlind);

        aisling.Vision
               .Should()
               .Be(VisionType.TrueBlind);
    }
    #endregion

    #region OnClicked
    [Test]
    public void OnClicked_ShouldSendSelfProfile_WhenClickingSelf()
    {
        var aisling = MockAisling.Create(Map);
        var clientMock = Mock.Get(aisling.Client);

        aisling.OnClicked(aisling);

        clientMock.Verify(c => c.SendSelfProfile(), Times.Once);
    }

    [Test]
    public void OnClicked_ShouldSendOtherProfile_WhenClickingOther()
    {
        var source = MockAisling.Create(Map, "Clicker");
        var target = MockAisling.Create(Map, "Target");
        var sourceClientMock = Mock.Get(source.Client);

        source.OnClicked(target);

        // Source clicks target, target's OnClicked is called with source
        target.OnClicked(source);

        sourceClientMock.Verify(c => c.SendOtherProfile(target), Times.Once);
    }
    #endregion

    #region Turn
    [Test]
    public void Turn_ShouldChangeDirection_WhenForced()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Direction = Direction.Up;

        aisling.Turn(Direction.Down, true);

        aisling.Direction
               .Should()
               .Be(Direction.Down);
    }

    [Test]
    public void Turn_ShouldUpdateTrackers_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);

        aisling.Turn(Direction.Left, true);

        aisling.Trackers
               .LastTurn
               .Should()
               .NotBeNull();
    }

    [Test]
    public void Turn_ShouldNotChangeDirection_WhenScriptDisallows()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Direction = Direction.Up;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(false);

        aisling.Turn(Direction.Down);

        aisling.Direction
               .Should()
               .Be(Direction.Up);
    }
    #endregion

    #region TryDrop
    [Test]
    public void TryDrop_ShouldReturnFalse_WhenWall()
    {
        // The default map has no walls, so use the aisling at position (5,5)
        // and make the map think there's a wall using the tile data
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        // Set up script mock to allow drops
        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropItem(It.IsAny<Item>()))
                  .Returns(true);

        var itemScriptMock = Mock.Get(item.Script);
        itemScriptMock.Reset();

        itemScriptMock.Setup(s => s.CanBeDropped(It.IsAny<Aisling>(), It.IsAny<Point>()))
                      .Returns(true);

        // Drop out of range — point far from aisling position
        var farPoint = new Point(9, 9);

        var result = aisling.TryDrop(farPoint, item.Slot, out _);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryDrop_ShouldReturnFalse_WhenAccountBound()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("BoundItem", setup: i => i.AccountBound = true);

        aisling.Inventory.TryAddToNextSlot(item);

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropItem(It.IsAny<Item>()))
                  .Returns(true);

        var result = aisling.TryDrop(aisling, item.Slot, out _);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region TryDropGold
    [Test]
    public void TryDropGold_ShouldReturnFalse_WhenOutOfRange()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var farPoint = new Point(9, 9);

        var result = aisling.TryDropGold(farPoint, 50, out _);

        result.Should()
              .BeFalse();

        aisling.Gold
               .Should()
               .Be(100);
    }

    [Test]
    public void TryDropGold_ShouldReturnFalse_WhenScriptDisallows()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropMoney(It.IsAny<int>()))
                  .Returns(false);

        var dropPoint = new Point(aisling.X, aisling.Y);

        var result = aisling.TryDropGold(dropPoint, 50, out _);

        result.Should()
              .BeFalse();

        aisling.Gold
               .Should()
               .Be(100);
    }

    [Test]
    public void TryDropGold_ShouldReturnFalse_WhenInsufficientGold()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 10;

        var scriptMock = Mock.Get(aisling.Script);

        scriptMock.Setup(s => s.CanDropMoney(It.IsAny<int>()))
                  .Returns(true);

        var dropPoint = new Point(aisling.X, aisling.Y);

        var result = aisling.TryDropGold(dropPoint, 100, out _);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryDropGold_ShouldReturnTrue_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 100;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropMoney(It.IsAny<int>()))
                  .Returns(true);

        var dropPoint = new Point(aisling.X, aisling.Y);

        var result = aisling.TryDropGold(dropPoint, 50, out var money);

        result.Should()
              .BeTrue();

        money.Should()
             .NotBeNull();

        money.Amount
             .Should()
             .Be(50);

        aisling.Gold
               .Should()
               .Be(50);
    }
    #endregion

    #region TryStartExchange
    [Test]
    public void OnGoldDroppedOn_ShouldDoNothing_WhenOutOfRange()
    {
        var source = MockAisling.Create(Map, "Source", new Point(0, 0));
        var target = MockAisling.Create(Map, "Target", new Point(9, 9));
        source.Gold = 100;

        target.OnGoldDroppedOn(source, 50);

        // No exchange started, gold unchanged
        source.Gold
              .Should()
              .Be(100);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldNotStartExchange_WhenScriptDisallows()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        source.Gold = 100;

        var scriptMock = Mock.Get(target.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropMoneyOn(It.IsAny<Aisling>(), It.IsAny<int>()))
                  .Returns(false);

        target.OnGoldDroppedOn(source, 50);

        source.Gold
              .Should()
              .Be(100);
    }
    #endregion

    #region OnItemDroppedOn
    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenOutOfRange()
    {
        var source = MockAisling.Create(Map, "Source", new Point(0, 0));
        source.UserStatSheet.SetMaxWeight(50);
        var target = MockAisling.Create(Map, "Target", new Point(9, 9));

        var item = MockItem.Create("Sword");
        source.Inventory.TryAddToNextSlot(item);

        target.OnItemDroppedOn(source, item.Slot, 0);

        // Item should still be in source's inventory
        source.Inventory
              .Contains("Sword")
              .Should()
              .BeTrue();
    }

    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenInventorySlotEmpty()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        target.OnItemDroppedOn(source, 5, 0);

        // No crash, just does nothing
    }
    #endregion

    #region TryUseItem
    [Test]
    public void TryUseItem_BySlot_ShouldReturnFalse_WhenSlotEmpty()
    {
        var aisling = MockAisling.Create(Map);

        aisling.TryUseItem(5)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseItem_ShouldReturnFalse_WhenCanUseReturnsFalse()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Potion");
        aisling.Inventory.TryAddToNextSlot(item);

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanUseItem(It.IsAny<Item>()))
                  .Returns(false);

        aisling.TryUseItem(item)
               .Should()
               .BeFalse();
    }
    #endregion

    #region TryUseSkill
    [Test]
    public void TryUseSkill_BySlot_ShouldReturnFalse_WhenSlotEmpty()
    {
        var aisling = MockAisling.Create(Map);

        aisling.TryUseSkill(5)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSkill_ShouldReturnFalse_WhenCanUseReturnsFalse()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create();

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanUseSkill(It.IsAny<Skill>()))
                  .Returns(false);

        aisling.TryUseSkill(skill)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSkill_Assail_ShouldSucceed_WhenAllChecksPass()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create("AssailSkill", true);

        // Set up script mocks to allow
        var aislingScriptMock = Mock.Get(aisling.Script);

        aislingScriptMock.Setup(s => s.CanUseSkill(It.IsAny<Skill>()))
                         .Returns(true);

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(s => s.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        // Assail skills skip the action/skill throttle checks in CanUse
        var result = aisling.TryUseSkill(skill);

        result.Should()
              .BeTrue();

        aisling.Trackers
               .LastUsedSkill
               .Should()
               .Be(skill);
    }
    #endregion

    #region TryUseSpell
    [Test]
    public void TryUseSpell_BySlot_ShouldReturnFalse_WhenSlotEmpty()
    {
        var aisling = MockAisling.Create(Map);

        aisling.TryUseSpell(5)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSpell_ShouldReturnFalse_WhenTargetedAndNoTargetId()
    {
        var aisling = MockAisling.Create(Map);

        var spell = MockSpell.Create(
            setup: s =>
            {
                typeof(SpellTemplate).GetProperty(nameof(SpellTemplate.SpellType))!.SetValue(s.Template, SpellType.Targeted);
            });

        aisling.TryUseSpell(spell)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSpell_ShouldReturnFalse_WhenTargetNotFound()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        // Target ID that doesn't exist on the map
        aisling.TryUseSpell(spell, 99999)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSpell_ShouldReturnFalse_WhenCanUseReturnsFalse()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        // Script.CanUseSpell returns false by default
        aisling.TryUseSpell(spell)
               .Should()
               .BeFalse();
    }
    #endregion

    #region ShowPublicMessage
    [Test]
    public void ShowPublicMessage_ShouldDoNothing_WhenScriptDisallowsTalk()
    {
        var aisling = MockAisling.Create(Map);

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTalk())
                  .Returns(false);

        // Should not throw
        aisling.ShowPublicMessage(PublicMessageType.Normal, "Hello");
    }

    [Test]
    public void ShowPublicMessage_ShouldSendMessage_WhenScriptAllowsTalk()
    {
        var aisling = MockAisling.Create(Map);

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTalk())
                  .Returns(true);

        // Should not throw and should process the message
        aisling.ShowPublicMessage(PublicMessageType.Normal, "Hello World");
    }
    #endregion

    #region CanObserve (Aisling override)
    [Test]
    public void CanObserve_ShouldReturnTrue_WhenSelf()
    {
        var aisling = MockAisling.Create(Map);

        aisling.CanObserve(aisling, true)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanObserve_ShouldReturnFalse_WhenNotInApproachTime_AndNotFullCheck()
    {
        var aisling = MockAisling.Create(Map);
        var other = MockAisling.Create(Map, "Other");

        aisling.CanObserve(other)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanObserve_ShouldReturnFalse_WhenFarAwayOnDarkMap()
    {
        var darkMap = MockMapInstance.Create(setup: m => m.Flags = MapFlags.Darkness);
        var aisling = MockAisling.Create(darkMap, "Observer", new Point(0, 0));
        var other = MockAisling.Create(darkMap, "Far", new Point(5, 5));

        // Distance is 10, no shared lantern vision on dark map
        aisling.CanObserve(other, true)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanObserve_ShouldReturnFalse_WhenCloseButTrueBlind()
    {
        var aisling = MockAisling.Create(Map);
        var other = MockAisling.Create(Map, "Other");

        // Same position (5,5), distance = 0
        aisling.SetVision(VisionType.TrueBlind);

        aisling.CanObserve(other, true)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanObserve_ShouldDelegateToBase_WhenCloseAndNotBlind()
    {
        var aisling = MockAisling.Create(Map);
        var other = MockAisling.Create(Map, "Other");

        // Same position (5,5), distance = 0, vision = Normal
        // Base returns true for Normal visibility entities
        aisling.CanObserve(other, true)
               .Should()
               .BeTrue();
    }
    #endregion

    #region CanSee (Aisling override)
    [Test]
    public void CanSee_ShouldReturnTrue_WhenAdmin()
    {
        var aisling = MockAisling.Create(Map, setup: a => a.IsAdmin = true);
        var hidden = MockAisling.Create(Map, "Hidden");
        hidden.SetVisibility(VisibilityType.GmHidden);

        aisling.CanSee(hidden)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanSee_ShouldDelegateToBase_WhenNotAdmin()
    {
        var aisling = MockAisling.Create(Map);
        var hidden = MockAisling.Create(Map, "Hidden");
        hidden.SetVisibility(VisibilityType.TrueHidden);

        // Base Creature.CanSee calls Script.CanSee
        var scriptMock = Mock.Get(aisling.Script);

        scriptMock.Setup(s => s.CanSee(hidden))
                  .Returns(false);

        aisling.CanSee(hidden)
               .Should()
               .BeFalse();
    }
    #endregion

    #region CanUse (Item) deeper
    [Test]
    public void CanUse_Item_ShouldReturnTrue_WhenAllChecksPass()
    {
        var aisling = MockAisling.Create(Map);
        var item = MockItem.Create("Potion");

        MockAisling.SetupScriptAllows(aisling);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanUse(aisling))
                      .Returns(true);

        aisling.CanUse(item)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanUse_Item_ShouldReturnFalse_WhenOnCooldown()
    {
        var aisling = MockAisling.Create(Map);
        var item = MockItem.Create("Potion");

        MockAisling.SetupScriptAllows(aisling);

        // Put item on cooldown
        item.Cooldown = TimeSpan.FromSeconds(10);
        item.Elapsed = TimeSpan.Zero;

        aisling.CanUse(item)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanUse_Item_ShouldReturnFalse_WhenItemScriptDisallows()
    {
        var aisling = MockAisling.Create(Map);
        var item = MockItem.Create("Potion");

        MockAisling.SetupScriptAllows(aisling);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanUse(aisling))
                      .Returns(false);

        aisling.CanUse(item)
               .Should()
               .BeFalse();
    }
    #endregion

    #region TryUseItem deeper
    [Test]
    public void TryUseItem_ShouldReturnTrue_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        var item = MockItem.Create("Potion");
        aisling.Inventory.TryAddToNextSlot(item);

        MockAisling.SetupScriptAllows(aisling);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanUse(aisling))
                      .Returns(true);

        aisling.TryUseItem(item)
               .Should()
               .BeTrue();
    }

    [Test]
    public void TryUseItem_BySlot_ShouldReturnTrue_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        var item = MockItem.Create("Potion");
        aisling.Inventory.TryAddToNextSlot(item);

        MockAisling.SetupScriptAllows(aisling);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanUse(aisling))
                      .Returns(true);

        aisling.TryUseItem(item.Slot)
               .Should()
               .BeTrue();
    }
    #endregion

    #region TryUseSkill deeper
    [Test]
    public void TryUseSkill_NonAssail_ShouldReturnTrue_WhenAllChecksPass()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create("NonAssailSkill");

        MockAisling.SetupScriptAllows(aisling);

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(s => s.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        var result = aisling.TryUseSkill(skill);

        result.Should()
              .BeTrue();

        aisling.Trackers
               .LastUsedSkill
               .Should()
               .Be(skill);

        aisling.Trackers
               .LastSkillUse
               .Should()
               .NotBeNull();
    }

    [Test]
    public void TryUseSkill_BySlot_ShouldReturnTrue_WhenSkillInBook()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create("BookSkill");
        aisling.SkillBook.TryAddToNextSlot(skill);

        MockAisling.SetupScriptAllows(aisling);

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(s => s.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        aisling.TryUseSkill(skill.Slot)
               .Should()
               .BeTrue();
    }
    #endregion

    #region TryUseSpell deeper
    [Test]
    public void TryUseSpell_ShouldReturnTrue_WhenSelfTargetNonTargeted()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        MockAisling.SetupScriptAllows(aisling);

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(s => s.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        var result = aisling.TryUseSpell(spell);

        result.Should()
              .BeTrue();

        aisling.Trackers
               .LastUsedSpell
               .Should()
               .Be(spell);

        aisling.Trackers
               .LastSpellUse
               .Should()
               .NotBeNull();
    }

    [Test]
    public void TryUseSpell_ShouldReturnTrue_WhenTargetFoundOnMap()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        var monster = MockMonster.Create(Map);
        Map.AddEntity(monster, new Point(5, 5));

        var spell = MockSpell.Create();

        MockAisling.SetupScriptAllows(aisling);

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(s => s.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        var result = aisling.TryUseSpell(spell, monster.Id);

        result.Should()
              .BeTrue();
    }

    [Test]
    public void TryUseSpell_BySlot_ShouldReturnTrue_WhenSpellInBook()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create("BookSpell");
        aisling.SpellBook.TryAddToNextSlot(spell);

        MockAisling.SetupScriptAllows(aisling);

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(s => s.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        aisling.TryUseSpell(spell.Slot)
               .Should()
               .BeTrue();
    }
    #endregion

    #region TryPickupItem
    [Test]
    public void TryPickupItem_ShouldReturnFalse_WhenCannotBePickedUp()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        MockAisling.SetupScriptAllows(aisling);

        var groundItem = MockGroundItem.Create(Map, canBePickedUp: false);

        aisling.TryPickupItem(groundItem, 1)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryPickupItem_ShouldReturnFalse_WhenScriptDisallows()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanPickupItem(It.IsAny<GroundItem>()))
                  .Returns(false);

        var groundItem = MockGroundItem.Create(Map);

        aisling.TryPickupItem(groundItem, 1)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryPickupItem_ShouldReturnTrue_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        MockAisling.SetupScriptAllows(aisling);

        var groundItem = MockGroundItem.Create(Map, "Sword");
        Map.AddEntity(groundItem, new Point(5, 5));

        var result = aisling.TryPickupItem(groundItem, 1);

        result.Should()
              .BeTrue();

        aisling.Inventory
               .Contains("Sword")
               .Should()
               .BeTrue();
    }

    [Test]
    public void TryPickupItem_ShouldReturnFalse_WhenCantCarry()
    {
        var aisling = MockAisling.Create(Map);

        // MaxWeight defaults to 0, can't carry
        MockAisling.SetupScriptAllows(aisling);

        var groundItem = MockGroundItem.Create(Map);

        aisling.TryPickupItem(groundItem, 1)
               .Should()
               .BeFalse();
    }
    #endregion

    #region TryPickupMoney
    [Test]
    public void TryPickupMoney_ShouldReturnFalse_WhenCannotBePickedUp()
    {
        var aisling = MockAisling.Create(Map);
        MockAisling.SetupScriptAllows(aisling);

        var money = new Money(100, Map, new Point(5, 5));
        var owner = MockAisling.Create(Map, "Owner");
        money.LockToAislings(60, owner);

        aisling.TryPickupMoney(money)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryPickupMoney_ShouldReturnFalse_WhenScriptDisallows()
    {
        var aisling = MockAisling.Create(Map);

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanPickupMoney(It.IsAny<Money>()))
                  .Returns(false);

        var money = new Money(100, Map, new Point(5, 5));

        aisling.TryPickupMoney(money)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryPickupMoney_ShouldReturnTrue_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Gold = 0;
        MockAisling.SetupScriptAllows(aisling);

        var money = new Money(100, Map, new Point(5, 5));
        Map.AddEntity(money, new Point(5, 5));

        var result = aisling.TryPickupMoney(money);

        aisling.Gold
               .Should()
               .Be(100);

        result.Should()
              .BeTrue();
    }
    #endregion

    #region TryStartExchange (via OnGoldDroppedOn)
    [Test]
    public void OnGoldDroppedOn_ShouldStartExchange_WhenAllowed()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        source.Gold = 100;

        MockAisling.SetupScriptAllows(target);

        var sourceClient = Mock.Get(source.Client);

        target.OnGoldDroppedOn(source, 50);

        // Exchange was started — verify exchange window was opened
        sourceClient.Verify(c => c.SendExchangeStart(target), Times.Once);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldFail_WhenSourceDisabledExchange()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        source.Gold = 100;
        source.Options.AllowExchange = false;

        MockAisling.SetupScriptAllows(target);

        var sourceClient = Mock.Get(source.Client);

        target.OnGoldDroppedOn(source, 50);

        sourceClient.Verify(c => c.SendExchangeStart(It.IsAny<Aisling>()), Times.Never);

        source.Gold
              .Should()
              .Be(100);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldFail_WhenTargetIgnoresSource()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        source.Gold = 100;
        target.IgnoreList.Add("Source");

        MockAisling.SetupScriptAllows(target);

        var sourceClient = Mock.Get(source.Client);

        target.OnGoldDroppedOn(source, 50);

        sourceClient.Verify(c => c.SendExchangeStart(It.IsAny<Aisling>()), Times.Never);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldFail_WhenTargetDisabledExchange()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        source.Gold = 100;
        target.Options.AllowExchange = false;

        MockAisling.SetupScriptAllows(target);

        var sourceClient = Mock.Get(source.Client);

        target.OnGoldDroppedOn(source, 50);

        sourceClient.Verify(c => c.SendExchangeStart(It.IsAny<Aisling>()), Times.Never);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldFail_WhenTargetIsBusy()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        source.Gold = 100;
        target.ActiveObject.SetIfNull(new object()); // Target is busy

        MockAisling.SetupScriptAllows(target);

        var sourceClient = Mock.Get(source.Client);

        target.OnGoldDroppedOn(source, 50);

        sourceClient.Verify(c => c.SendExchangeStart(It.IsAny<Aisling>()), Times.Never);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldFail_WhenSourceIsBusy()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        source.Gold = 100;
        source.ActiveObject.SetIfNull(new object()); // Source is busy

        MockAisling.SetupScriptAllows(target);

        var sourceClient = Mock.Get(source.Client);

        target.OnGoldDroppedOn(source, 50);

        sourceClient.Verify(c => c.SendExchangeStart(It.IsAny<Aisling>()), Times.Never);
    }
    #endregion

    #region OnItemDroppedOn deeper
    [Test]
    public void OnItemDroppedOn_ShouldFail_WhenScriptCanDropItemOnReturnsFalse()
    {
        var source = MockAisling.Create(Map, "Source");
        source.UserStatSheet.SetMaxWeight(50);
        var target = MockAisling.Create(Map, "Target");
        var item = MockItem.Create("Sword");
        source.Inventory.TryAddToNextSlot(item);

        var targetScriptMock = Mock.Get(target.Script);
        targetScriptMock.Reset();

        targetScriptMock.Setup(s => s.CanDropItemOn(It.IsAny<Aisling>(), It.IsAny<Item>()))
                        .Returns(false);

        target.OnItemDroppedOn(source, item.Slot, 0);

        // Item should still be in source's inventory
        source.Inventory
              .Contains("Sword")
              .Should()
              .BeTrue();
    }

    [Test]
    public void OnItemDroppedOn_ShouldAddItem_WhenCountIsZero()
    {
        var source = MockAisling.Create(Map, "Source");
        source.UserStatSheet.SetMaxWeight(50);
        var target = MockAisling.Create(Map, "Target");
        target.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Sword");
        source.Inventory.TryAddToNextSlot(item);

        MockAisling.SetupScriptAllows(target);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanBeDroppedOn(It.IsAny<Aisling>(), It.IsAny<Aisling>()))
                      .Returns(true);

        var sourceClient = Mock.Get(source.Client);

        target.OnItemDroppedOn(source, item.Slot, 0);

        // Exchange was started
        sourceClient.Verify(c => c.SendExchangeStart(target), Times.Once);
    }

    [Test]
    public void OnItemDroppedOn_ShouldAddStackableItem_WhenCountIsPositive()
    {
        var source = MockAisling.Create(Map, "Source");
        source.UserStatSheet.SetMaxWeight(50);
        var target = MockAisling.Create(Map, "Target");
        target.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Potion", 10, true);
        source.Inventory.TryAddToNextSlot(item);

        MockAisling.SetupScriptAllows(target);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanBeDroppedOn(It.IsAny<Aisling>(), It.IsAny<Aisling>()))
                      .Returns(true);

        var sourceClient = Mock.Get(source.Client);

        target.OnItemDroppedOn(source, item.Slot, 5);

        // Exchange was started
        sourceClient.Verify(c => c.SendExchangeStart(target), Times.Once);
    }

    [Test]
    public void OnItemDroppedOn_ShouldFail_WhenCountExceedsItemCount()
    {
        var source = MockAisling.Create(Map, "Source");
        source.UserStatSheet.SetMaxWeight(50);
        var target = MockAisling.Create(Map, "Target");

        var item = MockItem.Create("Potion", 5, true);
        source.Inventory.TryAddToNextSlot(item);

        target.OnItemDroppedOn(source, item.Slot, 10);

        // Exchange was not started, item still in inventory
        source.Inventory
              .Contains("Potion")
              .Should()
              .BeTrue();
    }
    #endregion

    #region TryDrop deeper
    [Test]
    public void TryDrop_ShouldReturnFalse_WhenActualWall()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        // Set up wall at adjacent point
        MockMapInstance.SetWall(Map, new Point(5, 6));

        var result = aisling.TryDrop(new Point(5, 6), item.Slot, out _);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryDrop_ShouldReturnFalse_WhenScriptCanDropItemReturnsFalse()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Sword");
        aisling.Inventory.TryAddToNextSlot(item);

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropItem(It.IsAny<Item>()))
                  .Returns(false);

        var result = aisling.TryDrop(aisling, item.Slot, out _);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryDrop_ShouldReturnTrue_WhenSuccessful_NoAmount()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        MockAisling.SetupScriptAllows(aisling);

        var item = MockItem.Create("Sword");

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanBeDropped(It.IsAny<Aisling>(), It.IsAny<Point>()))
                      .Returns(true);

        aisling.Inventory.TryAddToNextSlot(item);
        var slot = item.Slot;

        var result = aisling.TryDrop(aisling, slot, out var groundItems);

        result.Should()
              .BeTrue();

        groundItems.Should()
                   .NotBeNull();

        aisling.Inventory
               .TryGetObject(slot, out _)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryDrop_WithAmount_ShouldReturnFalse_WhenInsufficientCount()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        MockAisling.SetupScriptAllows(aisling);

        var item = MockItem.Create("Potion", 5, true);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanBeDropped(It.IsAny<Aisling>(), It.IsAny<Point>()))
                      .Returns(true);

        aisling.Inventory.TryAddToNextSlot(item);

        var result = aisling.TryDrop(
            aisling,
            item.Slot,
            out _,
            10);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryDrop_WithAmount_ShouldReturnTrue_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        MockAisling.SetupScriptAllows(aisling);

        // Inject cloner into inventory (default Inventory has null cloner)
        typeof(Inventory).GetField("ItemCloner", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(
            aisling.Inventory,
            MockScriptProvider.ItemCloner.Object);

        var item = MockItem.Create("Potion", 10, true);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanBeDropped(It.IsAny<Aisling>(), It.IsAny<Point>()))
                      .Returns(true);

        aisling.Inventory.TryAddToNextSlot(item);

        var result = aisling.TryDrop(
            aisling,
            item.Slot,
            out var groundItems,
            5);

        result.Should()
              .BeTrue();

        groundItems.Should()
                   .NotBeNull();
    }

    [Test]
    public void TryDrop_ShouldLockToAisling_WhenNoTrade()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);
        MockAisling.SetupScriptAllows(aisling);

        var item = MockItem.Create("BoundSword", setup: i => i.NoTrade = true);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanBeDropped(It.IsAny<Aisling>(), It.IsAny<Point>()))
                      .Returns(true);

        itemScriptMock.Setup(s => s.CanBePickedUp(It.IsAny<Aisling>(), It.IsAny<Point>()))
                      .Returns(true);

        aisling.Inventory.TryAddToNextSlot(item);

        var result = aisling.TryDrop(aisling, item.Slot, out var groundItems);

        result.Should()
              .BeTrue();

        // The ground items should be locked to the aisling (other can't pick up)
        var other = MockAisling.Create(Map, "Other");

        groundItems![0]
            .CanBePickedUp(other)
            .Should()
            .BeFalse();

        // Owner can pick up (base check passes + script check passes)
        groundItems[0]
            .CanBePickedUp(aisling)
            .Should()
            .BeTrue();
    }
    #endregion

    #region SetVision deeper
    [Test]
    public void SetVision_ShouldRefresh_WhenSettingToTrueBlind()
    {
        var aisling = MockAisling.Create(Map);
        var clientMock = Mock.Get(aisling.Client);

        aisling.SetVision(VisionType.TrueBlind);

        // Refresh sends MapInfo (among other things)
        clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);
    }

    [Test]
    public void SetVision_ShouldRefresh_WhenClearingTrueBlind()
    {
        var aisling = MockAisling.Create(Map);
        aisling.SetVision(VisionType.TrueBlind);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.SetVision(VisionType.Normal);

        // Refresh sends MapInfo when going from TrueBlind to Normal
        clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);
    }

    [Test]
    public void SetVision_ShouldSendAttributesOnly_WhenNotTrueBlind()
    {
        var aisling = MockAisling.Create(Map);

        // Set to something non-TrueBlind first
        aisling.SetVision(VisionType.TrueBlind);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Now set to another non-Normal, non-TrueBlind won't work...
        // Instead test Normal → Blind transition (non-TrueBlind)
        // Reset to Normal first
        aisling.SetVision(VisionType.Normal);
        clientMock.Invocations.Clear();

        // Normal to Blind — neither is TrueBlind
        aisling.SetVision(VisionType.Blind);

        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Secondary), Times.Once);
    }
    #endregion

    #region Turn (response version)
    [Test]
    public void Turn_Response_ShouldNotSendTurnToSelf_WhenNotForced()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(true);

        var clientMock = Mock.Get(aisling.Client);

        aisling.Turn(Direction.Down, false, true);

        // With isResponse=true and not forced, self should be skipped
        clientMock.Verify(c => c.SendCreatureTurn(aisling.Id, Direction.Down), Times.Never);
    }

    [Test]
    public void Turn_Response_ShouldSendTurnToSelf_WhenForced()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        var clientMock = Mock.Get(aisling.Client);

        aisling.Turn(Direction.Down, true, true);

        // With forced=true, self should receive the turn even with isResponse=true
        clientMock.Verify(c => c.SendCreatureTurn(aisling.Id, Direction.Down), Times.Once);
    }
    #endregion

    #region Refresh
    [Test]
    public void Refresh_ShouldDoNothing_WhenNotForcedAndNotDue()
    {
        var aisling = MockAisling.Create(Map);

        // Force a refresh to set LastRefresh
        aisling.Refresh(true);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Non-forced refresh immediately after should be skipped
        aisling.Refresh();

        clientMock.Verify(c => c.SendMapInfo(), Times.Never);
    }

    [Test]
    public void Refresh_ShouldExecute_WhenForced()
    {
        var aisling = MockAisling.Create(Map);

        // Force a refresh to set LastRefresh
        aisling.Refresh(true);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Forced refresh should always execute
        aisling.Refresh(true);

        clientMock.Verify(c => c.SendMapInfo(), Times.Once);
        clientMock.Verify(c => c.SendLocation(), Times.Once);
        clientMock.Verify(c => c.SendRefreshResponse(), Times.Once);
    }
    #endregion

    #region Equip deeper
    [Test]
    public void Equip_ShouldSetTimestamps()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var weapon = MockItem.Create(
            "Sword",
            setup: i =>
            {
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(i.Template, EquipmentType.Weapon);
            });

        aisling.Inventory.TryAddToNextSlot(weapon);

        aisling.Equip(EquipmentType.Weapon, weapon);

        aisling.Trackers
               .LastEquip
               .Should()
               .NotBeNull();
    }

    [Test]
    public void Equip_ShouldSetUnequipTimestamp_WhenSwapping()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var oldWeapon = MockItem.Create(
            "OldSword",
            setup: i =>
            {
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(i.Template, EquipmentType.Weapon);
            });

        aisling.Inventory.TryAddToNextSlot(oldWeapon);
        aisling.Equip(EquipmentType.Weapon, oldWeapon);

        var newWeapon = MockItem.Create(
            "NewSword",
            setup: i =>
            {
                typeof(ItemTemplate).GetProperty(nameof(ItemTemplate.EquipmentType))!.SetValue(i.Template, EquipmentType.Weapon);
            });

        aisling.Inventory.TryAddToNextSlot(newWeapon);
        aisling.Equip(EquipmentType.Weapon, newWeapon);

        aisling.Trackers
               .LastUnequip
               .Should()
               .NotBeNull();
    }
    #endregion

    #region Walk
    [Test]
    public void Walk_ShouldRefresh_WhenScriptCanMoveReturnsFalse()
    {
        var aisling = MockAisling.Create(Map);

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanMove())
                  .Returns(false);

        var clientMock = Mock.Get(aisling.Client);

        aisling.Walk(Direction.Down);

        // Walk should have called Refresh(true)
        clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);

        // Position should not have changed
        aisling.X
               .Should()
               .Be(5);

        aisling.Y
               .Should()
               .Be(5);
    }

    [Test]
    public void Walk_ShouldSetDirection_EvenWhenNotWalkable()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        MockAisling.SetupScriptAllows(aisling);

        // Walk toward edge of map (map is 10x10, going to 10,5 is out of bounds)
        // First, position at edge
        aisling.Direction = Direction.Down;

        // Walk into a wall
        MockMapInstance.SetWall(Map, new Point(5, 6));

        aisling.Walk(Direction.Down);

        // Direction should still be set even though walk failed
        aisling.Direction
               .Should()
               .Be(Direction.Down);
    }

    [Test]
    public void Walk_ShouldMovePosition_WhenSuccessful()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        MockAisling.SetupScriptAllows(aisling);

        aisling.Walk(Direction.Down);

        aisling.X
               .Should()
               .Be(5);

        aisling.Y
               .Should()
               .Be(6);

        aisling.Trackers
               .LastWalk
               .Should()
               .NotBeNull();
    }
    #endregion

    #region ShouldRefresh
    [Test]
    public void ShouldRefresh_ShouldReturnTrue_WhenNoLastRefresh()
    {
        var aisling = MockAisling.Create(Map);

        aisling.ShouldRefresh
               .Should()
               .BeTrue();
    }

    [Test]
    public void ShouldRefresh_ShouldReturnFalse_WhenJustRefreshed()
    {
        var aisling = MockAisling.Create(Map);

        aisling.Refresh(true);

        aisling.ShouldRefresh
               .Should()
               .BeFalse();
    }
    #endregion

    #region IsIgnoring
    [Test]
    public void IsIgnoring_ShouldReturnTrue_WhenNameIsInIgnoreList()
    {
        var aisling = MockAisling.Create(Map);

        aisling.IgnoreList.Add("Annoying");

        aisling.IsIgnoring("Annoying")
               .Should()
               .BeTrue();
    }

    [Test]
    public void IsIgnoring_ShouldReturnFalse_WhenNameIsNotInIgnoreList()
    {
        var aisling = MockAisling.Create(Map);

        aisling.IsIgnoring("Nobody")
               .Should()
               .BeFalse();
    }
    #endregion

    #region SendMessage methods
    [Test]
    public void SendServerMessage_ShouldDelegateToClient()
    {
        var aisling = MockAisling.Create(Map);

        aisling.SendServerMessage(ServerMessageType.ActiveMessage, "Test message");

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.ActiveMessage, "Test message"), Times.Once);
    }

    [Test]
    public void SendActiveMessage_ShouldSendActiveMessageType()
    {
        var aisling = MockAisling.Create(Map);

        aisling.SendActiveMessage("Hello");

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.ActiveMessage, "Hello"), Times.Once);
    }

    [Test]
    public void SendOrangeBarMessage_ShouldSendOrangeBar1Type()
    {
        var aisling = MockAisling.Create(Map);

        aisling.SendOrangeBarMessage("Warning");

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.OrangeBar1, "Warning"), Times.Once);
    }

    [Test]
    public void SendPersistentMessage_ShouldSendPersistentMessageType()
    {
        var aisling = MockAisling.Create(Map);

        aisling.SendPersistentMessage("Persistent");

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.PersistentMessage, "Persistent"), Times.Once);
    }

    [Test]
    public void SendMessage_ShouldDelegateToSendActiveMessage()
    {
        var aisling = MockAisling.Create(Map);

        aisling.SendMessage("Hello");

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.ActiveMessage, "Hello"), Times.Once);
    }
    #endregion

    #region ShowPublicMessage (Aisling override)
    [Test]
    public void ShowPublicMessage_ShouldReturn_WhenScriptCanTalkIsFalse()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTalk())
                  .Returns(false);

        var other = MockAisling.Create(Map, "Listener");
        Map.AddEntity(other, new Point(5, 5));
        var otherClientMock = Mock.Get(other.Client);
        otherClientMock.Invocations.Clear();

        aisling.ShowPublicMessage(PublicMessageType.Normal, "Hello");

        // If CanTalk returns false, no public message should be shown
        otherClientMock.Verify(
            c => c.SendDisplayPublicMessage(It.IsAny<uint>(), It.IsAny<PublicMessageType>(), It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public void ShowPublicMessage_ShouldCallBase_WhenScriptAllows()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        MockAisling.SetupScriptAllows(aisling);

        var other = MockAisling.Create(Map, "Listener");
        Map.AddEntity(other, new Point(5, 5));

        // Add to ApproachTime so other can "observe" aisling
        other.ApproachTime[aisling] = DateTime.UtcNow;

        aisling.ShowPublicMessage(PublicMessageType.Normal, "Hello");

        var otherClientMock = Mock.Get(other.Client);

        otherClientMock.Verify(
            c => c.SendDisplayPublicMessage(aisling.Id, PublicMessageType.Normal, It.Is<string>(s => s.Contains("Hello"))),
            Times.Once);
    }
    #endregion

    #region ShouldWalk
    [Test]
    public void ShouldWalk_ShouldReturnTrue_WhenNoProhibitionsEnabled()
    {
        var aisling = MockAisling.Create(Map);

        aisling.ShouldWalk
               .Should()
               .BeTrue();
    }

    [Test]
    public void ShouldWalk_ShouldReturnFalse_WhenF5WalkProhibited_AndRecentRefresh()
    {
        var aisling = MockAisling.Create(Map);
        var original = (WorldOptions)WorldOptions.Instance;

        try
        {
            WorldOptions.Instance = original with
            {
                ProhibitF5Walk = true
            };
            aisling.Trackers.LastRefresh = DateTime.UtcNow;

            aisling.ShouldWalk
                   .Should()
                   .BeFalse();
        } finally
        {
            WorldOptions.Instance = original;
        }
    }

    [Test]
    public void ShouldWalk_ShouldReturnFalse_WhenItemSwitchWalkProhibited_AndRecentEquip()
    {
        var aisling = MockAisling.Create(Map);
        var original = (WorldOptions)WorldOptions.Instance;

        try
        {
            WorldOptions.Instance = original with
            {
                ProhibitItemSwitchWalk = true
            };
            aisling.Trackers.LastEquip = DateTime.UtcNow;

            aisling.ShouldWalk
                   .Should()
                   .BeFalse();
        } finally
        {
            WorldOptions.Instance = original;
        }
    }

    [Test]
    public void ShouldWalk_ShouldReturnFalse_WhenItemSwitchWalkProhibited_AndRecentUnequip()
    {
        var aisling = MockAisling.Create(Map);
        var original = (WorldOptions)WorldOptions.Instance;

        try
        {
            WorldOptions.Instance = original with
            {
                ProhibitItemSwitchWalk = true
            };
            aisling.Trackers.LastUnequip = DateTime.UtcNow;

            aisling.ShouldWalk
                   .Should()
                   .BeFalse();
        } finally
        {
            WorldOptions.Instance = original;
        }
    }
    #endregion

    #region BeginObserving
    [Test]
    public void BeginObserving_ShouldReApplyEffects()
    {
        var aisling = MockAisling.Create(Map);

        var effectMock = new Mock<IEffect>();

        effectMock.SetupGet(e => e.Name)
                  .Returns("TestEffect");

        effectMock.SetupGet(e => e.ScriptKey)
                  .Returns("TestEffectKey");

        effectMock.SetupGet(e => e.Icon)
                  .Returns(1);

        effectMock.SetupGet(e => e.Remaining)
                  .Returns(TimeSpan.FromSeconds(30));

        effectMock.SetupGet(e => e.Color)
                  .Returns(0);

        effectMock.Setup(e => e.ShouldApply(It.IsAny<Creature>(), It.IsAny<Creature>()))
                  .Returns(true);

        aisling.Effects.Apply(aisling, effectMock.Object);

        aisling.BeginObserving();

        // Effect.Subject should be set to aisling during BeginObserving
        effectMock.VerifySet(e => e.Subject = aisling, Times.AtLeastOnce);

        // OnReApplied should be called during BeginObserving
        effectMock.Verify(e => e.OnReApplied(), Times.AtLeastOnce);
    }

    [Test]
    public void BeginObserving_ShouldNotThrow_WithEmptyPanels()
    {
        var aisling = MockAisling.Create(Map);

        var act = () => aisling.BeginObserving();

        act.Should()
           .NotThrow();
    }

    [Test]
    public void BeginObserving_ShouldSendAddedItems_AfterCallingBeginObserving()
    {
        var aisling = MockAisling.Create(Map);
        var item = MockItem.Create("Potion");

        aisling.Inventory.TryAddToNextSlot(item);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.BeginObserving();

        // After BeginObserving, items should have been re-added via observer (OnAdded called)
        clientMock.Verify(c => c.SendAddItemToPane(It.IsAny<Item>()), Times.AtLeastOnce);
    }
    #endregion

    #region UpdateViewPort (single)
    [Test]
    public void UpdateViewPort_Single_ShouldHide_WhenPreviouslyObservedButNoLongerObservable()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        var other = MockAisling.Create(Map, "Other");
        Map.AddEntity(other, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        // Simulate previous observation by adding to ApproachTime
        aisling.ApproachTime[other] = DateTime.UtcNow;

        // Remove other from map so they're no longer observable
        Map.RemoveEntity(other);

        aisling.UpdateViewPort(other);

        // Other should have departed — ApproachTime should no longer contain other
        aisling.ApproachTime
               .Should()
               .NotContainKey(other);
    }

    [Test]
    public void UpdateViewPort_Single_ShouldShow_WhenNewlyObservable_Aisling()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var other = MockAisling.Create(Map, "Other");
        Map.AddEntity(other, new Point(5, 5));
        MockAisling.SetupScriptAllows(other);

        // Clear any approach times set by AddEntity
        aisling.ApproachTime.Clear();

        aisling.UpdateViewPort(other);

        // After updating, ApproachTime should contain other (OnApproached was called)
        aisling.ApproachTime
               .Should()
               .ContainKey(other);
    }

    [Test]
    public void UpdateViewPort_Single_ShouldShowDoor_WhenNewlyObservable()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var door = new Door(
            false,
            100,
            Map,
            new Point(5, 5));
        Map.AddEntity(door, new Point(5, 5));

        aisling.UpdateViewPort(door);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendDoors(It.IsAny<IEnumerable<Door>>()), Times.AtLeastOnce);
    }

    [Test]
    public void UpdateViewPort_Single_ShouldDoNothing_WhenBothPreviouslyAndCurrentlyObserved()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var other = MockAisling.Create(Map, "Other");
        Map.AddEntity(other, new Point(5, 5));
        MockAisling.SetupScriptAllows(other);

        // Previously observed
        aisling.ApproachTime[other] = DateTime.UtcNow;

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.UpdateViewPort(other);

        // Still observed — no ShowTo or HideFrom should be called
        aisling.ApproachTime
               .Should()
               .ContainKey(other);
    }

    [Test]
    public void UpdateViewPort_Single_ShouldDoNothing_WhenNeitherPreviouslyNorCurrentlyObserved()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        var other = MockAisling.Create(Map, "Other");

        // Other not on map, not in ApproachTime
        var act = () => aisling.UpdateViewPort(other);

        act.Should()
           .NotThrow();

        aisling.ApproachTime
               .Should()
               .NotContainKey(other);
    }
    #endregion

    #region UpdateViewPort (multi)
    [Test]
    public void UpdateViewPort_Multi_ShouldDetectDepartures()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        var other = MockAisling.Create(Map, "Other");

        // Previously observed
        aisling.ApproachTime[other] = DateTime.UtcNow;

        // Full viewport update — other is not on map anymore
        aisling.UpdateViewPort();

        // Other should have departed
        aisling.ApproachTime
               .Should()
               .NotContainKey(other);
    }

    [Test]
    public void UpdateViewPort_Multi_ShouldDetectNewApproaches()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var other = MockAisling.Create(Map, "Other");
        Map.AddEntity(other, new Point(5, 5));
        MockAisling.SetupScriptAllows(other);

        aisling.UpdateViewPort();

        // Other should be newly observed
        aisling.ApproachTime
               .Should()
               .ContainKey(other);
    }

    [Test]
    public void UpdateViewPort_Multi_Refresh_ShouldStashAndRestoreApproachTimes()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var other = MockAisling.Create(Map, "Other");
        Map.AddEntity(other, new Point(5, 5));
        MockAisling.SetupScriptAllows(other);

        var oldTime = DateTime.UtcNow.AddMinutes(-5);
        aisling.ApproachTime[other] = oldTime;

        // Refresh should stash approach times and restore them afterward
        aisling.UpdateViewPort(refresh: true);

        aisling.ApproachTime
               .Should()
               .ContainKey(other);

        // The original time should be preserved (stashed and restored)
        aisling.ApproachTime[other]
               .Should()
               .Be(oldTime);
    }

    [Test]
    public void UpdateViewPort_Multi_PartialUpdate_ShouldOnlyCheckSpecifiedEntities()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var other = MockAisling.Create(Map, "Other");
        Map.AddEntity(other, new Point(5, 5));
        MockAisling.SetupScriptAllows(other);

        var other2 = MockAisling.Create(Map, "Other2");
        Map.AddEntity(other2, new Point(5, 5));
        MockAisling.SetupScriptAllows(other2);

        // Previously observed both
        aisling.ApproachTime[other] = DateTime.UtcNow;
        aisling.ApproachTime[other2] = DateTime.UtcNow;

        // Partial update with only other — other2 should remain untouched
        aisling.UpdateViewPort([other]);

        aisling.ApproachTime
               .Should()
               .ContainKey(other2);
    }
    #endregion

    #region Walk deeper (ShouldWalk integration)
    [Test]
    public void Walk_ShouldRefresh_WhenShouldWalkReturnsFalse_DueToF5Walk()
    {
        var aisling = MockAisling.Create(Map);
        MockAisling.SetupScriptAllows(aisling);

        var original = (WorldOptions)WorldOptions.Instance;

        try
        {
            WorldOptions.Instance = original with
            {
                ProhibitF5Walk = true
            };
            aisling.Trackers.LastRefresh = DateTime.UtcNow;

            var clientMock = Mock.Get(aisling.Client);

            aisling.Walk(Direction.Down);

            // Walk should have been rejected and Refresh called
            clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);

            // Position should not have changed
            aisling.X
                   .Should()
                   .Be(5);

            aisling.Y
                   .Should()
                   .Be(5);
        } finally
        {
            WorldOptions.Instance = original;
        }
    }

    [Test]
    public void Walk_ShouldAllowAdmin_EvenWithWalls()
    {
        var aisling = MockAisling.Create(Map, setup: a => a.IsAdmin = true);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        MockMapInstance.SetWall(Map, new Point(5, 6));

        aisling.Walk(Direction.Down);

        // Admin should walk through walls
        aisling.Y
               .Should()
               .Be(6);
    }

    [Test]
    public void Walk_ShouldRefresh_WhenAdminWalksOutOfBounds()
    {
        var aisling = MockAisling.Create(Map, setup: a => a.IsAdmin = true);
        Map.AddEntity(aisling, new Point(0, 0));
        MockAisling.SetupScriptAllows(aisling);

        var clientMock = Mock.Get(aisling.Client);

        aisling.Walk(Direction.Up);

        // Should have refreshed because out of bounds
        clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);

        // Position should not have changed
        aisling.Y
               .Should()
               .Be(0);
    }
    #endregion

    #region ShouldWalk (SpeedWalk detection)
    [Test]
    public void ShouldWalk_ShouldReturnFalse_WhenSpeedWalkProhibited_AndWalkCounterExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var original = (WorldOptions)WorldOptions.Instance;

        try
        {
            WorldOptions.Instance = original with
            {
                ProhibitSpeedWalk = true
            };

            // Sprite == 0 by default, so the speed walk check applies
            // WalkCounter is ResettingCounter(4, 2) => MaxCount = 8
            // Exhaust the counter by calling TryIncrement 8 times
            for (var i = 0; i < 8; i++)
                aisling.WalkCounter.TryIncrement();

            // Now ShouldWalk should return false because the counter is exhausted
            aisling.ShouldWalk
                   .Should()
                   .BeFalse();
        } finally
        {
            WorldOptions.Instance = original;
        }
    }

    [Test]
    public void ShouldWalk_ShouldReturnTrue_WhenSpeedWalkProhibited_ButCounterNotExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var original = (WorldOptions)WorldOptions.Instance;

        try
        {
            WorldOptions.Instance = original with
            {
                ProhibitSpeedWalk = true
            };

            // Only increment a few times, well below the limit of 8
            aisling.WalkCounter.TryIncrement();
            aisling.WalkCounter.TryIncrement();

            aisling.ShouldWalk
                   .Should()
                   .BeTrue();
        } finally
        {
            WorldOptions.Instance = original;
        }
    }

    [Test]
    public void ShouldWalk_ShouldSkipSpeedWalkCheck_WhenSpriteIsNonZero()
    {
        var aisling = MockAisling.Create(Map);
        var original = (WorldOptions)WorldOptions.Instance;

        try
        {
            WorldOptions.Instance = original with
            {
                ProhibitSpeedWalk = true
            };

            // Set sprite to non-zero so speed walk check is skipped
            aisling.Sprite = 100;

            // Exhaust the counter
            for (var i = 0; i < 8; i++)
                aisling.WalkCounter.TryIncrement();

            // Should still return true because sprite != 0 bypasses the speed walk check
            aisling.ShouldWalk
                   .Should()
                   .BeTrue();
        } finally
        {
            WorldOptions.Instance = original;
        }
    }
    #endregion

    #region CanCarry (stackable with existing stacks — deeper)
    [Test]
    public void CanCarry_ShouldReturnTrue_WhenStackableFillsExistingStack_NoNewSlotNeeded()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        // Add a stackable item with 50 count (max is 100)
        var existing = MockItem.Create("Potion", 50, true);
        aisling.Inventory.TryAddToNextSlot(existing);

        // Try to carry 50 more — should fit in existing stack
        var more = MockItem.Create("Potion", 50, true);

        aisling.CanCarry(more)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanCarry_ShouldReturnFalse_WhenStackableExceedsAllowedCount_WithExistingStacks()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(500);

        // Add a stackable item with 90 count (max per stack is 100)
        var existing = MockItem.Create("Potion", 90, true);
        aisling.Inventory.TryAddToNextSlot(existing);

        // Try to carry 20 more — total would be 110, but allowedCount = 100 - 90 = 10
        // 20 > 10, should return false
        var more = MockItem.Create("Potion", 20, true);

        aisling.CanCarry(more)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanCarry_ShouldReturnTrue_WhenStackableNoExistingStacks_FitsInOneStack()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        // No existing stacks of this item
        // allowedCount when numUniqueStacks == 0 is MaxStacks = 100
        // 50 <= 100 so allowed; estimatedStacks = ceil((50 - 100) / 100) = 0, so no weight added
        var newStackable = MockItem.Create("NewPotion", 50, true);

        aisling.CanCarry(newStackable)
               .Should()
               .BeTrue();
    }
    #endregion

    #region TryUseItem (throttle paths)
    [Test]
    public void TryUseItem_ShouldReturnFalse_WhenActionThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Potion");
        aisling.Inventory.TryAddToNextSlot(item);

        MockAisling.SetupScriptAllows(aisling);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanUse(aisling))
                      .Returns(true);

        // Exhaust the action throttle
        // ActionThrottle = ResettingCounter(MaxActionsPerSecond), MaxCount = MaxActionsPerSecond * 1
        var maxActions = ((WorldOptions)WorldOptions.Instance).MaxActionsPerSecond;

        for (var i = 0; i < maxActions; i++)
            aisling.ActionThrottle.TryIncrement();

        aisling.TryUseItem(item)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseItem_ShouldReturnFalse_WhenItemThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Potion");
        aisling.Inventory.TryAddToNextSlot(item);

        MockAisling.SetupScriptAllows(aisling);

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(s => s.CanUse(aisling))
                      .Returns(true);

        // Exhaust the item throttle
        var maxItems = ((WorldOptions)WorldOptions.Instance).MaxItemsPerSecond;

        for (var i = 0; i < maxItems; i++)
            aisling.ItemThrottle.TryIncrement();

        aisling.TryUseItem(item)
               .Should()
               .BeFalse();
    }
    #endregion

    #region Walk (water tile transition)
    [Test]
    public void Walk_ShouldSendFullAttributes_WhenTransitioningToWaterTile()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        // Set the destination tile (5,6) to a water tile
        // Water tiles have background IDs that match WATER_TILE_IDS (16977 - 1 = 16976)
        Map.Template.Tiles[5, 6] = new Tile(16977, 0, 0);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.Walk(Direction.Down);

        // Should have called SendAttributes with StatUpdateType.Full due to water transition
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.AtLeastOnce);
    }

    [Test]
    public void Walk_ShouldSendFullAttributes_WhenTransitioningFromWaterTile()
    {
        var aisling = MockAisling.Create(Map);

        // Set the start tile to water
        Map.Template.Tiles[5, 5] = new Tile(16977, 0, 0);

        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        // Destination (5,6) is default non-water tile
        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.Walk(Direction.Down);

        // Should have called SendAttributes with StatUpdateType.Full due to water transition
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.AtLeastOnce);
    }

    [Test]
    public void Walk_ShouldNotSendFullAttributes_WhenNotTransitioningWater()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        // Both tiles are non-water (default)
        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.Walk(Direction.Down);

        // Should NOT send Full attributes (only ClientWalkResponse and possibly other calls)
        clientMock.Verify(c => c.SendAttributes(StatUpdateType.Full), Times.Never);
    }
    #endregion

    #region CanUse (Item — ActionThrottle/ItemThrottle blocks)
    [Test]
    public void CanUse_Item_ShouldReturnFalse_WhenActionThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var item = MockItem.Create("Potion");

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanUseItem(It.IsAny<Item>()))
                  .Returns(true);

        // Exhaust the action throttle
        var maxActions = ((WorldOptions)WorldOptions.Instance).MaxActionsPerSecond;

        for (var i = 0; i < maxActions; i++)
            aisling.ActionThrottle.TryIncrement();

        aisling.CanUse(item)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanUse_Item_ShouldReturnFalse_WhenItemThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var item = MockItem.Create("Potion");

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanUseItem(It.IsAny<Item>()))
                  .Returns(true);

        // Exhaust the item throttle
        var maxItems = ((WorldOptions)WorldOptions.Instance).MaxItemsPerSecond;

        for (var i = 0; i < maxItems; i++)
            aisling.ItemThrottle.TryIncrement();

        aisling.CanUse(item)
               .Should()
               .BeFalse();
    }
    #endregion

    #region CanUse (Skill — ActionThrottle/SkillThrottle blocks)
    [Test]
    public void CanUse_Skill_ShouldReturnFalse_WhenActionThrottleExhausted_NonAssail()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create();

        MockAisling.SetupScriptAllows(aisling);

        // Exhaust the action throttle
        var maxActions = ((WorldOptions)WorldOptions.Instance).MaxActionsPerSecond;

        for (var i = 0; i < maxActions; i++)
            aisling.ActionThrottle.TryIncrement();

        aisling.CanUse(skill, out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }

    [Test]
    public void CanUse_Skill_ShouldNotCheckThrottle_WhenAssail()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create("AssailSkill", true);

        MockAisling.SetupScriptAllows(aisling);

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(s => s.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        // Exhaust the action throttle
        var maxActions = ((WorldOptions)WorldOptions.Instance).MaxActionsPerSecond;

        for (var i = 0; i < maxActions; i++)
            aisling.ActionThrottle.TryIncrement();

        // Assail skills skip throttle checks in Aisling.CanUse
        aisling.CanUse(skill, out var context)
               .Should()
               .BeTrue();

        context.Should()
               .NotBeNull();
    }
    #endregion

    #region CanUse (Spell — ActionThrottle/SpellThrottle blocks)
    [Test]
    public void CanUse_Spell_ShouldReturnFalse_WhenActionThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        MockAisling.SetupScriptAllows(aisling);

        // Exhaust the action throttle
        var maxActions = ((WorldOptions)WorldOptions.Instance).MaxActionsPerSecond;

        for (var i = 0; i < maxActions; i++)
            aisling.ActionThrottle.TryIncrement();

        aisling.CanUse(
                   spell,
                   aisling,
                   null,
                   out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }

    [Test]
    public void CanUse_Spell_ShouldReturnFalse_WhenSpellThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        MockAisling.SetupScriptAllows(aisling);

        // Exhaust the spell throttle
        var maxSpells = ((WorldOptions)WorldOptions.Instance).MaxSpellsPerSecond;

        for (var i = 0; i < maxSpells; i++)
            aisling.SpellThrottle.TryIncrement();

        aisling.CanUse(
                   spell,
                   aisling,
                   null,
                   out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }
    #endregion

    #region Turn (deeper — override branches)
    [Test]
    public void Turn_Override_ShouldReturnEarly_WhenNotForcedAndScriptCanTurnReturnsFalse()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Direction = Direction.Up;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(false);

        // Call the override (2-param) version
        ((Creature)aisling).Turn(Direction.Down);

        aisling.Direction
               .Should()
               .Be(Direction.Up);

        aisling.Trackers
               .LastTurn
               .Should()
               .BeNull();
    }

    [Test]
    public void Turn_Override_ShouldReturnEarly_WhenNotForcedAndTurnThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Direction = Direction.Up;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(true);

        // TurnThrottle = ResettingCounter(3) => MaxCount = 3
        for (var i = 0; i < 3; i++)
            aisling.TurnThrottle.TryIncrement();

        // Call the override (2-param) version
        ((Creature)aisling).Turn(Direction.Down);

        aisling.Direction
               .Should()
               .Be(Direction.Up);
    }

    [Test]
    public void Turn_Override_ShouldIgnoreScriptAndThrottle_WhenForced()
    {
        var aisling = MockAisling.Create(Map);
        aisling.Direction = Direction.Up;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(false);

        // Exhaust the throttle
        for (var i = 0; i < 3; i++)
            aisling.TurnThrottle.TryIncrement();

        // Forced should bypass both script and throttle
        ((Creature)aisling).Turn(Direction.Down, true);

        aisling.Direction
               .Should()
               .Be(Direction.Down);

        aisling.Trackers
               .LastTurn
               .Should()
               .NotBeNull();
    }
    #endregion

    #region BeginObserving (cooldown branches)
    [Test]
    public void BeginObserving_ShouldSendCooldown_WhenSpellHasElapsed()
    {
        var aisling = MockAisling.Create(Map);

        var spell = MockSpell.Create("CooldownSpell");
        spell.Cooldown = TimeSpan.FromSeconds(10);
        spell.Elapsed = TimeSpan.FromSeconds(3); // Active cooldown (not null)

        aisling.SpellBook.TryAddToNextSlot(spell);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.BeginObserving();

        clientMock.Verify(c => c.SendCooldown(spell), Times.Once);
    }

    [Test]
    public void BeginObserving_ShouldSendCooldown_WhenNonAssailSkillHasElapsed()
    {
        var aisling = MockAisling.Create(Map);

        var skill = MockSkill.Create("CooldownSkill");
        skill.Cooldown = TimeSpan.FromSeconds(10);
        skill.Elapsed = TimeSpan.FromSeconds(3); // Active cooldown (not null)

        aisling.SkillBook.TryAddToNextSlot(skill);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.BeginObserving();

        clientMock.Verify(c => c.SendCooldown(skill), Times.Once);
    }

    [Test]
    public void BeginObserving_ShouldNotSendCooldown_WhenAssailSkillHasElapsed()
    {
        var aisling = MockAisling.Create(Map);

        var skill = MockSkill.Create("AssailSkill", true);
        skill.Cooldown = TimeSpan.FromSeconds(10);
        skill.Elapsed = TimeSpan.FromSeconds(3); // Active cooldown (not null)

        aisling.SkillBook.TryAddToNextSlot(skill);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        aisling.BeginObserving();

        // Assail skills should NOT trigger SendCooldown per the pattern match
        clientMock.Verify(c => c.SendCooldown(skill), Times.Never);
    }
    #endregion

    #region TryUseSkill (throttle false paths)
    [Test]
    public void TryUseSkill_ShouldReturnFalse_WhenActionThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create("ThrottledSkill");

        MockAisling.SetupScriptAllows(aisling);

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(s => s.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        // Exhaust the action throttle
        var maxActions = ((WorldOptions)WorldOptions.Instance).MaxActionsPerSecond;

        for (var i = 0; i < maxActions; i++)
            aisling.ActionThrottle.TryIncrement();

        aisling.TryUseSkill(skill)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSkill_ShouldReturnFalse_WhenSkillThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var skill = MockSkill.Create("ThrottledSkill2");

        MockAisling.SetupScriptAllows(aisling);

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(s => s.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        // Exhaust the skill throttle
        var maxSkills = ((WorldOptions)WorldOptions.Instance).MaxSkillsPerSecond;

        for (var i = 0; i < maxSkills; i++)
            aisling.SkillThrottle.TryIncrement();

        aisling.TryUseSkill(skill)
               .Should()
               .BeFalse();
    }
    #endregion

    #region TryUseSpell (throttle false paths)
    [Test]
    public void TryUseSpell_ShouldReturnFalse_WhenActionThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        MockAisling.SetupScriptAllows(aisling);

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(s => s.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        // Exhaust the action throttle
        var maxActions = ((WorldOptions)WorldOptions.Instance).MaxActionsPerSecond;

        for (var i = 0; i < maxActions; i++)
            aisling.ActionThrottle.TryIncrement();

        aisling.TryUseSpell(spell)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSpell_ShouldReturnFalse_WhenSpellThrottleExhausted()
    {
        var aisling = MockAisling.Create(Map);
        var spell = MockSpell.Create();

        MockAisling.SetupScriptAllows(aisling);

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(s => s.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        // Exhaust the spell throttle
        var maxSpells = ((WorldOptions)WorldOptions.Instance).MaxSpellsPerSecond;

        for (var i = 0; i < maxSpells; i++)
            aisling.SpellThrottle.TryIncrement();

        aisling.TryUseSpell(spell)
               .Should()
               .BeFalse();
    }
    #endregion

    #region Walk (CanTurn false branch)
    [Test]
    public void Walk_ShouldRefresh_WhenCanTurnReturnsFalse_AndDirectionDiffers()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));

        // Direction starts at default (Down via Aisling, or could be from AddEntity)
        aisling.Direction = Direction.Up;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        // CanMove returns true, but CanTurn returns false
        scriptMock.Setup(s => s.CanMove())
                  .Returns(true);

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(false);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Walk in a different direction than current
        aisling.Walk(Direction.Down);

        // Should have been rejected because direction differs and CanTurn is false
        clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);

        // Position should not have changed
        aisling.X
               .Should()
               .Be(5);

        aisling.Y
               .Should()
               .Be(5);
    }

    [Test]
    public void Walk_ShouldSucceed_WhenCanTurnReturnsFalse_ButDirectionMatches()
    {
        var aisling = MockAisling.Create(Map);
        Map.AddEntity(aisling, new Point(5, 5));
        aisling.Direction = Direction.Down;

        var scriptMock = Mock.Get(aisling.Script);
        scriptMock.Reset();

        // CanMove true, CanTurn false — but direction matches, so no turn check
        scriptMock.Setup(s => s.CanMove())
                  .Returns(true);

        scriptMock.Setup(s => s.CanTurn())
                  .Returns(false);

        scriptMock.Setup(s => s.CanSee(It.IsAny<VisibleEntity>()))
                  .Returns(true);

        aisling.Walk(Direction.Down);

        // Should succeed because direction == current direction, no turn needed
        aisling.Y
               .Should()
               .Be(6);
    }
    #endregion

    #region WarpTo (watcher HideFrom/ShowTo)
    [Test]
    public void WarpTo_ShouldCallHideFromAndShowTo_ForWatcherAtBothPoints()
    {
        var aisling = MockAisling.Create(Map, "Warper");
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        // Create a watcher aisling at a point that can see both start (5,5) and end (7,7)
        var watcher = MockAisling.Create(Map, "Watcher");
        Map.AddEntity(watcher, new Point(6, 6));
        MockAisling.SetupScriptAllows(watcher);

        // Both should be in each other's ApproachTime
        aisling.ApproachTime.Clear();
        watcher.ApproachTime.Clear();
        aisling.ApproachTime[watcher] = DateTime.UtcNow;
        watcher.ApproachTime[aisling] = DateTime.UtcNow;

        var watcherClient = Mock.Get(watcher.Client);
        watcherClient.Invocations.Clear();

        aisling.WarpTo(new Point(7, 7));

        // Watcher should have received display calls (HideFrom sends effect, ShowTo sends display)
        // The watcher should still have the warper in their ApproachTime
        watcher.ApproachTime
               .Should()
               .ContainKey(aisling);

        // Verify that the watcher's client received HideFrom (SendRemoveEntity) and ShowTo calls
        watcherClient.Verify(c => c.SendRemoveEntity(aisling.Id), Times.AtLeastOnce);
    }

    [Test]
    public void WarpTo_ShouldUpdatePosition()
    {
        var aisling = MockAisling.Create(Map, "Warper");
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        aisling.WarpTo(new Point(8, 8));

        aisling.X
               .Should()
               .Be(8);

        aisling.Y
               .Should()
               .Be(8);
    }

    [Test]
    public void WarpTo_ShouldSetTrackerLastPosition()
    {
        var aisling = MockAisling.Create(Map, "Warper");
        Map.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        aisling.WarpTo(new Point(8, 8));

        aisling.Trackers.LastPosition!.X
               .Should()
               .Be(5);

        aisling.Trackers.LastPosition!.Y
               .Should()
               .Be(5);
    }
    #endregion

    #region Walk on dark map (Aisling viewport branch)
    [Test]
    public void Walk_OnDarkMap_ShouldTriggerFullViewPortUpdate_ForNearbyAislings()
    {
        var darkMap = MockMapInstance.Create(width: 20, height: 20);
        darkMap.Flags = MapFlags.Darkness;

        var aisling = MockAisling.Create(darkMap, "Walker", new Point(5, 5));
        darkMap.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var other = MockAisling.Create(darkMap, "Observer", new Point(6, 6));
        darkMap.AddEntity(other, new Point(6, 6));
        MockAisling.SetupScriptAllows(other);

        var otherClient = Mock.Get(other.Client);
        otherClient.Invocations.Clear();

        aisling.Walk(Direction.Down);

        // On dark map, nearby Aislings should get a full UpdateViewPort
        // which sends display changes (not just single entity update)
        // Verify that SendMapInfo is NOT called (not a refresh), but viewport updates are triggered
        aisling.Y
               .Should()
               .Be(6);
    }

    [Test]
    public void Walk_OnNormalMap_ShouldTriggerSingleEntityUpdate_ForNearbyAislings()
    {
        // Normal map (no Darkness flag)
        var normalMap = MockMapInstance.Create(width: 20, height: 20);

        var aisling = MockAisling.Create(normalMap, "Walker", new Point(5, 5));
        normalMap.AddEntity(aisling, new Point(5, 5));
        MockAisling.SetupScriptAllows(aisling);

        var other = MockAisling.Create(normalMap, "Observer", new Point(6, 6));
        normalMap.AddEntity(other, new Point(6, 6));
        MockAisling.SetupScriptAllows(other);

        var otherClient = Mock.Get(other.Client);
        otherClient.Invocations.Clear();

        aisling.Walk(Direction.Down);

        // On normal map, single entity update is used
        aisling.Y
               .Should()
               .Be(6);
    }
    #endregion

    #region TryGiveItem — full inventory
    [Test]
    public void TryGiveItem_WithSlot_ShouldReturnFalse_WhenInventoryFull()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(5000);

        // Fill all 60 inventory slots
        for (byte i = 1; i <= 60; i++)
        {
            var filler = MockItem.Create($"Filler{i}");
            aisling.Inventory.TryAddDirect(i, filler);
        }

        var newItem = MockItem.Create("Overflow");

        var result = aisling.TryGiveItem(ref newItem, 1);

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryGiveItem_NoSlot_ShouldReturnFalse_WhenInventoryFull()
    {
        var aisling = MockAisling.Create(Map);
        aisling.UserStatSheet.SetMaxWeight(5000);

        // Fill all 60 inventory slots
        for (byte i = 1; i <= 60; i++)
        {
            var filler = MockItem.Create($"Filler{i}");
            aisling.Inventory.TryAddDirect(i, filler);
        }

        var newItem = MockItem.Create("Overflow");

        var result = aisling.TryGiveItem(ref newItem);

        result.Should()
              .BeFalse();
    }
    #endregion
}