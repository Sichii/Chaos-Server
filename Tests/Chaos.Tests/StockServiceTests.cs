#region
using Chaos.Services.Other;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class StockServiceTests : IDisposable
{
    private readonly StockService Service;

    public StockServiceTests()
    {
        var logger = MockLogger.Create<StockService>();
        Service = new StockService(logger.Object);
    }

    public void Dispose() => Service.Dispose();

    #region TryDecrementStock — Edge Cases
    [Test]
    public void TryDecrementStock_ShouldNotChangeStock_WhenUnlimited()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", -1)],
            TimeSpan.FromMinutes(30),
            100);

        Service.TryDecrementStock("merchant1", "item1", 999);

        // Unlimited stock should still show unlimited
        Service.HasStock("merchant1", "item1")
               .Should()
               .BeTrue();
    }
    #endregion

    #region GetStock / HasStock
    [Test]
    public void GetStock_ShouldReturnZero_WhenMerchantNotRegistered()
    {
        var result = Service.GetStock("unknown_merchant", "item1");

        result.Should()
              .Be(0);
    }

    [Test]
    public void GetStock_ShouldReturnZero_WhenItemNotInStock()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        var result = Service.GetStock("merchant1", "unknown_item");

        result.Should()
              .Be(0);
    }

    [Test]
    public void GetStock_ShouldReturnMaxStock_AfterRegistration()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        var result = Service.GetStock("merchant1", "item1");

        result.Should()
              .Be(10);
    }

    [Test]
    public void HasStock_ShouldReturnFalse_WhenMerchantNotRegistered()
    {
        var result = Service.HasStock("unknown_merchant", "item1");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void HasStock_ShouldReturnTrue_WhenItemHasStock()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 5)],
            TimeSpan.FromMinutes(30),
            100);

        var result = Service.HasStock("merchant1", "item1");

        result.Should()
              .BeTrue();
    }

    [Test]
    public void HasStock_ShouldReturnTrue_WhenMaxStockIsUnlimited()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", -1)],
            TimeSpan.FromMinutes(30),
            100);

        var result = Service.HasStock("merchant1", "item1");

        result.Should()
              .BeTrue();
    }
    #endregion

    #region TryDecrementStock
    [Test]
    public void TryDecrementStock_ShouldReturnFalse_WhenMerchantNotRegistered()
    {
        var result = Service.TryDecrementStock("unknown", "item1");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryDecrementStock_ShouldReturnFalse_WhenItemNotRegistered()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 5)],
            TimeSpan.FromMinutes(30),
            100);

        var result = Service.TryDecrementStock("merchant1", "unknown_item");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void TryDecrementStock_ShouldDecrementByOne_ByDefault()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 5)],
            TimeSpan.FromMinutes(30),
            100);

        Service.TryDecrementStock("merchant1", "item1");
        var result = Service.GetStock("merchant1", "item1");

        result.Should()
              .Be(4);
    }

    [Test]
    public void TryDecrementStock_ShouldDecrementBySpecifiedAmount()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        Service.TryDecrementStock("merchant1", "item1", 3);
        var result = Service.GetStock("merchant1", "item1");

        result.Should()
              .Be(7);
    }

    [Test]
    public void TryDecrementStock_ShouldReturnFalse_WhenInsufficientStock()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 2)],
            TimeSpan.FromMinutes(30),
            100);

        var result = Service.TryDecrementStock("merchant1", "item1", 5);

        result.Should()
              .BeFalse();

        // Stock should remain unchanged
        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(2);
    }

    [Test]
    public void TryDecrementStock_ShouldReturnTrue_WhenExactAmount()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 3)],
            TimeSpan.FromMinutes(30),
            100);

        var result = Service.TryDecrementStock("merchant1", "item1", 3);

        result.Should()
              .BeTrue();

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(0);
    }

    [Test]
    public void TryDecrementStock_ShouldAlwaysReturnTrue_WhenUnlimitedStock()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", -1)],
            TimeSpan.FromMinutes(30),
            100);

        // Decrement many times, should always succeed
        for (var i = 0; i < 100; i++)
            Service.TryDecrementStock("merchant1", "item1")
                   .Should()
                   .BeTrue();

        // HasStock should still be true
        Service.HasStock("merchant1", "item1")
               .Should()
               .BeTrue();
    }

    [Test]
    public void TryDecrementStock_ShouldMakeHasStockReturnFalse_WhenDepleted()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 1)],
            TimeSpan.FromMinutes(30),
            100);

        Service.TryDecrementStock("merchant1", "item1");

        Service.HasStock("merchant1", "item1")
               .Should()
               .BeFalse();
    }
    #endregion

    #region RegisterStock
    [Test]
    public void RegisterStock_ShouldSupportMultipleItems()
    {
        Service.RegisterStock(
            "merchant1",
            [
                ("item1", 10),
                ("item2", 5),
                ("item3", 20)
            ],
            TimeSpan.FromMinutes(30),
            100);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(10);

        Service.GetStock("merchant1", "item2")
               .Should()
               .Be(5);

        Service.GetStock("merchant1", "item3")
               .Should()
               .Be(20);
    }

    [Test]
    public void RegisterStock_ShouldSupportMultipleMerchants()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        Service.RegisterStock(
            "merchant2",
            [("item1", 20)],
            TimeSpan.FromMinutes(30),
            100);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(10);

        Service.GetStock("merchant2", "item1")
               .Should()
               .Be(20);
    }

    [Test]
    public void RegisterStock_ShouldBeCaseInsensitive_ForMerchantKey()
    {
        Service.RegisterStock(
            "Merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(10);
    }

    [Test]
    public void RegisterStock_ShouldPreserveCurrentStock_WhenReRegistered()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        // Decrement some stock
        Service.TryDecrementStock("merchant1", "item1", 3);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(7);

        // Re-register with same max stock
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        // Current stock should be preserved, not reset to max
        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(7);
    }

    [Test]
    public void RegisterStock_ShouldCapCurrentStock_WhenNewMaxIsLower()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        // Re-register with a lower max stock
        Service.RegisterStock(
            "merchant1",
            [("item1", 5)],
            TimeSpan.FromMinutes(30),
            100);

        // Current stock should be capped to new max
        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(5);
    }

    [Test]
    public void RegisterStock_ReRegister_ShouldAddNewItems_NotInExistingStock()
    {
        // First registration: only item1
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        // Re-register with both item1 and new item2
        Service.RegisterStock(
            "merchant1",
            [
                ("item1", 10),
                ("item2", 5)
            ],
            TimeSpan.FromMinutes(30),
            100);

        // item1 should keep existing stock, item2 should have its max
        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(10);

        Service.GetStock("merchant1", "item2")
               .Should()
               .Be(5);
    }

    [Test]
    public void RegisterStock_ReRegister_ShouldDropItems_NotInNewStock()
    {
        // First registration: item1 and item2
        Service.RegisterStock(
            "merchant1",
            [
                ("item1", 10),
                ("item2", 5)
            ],
            TimeSpan.FromMinutes(30),
            100);

        // Re-register with only item1 (item2 dropped)
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(10);

        // item2 should no longer exist
        Service.HasStock("merchant1", "item2")
               .Should()
               .BeFalse();
    }
    #endregion

    #region Restock
    [Test]
    public void Restock_ShouldDoNothing_WhenMerchantNotRegistered()
        =>

            // Should not throw
            Service.Restock("unknown_merchant", 100);

    [Test]
    public void Restock_ShouldRestoreStockByPercent()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        // Deplete stock
        Service.TryDecrementStock("merchant1", "item1", 10);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(0);

        // Restock 50%
        Service.Restock("merchant1", 50);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(5);
    }

    [Test]
    public void Restock_ShouldNotExceedMaxStock()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        // Deplete only 1
        Service.TryDecrementStock("merchant1", "item1");

        // Restock 100% — should cap at max
        Service.Restock("merchant1", 100);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(10);
    }

    [Test]
    public void Restock_ShouldRestockAllItems()
    {
        Service.RegisterStock(
            "merchant1",
            [
                ("item1", 10),
                ("item2", 20)
            ],
            TimeSpan.FromMinutes(30),
            100);

        Service.TryDecrementStock("merchant1", "item1", 10);
        Service.TryDecrementStock("merchant1", "item2", 20);

        Service.Restock("merchant1", 100);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(10);

        Service.GetStock("merchant1", "item2")
               .Should()
               .Be(20);
    }

    [Test]
    public void Restock_ShouldNotAffectUnlimitedStock()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", -1)],
            TimeSpan.FromMinutes(30),
            100);

        Service.Restock("merchant1", 50);

        Service.HasStock("merchant1", "item1")
               .Should()
               .BeTrue();
    }

    [Test]
    public void Restock_ShouldRestockPartially()
    {
        Service.RegisterStock(
            "merchant1",
            [("item1", 10)],
            TimeSpan.FromMinutes(30),
            100);

        // Fully deplete
        Service.TryDecrementStock("merchant1", "item1", 10);

        // Restock only 30%
        Service.Restock("merchant1", 30);

        Service.GetStock("merchant1", "item1")
               .Should()
               .Be(3);
    }
    #endregion
}