using Chaos.Models.Panel;

namespace Chaos.Models.Abstractions;

public interface IBuyShopSource : IDialogSourceEntity
{
    ICollection<Item> ItemsForSale { get; }
    int GetStock(string itemTemplateKey);
    bool HasStock(string itemTemplateKey);
    void Restock(int percent);

    bool TryDecrementStock(string itemTemplateKey, int amount);

    bool TryGetItem(string itemName, [MaybeNullWhen(false)] out Item item);
}