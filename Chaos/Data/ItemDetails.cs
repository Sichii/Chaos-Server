using Chaos.Objects.Panel;

namespace Chaos.Data;

public sealed class ItemDetails
{
    public required int AmountOrPrice { get; init; }
    public required Item Item { get; init; }

    public static ItemDetails Default(Item item) => new()
    {
        Item = item,
        AmountOrPrice = item.Template.BuyCost
    };
}