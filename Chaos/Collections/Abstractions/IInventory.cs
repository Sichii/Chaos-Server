using Chaos.Models.Panel;

namespace Chaos.Collections.Abstractions;

public interface IInventory : IPanel<Item>
{
    int CountOf(string name);
    int CountOfByTemplateKey(string templateKey);
    bool HasCount(string name, int quantity);
    bool HasCountByTemplateKey(string templateKey, int quantity);
    bool RemoveQuantity(byte slot, int quantity, [MaybeNullWhen(false)] out List<Item> items);
    bool RemoveQuantity(string name, int quantity, [MaybeNullWhen(false)] out List<Item> items);
    bool RemoveQuantity(byte slot, int quantity);
    bool RemoveQuantity(string name, int quantity);
    bool RemoveQuantityByTemplateKey(string templateKey, int quantity, [MaybeNullWhen(false)] out List<Item> items);
    bool RemoveQuantityByTemplateKey(string templateKey, int quantity);
    bool TryAddDirect(byte slot, Item obj);
}