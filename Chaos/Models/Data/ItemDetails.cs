using Chaos.Models.Panel;

namespace Chaos.Models.Data;

public sealed class ItemDetails
{
    public required Item Item { get; init; }
    public required int Price { get; init; }

    public static ItemDetails BuyWithGold(Item item)
        => new()
        {
            Item = item,
            Price = item.Template.BuyCost
        };

    public static ItemDetails WithdrawItem(Item item)
        => new()
        {
            Item = item,
            Price = item.Count
        };
}