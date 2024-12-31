#region
using Chaos.DarkAges.Definitions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     Defines the methods and properties for a panel that manages objects
/// </summary>
/// <typeparam name="T">
///     The type of entity the panel manages
/// </typeparam>
public interface IPanel<T> : IEnumerable<T>, IDeltaUpdatable
{
    /// <summary>
    ///     The number of available slots in the panel
    /// </summary>
    int AvailableSlots { get; }

    /// <summary>
    ///     Indicates whether the panel has no available slots
    /// </summary>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when all slots in the panel are occupied, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool IsFull { get; }

    /// <summary>
    ///     The type of the panel
    /// </summary>
    PanelType PanelType { get; }

    /// <summary>
    ///     Adds an observer to the panel
    /// </summary>
    /// <param name="observer">
    ///     The observer to add
    /// </param>
    void AddObserver(Observers.Abstractions.IObserver<T> observer);

    /// <summary>
    ///     Clears all objects from the panel
    /// </summary>
    void Clear();

    /// <summary>
    ///     Determines whether the current panel contains the specified object
    /// </summary>
    /// <param name="obj">
    ///     The object to locate in the panel
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the specified object exists in the panel, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool Contains(T obj);

    /// <summary>
    ///     Determines whether any object exists at the given slot in the panel
    /// </summary>
    /// <param name="slot">
    ///     The slot to check for the presence of an object
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the slot is valid and contains an object, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool Contains(byte slot);

    /// <summary>
    ///     Determines whether the panel contains an item with the specified name
    /// </summary>
    /// <param name="name">
    ///     The name of the item to locate in the panel
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the item with the specified name exists in the panel, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool Contains(string name);

    /// <summary>
    ///     Determines whether the panel contains an object with the specified template key
    /// </summary>
    /// <param name="templateKey">
    ///     The template key to search for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when an object with the specified template key is found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool ContainsByTemplateKey(string templateKey);

    /// <summary>
    ///     Forces the addition of the specified object to the panel, possibly overriding existing entries
    /// </summary>
    /// <param name="obj">
    ///     The object to be forcibly added to the panel
    /// </param>
    void ForceAdd(T obj);

    /// <summary>
    ///     Determines whether the specified slot is valid within the panel
    /// </summary>
    /// <param name="slot">
    ///     The slot to check for validity
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the slot is within the valid range and is not marked as invalid, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool IsValidSlot(byte slot);

    /// <summary>
    ///     Retrieves the object at the specified slot in the panel
    /// </summary>
    T? this[byte slot] { get; }

    /// <summary>
    ///     Retrieves the object with the specified name from the panel
    /// </summary>
    T? this[string name] { get; }

    /// <summary>
    ///     Removes an object from the panel at the specified slot
    /// </summary>
    /// <param name="slot">
    ///     The slot index to remove from
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object is successfully removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool Remove(byte slot);

    /// <summary>
    ///     Removes an item from the panel by its name
    /// </summary>
    /// <param name="name">
    ///     The name of the item to be removed
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the item with the specified name exists in the panel and is successfully removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool Remove(string name);

    /// <summary>
    ///     Removes an object from the panel based on its template key
    /// </summary>
    /// <param name="templateKey">
    ///     The template key of the object to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object with the specified template key is successfully removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool RemoveByTemplateKey(string templateKey);

    /// <summary>
    ///     Attempts to add the specified object to the panel at the given slot
    /// </summary>
    /// <param name="slot">
    ///     The slot where the object should be added
    /// </param>
    /// <param name="obj">
    ///     The object to add to the panel
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object is successfully added to the specified slot, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryAdd(byte slot, T obj);

    /// <summary>
    ///     Attempts to add the specified object to the next available slot in the panel
    /// </summary>
    /// <param name="obj">
    ///     The object to be added to the next available slot
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object is successfully added to the next slot, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryAddToNextSlot(T obj);

    /// <summary>
    ///     Attempts to retrieve an object from the given slot
    /// </summary>
    /// <param name="slot">
    ///     The slot index of the object to retrieve
    /// </param>
    /// <param name="obj">
    ///     The object retrieved if successful, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object is successfully retrieved, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetObject(byte slot, [MaybeNullWhen(false)] out T obj);

    /// <summary>
    ///     Tries to retrieve an object by its name from the panel
    /// </summary>
    /// <param name="name">
    ///     The name of the object to retrieve.
    /// </param>
    /// <param name="obj">
    ///     The object retrieved if successful, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object with the given name is found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetObject(string name, [MaybeNullWhen(false)] out T obj);

    /// Attempts to retrieve an object from the panel based on the specified template key.
    /// <param name="templateKey">
    ///     The template key of the object to retrieve
    /// </param>
    /// <param name="obj">
    ///     The object retrieved if found, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when an object with the specified template key exists in the panel, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetObjectByTemplateKey(string templateKey, [MaybeNullWhen(false)] out T obj);

    /// <summary>
    ///     Attempts to retrieve and remove an object at the specified slot
    /// </summary>
    /// <param name="slot">
    ///     The slot index of the object to retrieve and remove
    /// </param>
    /// <param name="obj">
    ///     The object retrieved if successful, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object is successfully retrieved and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetRemove(byte slot, [MaybeNullWhen(false)] out T obj);

    /// <summary>
    ///     Attempts to retrieve and remove an object by its name.
    /// </summary>
    /// <param name="name">
    ///     The name of the object to retrieve and remove
    /// </param>
    /// <param name="obj">
    ///     The object retrieved if successful, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object is successfully retrieved and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetRemove(string name, [MaybeNullWhen(false)] out T obj);

    /// <summary>
    ///     Attempts to retrieve and remove an object from the panel based on its template key.
    /// </summary>
    /// <param name="templateKey">
    ///     The template key used to locate the object in the panel
    /// </param>
    /// <param name="obj">
    ///     The object retrieved if successful, otherwise
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when an object with the specified template key is found and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetRemoveByTemplateKey(string templateKey, [MaybeNullWhen(false)] out T obj);

    /// <summary>
    ///     Attempts to swap objects between the specified slots
    /// </summary>
    /// <param name="slot1">
    ///     The first slot to swap
    /// </param>
    /// <param name="slot2">
    ///     The second slot to swap
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the objects are successfully swapped, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TrySwap(byte slot1, byte slot2);

    /// <summary>
    ///     Updates the object at the specified slot with an optional action
    /// </summary>
    /// <param name="slot">
    ///     The slot of the object to update
    /// </param>
    /// <param name="action">
    ///     An optional action to invoke on the object at the specified slot
    /// </param>
    /// <remarks>
    ///     This method is used to update an object in the panel. Generally the action should do the updating. Obervers will be
    ///     made aware of the change
    /// </remarks>
    void Update(byte slot, Action<T>? action = null);
}