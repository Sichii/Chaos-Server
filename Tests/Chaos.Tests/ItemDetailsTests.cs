#region
using Chaos.Models.Data;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class ItemDetailsTests
{
    #region BuyWithGold
    [Test]
    public void BuyWithGold_ShouldSetPriceToBuyCost()
    {
        // MockItem template has BuyCost = 0 by default
        var item = MockItem.Create("Sword");

        var details = ItemDetails.BuyWithGold(item);

        details.Item
               .Should()
               .BeSameAs(item);

        details.Price
               .Should()
               .Be(item.Template.BuyCost);
    }
    #endregion

    #region WithdrawItem
    [Test]
    public void WithdrawItem_ShouldSetPriceToCount()
    {
        var item = MockItem.Create("Potion", 10, true);

        var details = ItemDetails.WithdrawItem(item);

        details.Item
               .Should()
               .BeSameAs(item);

        details.Price
               .Should()
               .Be(10);
    }
    #endregion
}