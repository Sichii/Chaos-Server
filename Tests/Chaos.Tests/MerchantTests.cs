#region
using Chaos.Collections;
using Chaos.Collections.Common;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Other.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class MerchantTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region Activate
    [Test]
    public void Activate_ShouldCallScriptOnClicked()
    {
        var merchant = MockMerchant.Create(Map);
        var aisling = MockAisling.Create(Map, "Clicker");

        var scriptMock = Mock.Get(merchant.Script);
        scriptMock.Reset();

        ((IDialogSourceEntity)merchant).Activate(aisling);

        scriptMock.Verify(s => s.OnClicked(aisling), Times.Once);
    }
    #endregion

    #region Constructor — ItemsForSale StockService Registration
    [Test]
    public void Constructor_ShouldRegisterStock_WhenItemsForSaleHasEntries()
    {
        var itemsForSale = new CounterCollection(
            [
                new KeyValuePair<string, int>("healthPotion", 10),
                new KeyValuePair<string, int>("manaPotion", 5)
            ]);

        var template = new MerchantTemplate
        {
            Name = "ShopKeeper",
            TemplateKey = "shopkeeper",
            Sprite = 1,
            WanderIntervalMs = 1000,
            DefaultStock = new Dictionary<string, int>(),
            ItemsForSale = itemsForSale,
            ItemsToBuy = [],
            SkillsToTeach = [],
            SpellsToTeach = [],
            RestockIntervalHrs = 1,
            RestockPct = 100,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var loggerMock = new Mock<ILogger<Merchant>>();
        var skillFactoryMock = new Mock<ISkillFactory>();
        var spellFactoryMock = new Mock<ISpellFactory>();
        var itemFactoryMock = new Mock<IItemFactory>();
        var stockServiceMock = new Mock<IStockService>();

        // Set up item factory to return faux items for each key
        itemFactoryMock.Setup(f => f.CreateFaux("healthPotion"))
                       .Returns(() => MockItem.Create("HealthPotion"));

        itemFactoryMock.Setup(f => f.CreateFaux("manaPotion"))
                       .Returns(() => MockItem.Create("ManaPotion"));

        var merchant = new Merchant(
            template,
            Map,
            new Point(5, 5),
            loggerMock.Object,
            skillFactoryMock.Object,
            spellFactoryMock.Object,
            itemFactoryMock.Object,
            stockServiceMock.Object,
            MockScriptProvider.Instance.Object);

        // RegisterStock should have been called because ItemsForSale is not empty
        stockServiceMock.Verify(
            s => s.RegisterStock(
                "shopkeeper",
                It.IsAny<IEnumerable<(string, int)>>(),
                TimeSpan.FromHours(1),
                100),
            Times.Once);

        // Merchant should have the items in its ItemsForSale collection
        merchant.ItemsForSale
                .Should()
                .HaveCount(2);
    }
    #endregion

    #region OnClicked
    [Test]
    public void OnClicked_ShouldCallScriptOnClicked()
    {
        var merchant = MockMerchant.Create(Map);
        var aisling = MockAisling.Create(Map, "Clicker");

        var scriptMock = Mock.Get(merchant.Script);
        scriptMock.Reset();

        merchant.OnClicked(aisling);

        scriptMock.Verify(s => s.OnClicked(aisling), Times.Once);
    }
    #endregion

    #region Update (with non-zero delta)
    [Test]
    public void Update_ShouldCallScriptUpdate()
    {
        var merchant = MockMerchant.Create(Map);

        var scriptMock = Mock.Get(merchant.Script);
        scriptMock.Reset();

        merchant.Update(TimeSpan.FromMilliseconds(500));

        scriptMock.Verify(s => s.Update(It.IsAny<TimeSpan>()), Times.AtLeastOnce);
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldNotThrow()
    {
        var merchant = MockMerchant.Create(Map);

        var act = () => merchant.Update(TimeSpan.FromMilliseconds(100));

        act.Should()
           .NotThrow();
    }
    #endregion

    #region Properties
    [Test]
    public void IsAlive_ShouldAlwaysReturnTrue()
    {
        var merchant = MockMerchant.Create(Map);

        merchant.IsAlive
                .Should()
                .BeTrue();
    }

    [Test]
    public void EntityType_ShouldBeCreature()
    {
        var merchant = MockMerchant.Create(Map);
        var dialogSource = (IDialogSourceEntity)merchant;

        dialogSource.EntityType
                    .Should()
                    .Be(EntityType.Creature);
    }

    [Test]
    public void Type_ShouldBeMerchant()
    {
        var merchant = MockMerchant.Create(Map);

        merchant.Type
                .Should()
                .Be(CreatureType.Merchant);
    }

    [Test]
    public void StatSheet_ShouldBeMaxed()
    {
        var merchant = MockMerchant.Create(Map);

        // Merchant uses StatSheet.Maxed
        merchant.StatSheet
                .Should()
                .NotBeNull();
    }
    #endregion

    #region TryGetItem
    [Test]
    public void TryGetItem_ShouldReturnTrue_WhenItemExists()
    {
        var item = MockItem.Create("HealthPotion");

        var merchant = MockMerchant.Create(Map, setup: m => m.ItemsForSale.Add(item));

        merchant.TryGetItem("HealthPotion", out var found)
                .Should()
                .BeTrue();

        found.Should()
             .Be(item);
    }

    [Test]
    public void TryGetItem_ShouldReturnFalse_WhenItemDoesNotExist()
    {
        var merchant = MockMerchant.Create(Map);

        merchant.TryGetItem("NonExistent", out var found)
                .Should()
                .BeFalse();

        found.Should()
             .BeNull();
    }

    [Test]
    public void TryGetItem_ShouldBeCaseInsensitive()
    {
        var item = MockItem.Create("HealthPotion");

        var merchant = MockMerchant.Create(Map, setup: m => m.ItemsForSale.Add(item));

        merchant.TryGetItem("healthpotion", out var found)
                .Should()
                .BeTrue();

        found.Should()
             .Be(item);
    }
    #endregion

    #region TryGetSkill
    [Test]
    public void TryGetSkill_ShouldReturnTrue_WhenSkillExists()
    {
        var skill = MockSkill.Create("Punch");

        var merchant = MockMerchant.Create(Map, setup: m => m.SkillsToTeach.Add(skill));

        merchant.TryGetSkill("Punch", out var found)
                .Should()
                .BeTrue();

        found.Should()
             .Be(skill);
    }

    [Test]
    public void TryGetSkill_ShouldReturnFalse_WhenSkillDoesNotExist()
    {
        var merchant = MockMerchant.Create(Map);

        merchant.TryGetSkill("NonExistent", out var found)
                .Should()
                .BeFalse();

        found.Should()
             .BeNull();
    }

    [Test]
    public void TryGetSkill_ShouldBeCaseInsensitive()
    {
        var skill = MockSkill.Create("Punch");

        var merchant = MockMerchant.Create(Map, setup: m => m.SkillsToTeach.Add(skill));

        merchant.TryGetSkill("punch", out var found)
                .Should()
                .BeTrue();

        found.Should()
             .Be(skill);
    }
    #endregion

    #region TryGetSpell
    [Test]
    public void TryGetSpell_ShouldReturnTrue_WhenSpellExists()
    {
        var spell = MockSpell.Create("Fireball");

        var merchant = MockMerchant.Create(Map, setup: m => m.SpellsToTeach.Add(spell));

        merchant.TryGetSpell("Fireball", out var found)
                .Should()
                .BeTrue();

        found.Should()
             .Be(spell);
    }

    [Test]
    public void TryGetSpell_ShouldReturnFalse_WhenSpellDoesNotExist()
    {
        var merchant = MockMerchant.Create(Map);

        merchant.TryGetSpell("NonExistent", out var found)
                .Should()
                .BeFalse();

        found.Should()
             .BeNull();
    }

    [Test]
    public void TryGetSpell_ShouldBeCaseInsensitive()
    {
        var spell = MockSpell.Create("Fireball");

        var merchant = MockMerchant.Create(Map, setup: m => m.SpellsToTeach.Add(spell));

        merchant.TryGetSpell("fireball", out var found)
                .Should()
                .BeTrue();

        found.Should()
             .Be(spell);
    }
    #endregion

    #region IsBuying
    [Test]
    public void IsBuying_ShouldReturnTrue_WhenItemIsInBuyList()
    {
        var buyItem = MockItem.Create("OldSword");

        var merchant = MockMerchant.Create(Map, setup: m => m.ItemsToBuy.Add(buyItem));

        var sellItem = MockItem.Create("OldSword");

        merchant.IsBuying(sellItem)
                .Should()
                .BeTrue();
    }

    [Test]
    public void IsBuying_ShouldReturnFalse_WhenItemIsNotInBuyList()
    {
        var merchant = MockMerchant.Create(Map);

        var item = MockItem.Create("RandomItem");

        merchant.IsBuying(item)
                .Should()
                .BeFalse();
    }

    [Test]
    public void IsBuying_ShouldBeCaseInsensitive()
    {
        var buyItem = MockItem.Create("OldSword");

        var merchant = MockMerchant.Create(Map, setup: m => m.ItemsToBuy.Add(buyItem));

        var sellItem = MockItem.Create("oldsword");

        merchant.IsBuying(sellItem)
                .Should()
                .BeTrue();
    }
    #endregion

    #region OnGoldDroppedOn
    [Test]
    public void OnGoldDroppedOn_ShouldReturn_WhenOutOfRange()
    {
        var merchant = MockMerchant.Create(Map);
        Map.AddEntity(merchant, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Shopper");
        Map.AddEntity(aisling, new Point(9, 9));

        var scriptMock = Mock.Get(merchant.Script);
        scriptMock.Reset();

        merchant.OnGoldDroppedOn(aisling, 100);

        // Script should not have been called since out of range
        scriptMock.Verify(s => s.OnGoldDroppedOn(It.IsAny<Aisling>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldSendMessage_WhenScriptDisallows()
    {
        var merchant = MockMerchant.Create(Map);
        Map.AddEntity(merchant, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Shopper");
        Map.AddEntity(aisling, new Point(5, 5));

        var scriptMock = Mock.Get(merchant.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropMoneyOn(It.IsAny<Aisling>(), It.IsAny<int>()))
                  .Returns(false);

        merchant.OnGoldDroppedOn(aisling, 100);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendServerMessage(ServerMessageType.ActiveMessage, "You can't do that right now"), Times.Once);
        scriptMock.Verify(s => s.OnGoldDroppedOn(It.IsAny<Aisling>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldCallScript_WhenAllowed()
    {
        var merchant = MockMerchant.Create(Map);
        Map.AddEntity(merchant, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Shopper");
        Map.AddEntity(aisling, new Point(5, 5));

        var scriptMock = Mock.Get(merchant.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.CanDropMoneyOn(It.IsAny<Aisling>(), It.IsAny<int>()))
                  .Returns(true);

        merchant.OnGoldDroppedOn(aisling, 100);

        scriptMock.Verify(s => s.OnGoldDroppedOn(aisling, 100), Times.Once);
    }
    #endregion

    #region BlackList
    [Test]
    public void BlackList_ShouldBeInitiallyEmpty()
    {
        var merchant = MockMerchant.Create(Map);

        merchant.BlackList
                .Should()
                .BeEmpty();
    }

    [Test]
    public void BlackList_ShouldAcceptPoints()
    {
        var merchant = MockMerchant.Create(Map);

        merchant.BlackList.Add(new Point(1, 1));
        merchant.BlackList.Add(new Point(2, 3));

        merchant.BlackList
                .Should()
                .HaveCount(2);
    }
    #endregion

    #region Stock Service Delegation
    [Test]
    public void HasStock_ShouldDelegateToStockService()
    {
        var merchant = MockMerchant.Create(Map);

        // Access as IBuyShopSource to call explicit interface members
        var buyShop = (IBuyShopSource)merchant;

        buyShop.HasStock("test_item");

        var stockServiceMock = Mock.Get(merchant.StockService);

        stockServiceMock.Verify(s => s.HasStock(merchant.Template.TemplateKey, "test_item"), Times.Once);
    }

    [Test]
    public void TryDecrementStock_ShouldDelegateToStockService()
    {
        var merchant = MockMerchant.Create(Map);
        var buyShop = (IBuyShopSource)merchant;

        var stockServiceMock = Mock.Get(merchant.StockService);

        stockServiceMock.Setup(s => s.TryDecrementStock(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                        .Returns(true);

        var result = buyShop.TryDecrementStock("test_item", 5);

        result.Should()
              .BeTrue();

        stockServiceMock.Verify(s => s.TryDecrementStock(merchant.Template.TemplateKey, "test_item", 5), Times.Once);
    }

    [Test]
    public void GetStock_ShouldDelegateToStockService()
    {
        var merchant = MockMerchant.Create(Map);
        var buyShop = (IBuyShopSource)merchant;

        var stockServiceMock = Mock.Get(merchant.StockService);

        stockServiceMock.Setup(s => s.GetStock(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(42);

        var result = buyShop.GetStock("test_item");

        result.Should()
              .Be(42);

        stockServiceMock.Verify(s => s.GetStock(merchant.Template.TemplateKey, "test_item"), Times.Once);
    }
    #endregion

    #region OnItemDroppedOn (Creature.OnItemDroppedOn via Merchant)
    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenSlotIsEmpty()
    {
        var merchant = MockMerchant.Create(Map);
        Map.AddEntity(merchant, new Point(5, 5));

        var source = MockAisling.Create(Map, "Shopper");
        Map.AddEntity(source, new Point(5, 5));

        var scriptMock = Mock.Get(merchant.Script);
        scriptMock.Reset();

        // Slot 10 is empty in the inventory
        merchant.OnItemDroppedOn(source, 10, 1);

        scriptMock.Verify(s => s.OnItemDroppedOn(It.IsAny<Aisling>(), It.IsAny<Item>()), Times.Never);
    }

    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenCountExceedsItemCount()
    {
        var merchant = MockMerchant.Create(Map);
        Map.AddEntity(merchant, new Point(5, 5));

        var source = MockAisling.Create(Map, "Shopper");
        Map.AddEntity(source, new Point(5, 5));

        var item = MockItem.Create("TestItem", 3, true);
        source.Inventory.TryAddToNextSlot(item);

        var scriptMock = Mock.Get(merchant.Script);
        scriptMock.Reset();

        // Count 5 exceeds item.Count of 3
        merchant.OnItemDroppedOn(source, item.Slot, 5);

        scriptMock.Verify(s => s.OnItemDroppedOn(It.IsAny<Aisling>(), It.IsAny<Item>()), Times.Never);
    }
    #endregion

    #region Wander
    [Test]
    public void Wander_ShouldUseDefaultPathOptions_WhenNoneProvided()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        pathfinderMock.Setup(p => p.FindRandomDirection(It.IsAny<string>(), It.IsAny<IPoint>(), It.IsAny<IPathOptions>()))
                      .Returns(Direction.Invalid);

        var map = MockMapInstance.Create(setup: m => m.Pathfinder = pathfinderMock.Object);
        var merchant = MockMerchant.Create(map);
        map.AddEntity(merchant, new Point(5, 5));

        merchant.BlackList.Add(new Point(4, 5));

        // Should not throw — Wander uses null pathOptions which triggers the ??= default path
        var act = () => merchant.Wander();

        act.Should()
           .NotThrow();
    }

    [Test]
    public void Wander_ShouldIncludeBlackListInBlockedPoints_WhenPathOptionsProvided()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        pathfinderMock.Setup(p => p.FindRandomDirection(It.IsAny<string>(), It.IsAny<IPoint>(), It.IsAny<IPathOptions>()))
                      .Returns(Direction.Invalid);

        var map = MockMapInstance.Create(setup: m => m.Pathfinder = pathfinderMock.Object);
        var merchant = MockMerchant.Create(map);
        map.AddEntity(merchant, new Point(5, 5));

        merchant.BlackList.Add(new Point(4, 5));
        merchant.BlackList.Add(new Point(6, 5));

        var pathOptions = PathOptions.Default;

        var act = () => merchant.Wander(pathOptions);

        act.Should()
           .NotThrow();
    }
    #endregion
}