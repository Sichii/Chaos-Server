using Chaos.Objects.Panel;

namespace Chaos.Objects.Abstractions;

public interface IBuyShopSource : IDialogSourceEntity
{
    ICollection<Item> ItemsForSale { get; }
    int GetStock(string itemTemplateKey);
    bool HasStock(string itemTemplateKey);
    void Restock(decimal percent);

    bool TryDecrementStock(string itemTemplateKey, int amount);

    bool TryGetItem(string itemName, [MaybeNullWhen(false)] out Item item);
}