#region
using Chaos.Models.Panel;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     Provides methods for managing inventory items
/// </summary>
public interface IInventory : IPanel<Item>
{
    /// <summary>
    ///     Gets the total count of items with the specified name in the inventory
    /// </summary>
    /// <param name="name">
    ///     The name of the item to count.
    /// </param>
    int CountOf(string name);

    /// <summary>
    ///     Gets the total count of items in the inventory that match the specified template key
    /// </summary>
    /// <param name="templateKey">
    ///     The template key used to filter items in the inventory
    /// </param>
    int CountOfByTemplateKey(string templateKey);

    /// <summary>
    ///     Determines whether the inventory contains the specified quantity of items with the given name
    /// </summary>
    /// <param name="name">
    ///     The name of the item to check
    /// </param>
    /// <param name="quantity">
    ///     The required quantity of the item
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the inventory contains the required quantity of items, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool HasCount(string name, int quantity);

    /// <summary>
    ///     Determines if the inventory contains at least the specified quantity of items associated with the given template
    ///     key
    /// </summary>
    /// <param name="templateKey">
    ///     The template key of an item
    /// </param>
    /// <param name="quantity">
    ///     The required quantity of items
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the inventory contains the specified quantity of items associated with the template key, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool HasCountByTemplateKey(string templateKey, int quantity);

    /// <summary>
    ///     Removes a specified quantity of items from a given inventory slot
    /// </summary>
    /// <param name="slot">
    ///     The slot index from which the items should be removed
    /// </param>
    /// <param name="quantity">
    ///     The number of items to remove
    /// </param>
    /// <param name="items">
    ///     When the operation is successful, the list of removed items
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the operation is successful and items are removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool RemoveQuantity(byte slot, int quantity, [MaybeNullWhen(false)] out List<Item> items);

    /// <summary>
    ///     Removes the specified quantity of items with the given name from the inventory
    /// </summary>
    /// <param name="name">
    ///     The name of the item to be removed
    /// </param>
    /// <param name="quantity">
    ///     The number of items to remove
    /// </param>
    /// <param name="items">
    ///     The list that will hold the removed items, if successful
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the specified quantity of the named item is successfully removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool RemoveQuantity(string name, int quantity, [MaybeNullWhen(false)] out List<Item> items);

    /// <summary>
    ///     Removes a specified quantity of items from a given slot in the inventory
    /// </summary>
    /// <param name="slot">
    ///     The slot from which items are to be removed
    /// </param>
    /// <param name="quantity">
    ///     The quantity of items to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the specified quantity is successfully removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool RemoveQuantity(byte slot, int quantity);

    /// <summary>
    ///     Removes a specified quantity of items with the given name from the inventory
    /// </summary>
    /// <param name="name">
    ///     The name of the item to remove
    /// </param>
    /// <param name="quantity">
    ///     The quantity of the item to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the specified quantity of items is successfully removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool RemoveQuantity(string name, int quantity);

    /// <summary>
    ///     Removes a specified quantity of items from the inventory that match the given template key
    /// </summary>
    /// <param name="templateKey">
    ///     The template key of the items to remove
    /// </param>
    /// <param name="quantity">
    ///     The quantity of items to remove
    /// </param>
    /// <param name="items">
    ///     The list of items removed from the inventory if successful, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the specified quantity of items matching the template key were successfully removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool RemoveQuantityByTemplateKey(string templateKey, int quantity, [MaybeNullWhen(false)] out List<Item> items);

    /// <summary>
    ///     Removes a specified quantity of items identified by their template key
    /// </summary>
    /// <param name="templateKey">
    ///     The template key of the items to remove
    /// </param>
    /// <param name="quantity">
    ///     The quantity of items to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the specified quantity of items identified by the template key is successfully removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool RemoveQuantityByTemplateKey(string templateKey, int quantity);

    /// <summary>
    ///     Tries to add an item to a specific slot in the inventory.
    /// </summary>
    /// <param name="slot">
    ///     The index of the slot where the item should be added
    /// </param>
    /// <param name="obj">
    ///     The item to be added to the inventory
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the item is successfully added to the slot, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryAddDirect(byte slot, Item obj);
}