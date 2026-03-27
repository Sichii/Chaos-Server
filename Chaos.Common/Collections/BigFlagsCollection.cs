#region
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Chaos.Common.Abstractions;
using Chaos.Common.Converters;
using Chaos.Common.CustomTypes;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Common;

/// <summary>
///     A serializable collection of BigFlagsValue flags, keyed by their marker type
/// </summary>
[JsonConverter(typeof(BigFlagsCollectionJsonConverter))]
public sealed class BigFlagsCollection : IEnumerable<KeyValuePair<Type, IBigFlagsValue>>
{
    private readonly ConcurrentDictionary<Type, IBigFlagsValue> Flags;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BigFlagsCollection" /> class
    /// </summary>
    public BigFlagsCollection() => Flags = new ConcurrentDictionary<Type, IBigFlagsValue>();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Type, IBigFlagsValue>> GetEnumerator() => Flags.GetEnumerator();

    /// <summary>
    ///     Adds a flag to an existing flag of the same marker type, or sets it
    /// </summary>
    /// <param name="flag">
    ///     The flag to add or set
    /// </param>
    /// <typeparam name="TMarker">
    ///     The marker type of the flag
    /// </typeparam>
    public void AddFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker: class => AddFlag(typeof(TMarker), flag);

    /// <summary>
    ///     Adds a flag to an existing flag of the same marker type, or sets it
    /// </summary>
    /// <param name="markerType">
    ///     The marker type of the flag
    /// </param>
    /// <param name="flagValue">
    ///     The flag to add or set
    /// </param>
    public void AddFlag(Type markerType, IBigFlagsValue flagValue)
    {
        if (Flags.TryGetValue(markerType, out var existingValue))
        {
            // Combine the existing value with the new value using OR
            var combined = existingValue.Value | flagValue.Value;
            var newValue = BigFlags.Create(markerType, combined);
            Flags[markerType] = newValue;
        } else
            Flags.TryAdd(markerType, flagValue);
    }

    /// <summary>
    ///     Clears all flags from the collection
    /// </summary>
    public void Clear() => Flags.Clear();

    /// <summary>
    ///     Gets the flag of the specified marker type
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type of the flag to retrieve
    /// </typeparam>
    /// <returns>
    ///     The BigFlagsValue for the specified marker type
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    ///     Flag type not found
    /// </exception>
    public BigFlagsValue<TMarker> GetFlag<TMarker>() where TMarker: class
    {
        var markerType = typeof(TMarker);

        if (Flags.TryGetValue(markerType, out var value))
            return (BigFlagsValue<TMarker>)value;

        throw new KeyNotFoundException($"BigFlagsValue with marker type {markerType.FullName} was not found in the collection");
    }

    /// <summary>
    ///     Determines if the flag collection contains the specified flag
    /// </summary>
    /// <param name="flag">
    ///     The flag value to check for
    /// </param>
    /// <typeparam name="TMarker">
    ///     The marker type of the flag
    /// </typeparam>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a flag of the given marker type was found and contains the specified value, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool HasFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker: class
    {
        var markerType = typeof(TMarker);

        return HasFlag(markerType, flag);
    }

    /// <summary>
    ///     Determines if the flag collection contains the specified flag
    /// </summary>
    /// <param name="markerType">
    ///     The marker type of the flag
    /// </param>
    /// <param name="value">
    ///     The flag value to check for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a flag of the given marker type was found and contains the specified value, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool HasFlag(Type markerType, IBigFlagsValue value)
    {
        if (Flags.TryGetValue(markerType, out var existingValue))
        {
            var flagValue = existingValue.Value;
            var valueToCheck = value.Value;

            return (flagValue & valueToCheck) == valueToCheck;
        }

        return false;
    }

    /// <summary>
    ///     Removes all flags of the specified marker type from the collection
    /// </summary>
    /// <typeparam name="TMarker">
    ///     The marker type to remove
    /// </typeparam>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the marker type was found and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool Remove<TMarker>() where TMarker: class => Flags.TryRemove(typeof(TMarker), out _);

    /// <summary>
    ///     Removes the value of the flag of the specified marker type from the collection
    /// </summary>
    /// <param name="flag">
    ///     The value of the flag to remove
    /// </param>
    /// <typeparam name="TMarker">
    ///     The marker type of the flag
    /// </typeparam>
    public void RemoveFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker: class => RemoveFlag(typeof(TMarker), flag);

    /// <summary>
    ///     Removes the value of the flag of the specified marker type from the collection
    /// </summary>
    /// <param name="markerType">
    ///     The marker type of the flag
    /// </param>
    /// <param name="flagValue">
    ///     The value of the flag to remove
    /// </param>
    public void RemoveFlag(Type markerType, IBigFlagsValue flagValue)
    {
        if (Flags.TryGetValue(markerType, out var existingValue))
        {
            var newValue = existingValue.Value & ~flagValue.Value;
            var updated = BigFlags.Create(markerType, newValue);
            Flags[markerType] = updated;
        }
    }

    /// <summary>
    ///     Sets the flag value for the specified marker type, replacing any existing value
    /// </summary>
    /// <param name="flag">
    ///     The flag value to set
    /// </param>
    /// <typeparam name="TMarker">
    ///     The marker type of the flag
    /// </typeparam>
    public void SetFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker: class => Flags[typeof(TMarker)] = flag;

    /// <summary>
    ///     Attempts to retrieve the value of the flag of the specified marker type
    /// </summary>
    /// <param name="value">
    ///     The value of the flag retrieved
    /// </param>
    /// <typeparam name="TMarker">
    ///     The marker type of the flag
    /// </typeparam>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a flag with the specified marker type was found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool TryGetFlag<TMarker>([MaybeNullWhen(false)] out BigFlagsValue<TMarker> value) where TMarker: class
    {
        value = default;
        var markerType = typeof(TMarker);

        if (!Flags.TryGetValue(markerType, out var flagValue))
            return false;

        value = (BigFlagsValue<TMarker>)flagValue;

        return true;
    }

    /// <summary>
    ///     Attempts to retrieve the value of the flag of the specified marker type
    /// </summary>
    /// <param name="markerType">
    ///     The marker type of the flag
    /// </param>
    /// <param name="flagValue">
    ///     The value of the flag retrieved
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a flag with the specified marker type was found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool TryGetFlag(Type markerType, [MaybeNullWhen(false)] out IBigFlagsValue flagValue)
        => Flags.TryGetValue(markerType, out flagValue);
}