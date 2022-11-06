using Chaos.Objects.Panel;

namespace Chaos.Containers.Abstractions;

public interface IInventory : IPanel<Item>
{
    int CountOf(string name);
    bool HasCount(string name, int quantity);
    bool RemoveQuantity(string name, int quantity);
    bool RemoveQuantity(byte slot, int quantity, [MaybeNullWhen(false)] out Item item);
    bool RemoveQuantity(string name, int quantity, [MaybeNullWhen(false)] out ICollection<Item> items);
    bool TryAddDirect(byte slot, Item obj);
}