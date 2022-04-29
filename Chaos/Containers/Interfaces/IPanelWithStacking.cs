using System.Diagnostics.CodeAnalysis;
using Chaos.PanelObjects;

namespace Chaos.Containers.Interfaces;

public interface IPanelWithStacking<T> : IPanel<T>
{
    bool RemoveQuantity(string name, int quantity);
    bool RemoveQuantity(byte slot, int quantity, [MaybeNullWhen(false)] out Item item);
    bool TryAddDirect(byte slot, Item obj);
}