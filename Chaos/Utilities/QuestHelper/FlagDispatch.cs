#region
using Chaos.Common.CustomTypes;
using Chaos.Models.World;
#endregion

namespace Chaos.Utilities.QuestHelper;

/// <summary>
///     Internal dispatch for flag operations. The codebase distinguishes regular flag enums
///     (handled by <c>Trackers.Flags</c> / <see cref="Chaos.Collections.Common.FlagCollection" />)
///     from "big" flags (handled by <c>Trackers.BigFlags</c> /
///     <see cref="Chaos.Collections.Common.BigFlagsCollection" />) by type, not by attribute:
///     regular flags are <see cref="Enum" />s with <c>[Flags]</c>, big flags are
///     <see cref="BigFlagsValue{TMarker}" /> structs whose marker class inherits
///     <see cref="BigFlags{TMarker}" />.
///     Each operation therefore exposes two strongly-typed overloads — one per family — and the
///     compiler picks the right one based on the argument's type.
/// </summary>
internal static class FlagDispatch
{
    // ===== Regular flag enums → Trackers.Flags =====

    public static void Set<T>(Aisling source, T value) where T : struct, Enum
        => source.Trackers.Flags.AddFlag(value);

    public static void Clear<T>(Aisling source, T value) where T : struct, Enum
        => source.Trackers.Flags.RemoveFlag(value);

    public static bool Has<T>(Aisling source, T value) where T : struct, Enum
        => source.Trackers.Flags.HasFlag(value);

    /// <summary>
    ///     FlagCollection.HasFlag already treats combined values as "all bits required" — it ANDs the
    ///     stored value with the query and checks equality with the query. So HasAll is just Has.
    /// </summary>
    public static bool HasAll<T>(Aisling source, T value) where T : struct, Enum
        => source.Trackers.Flags.HasFlag(value);

    /// <summary>
    ///     Iterate the set bits in <paramref name="value" /> and return true if any single bit is
    ///     present in the stored flags.
    /// </summary>
    public static bool HasAny<T>(Aisling source, T value) where T : struct, Enum
    {
        var raw = Convert.ToUInt64(value);

        if (raw == 0)
            return source.Trackers.Flags.HasFlag(value);

        for (var bit = 0; bit < 64; bit++)
        {
            var mask = 1UL << bit;

            if ((raw & mask) == 0)
                continue;

            var single = (T)Enum.ToObject(typeof(T), mask);

            if (source.Trackers.Flags.HasFlag(single))
                return true;
        }

        return false;
    }

    // ===== Big flags → Trackers.BigFlags =====

    public static void Set<TMarker>(Aisling source, BigFlagsValue<TMarker> value) where TMarker : class
        => source.Trackers.BigFlags.AddFlag(value);

    public static void Clear<TMarker>(Aisling source, BigFlagsValue<TMarker> value) where TMarker : class
        => source.Trackers.BigFlags.RemoveFlag(value);

    public static bool Has<TMarker>(Aisling source, BigFlagsValue<TMarker> value) where TMarker : class
        => source.Trackers.BigFlags.HasFlag(value);

    /// <summary>
    ///     BigFlagsCollection.HasFlag ANDs the stored bits with the query and checks equality with
    ///     the query — i.e., already "all bits required" semantics. So HasAll is just Has.
    /// </summary>
    public static bool HasAll<TMarker>(Aisling source, BigFlagsValue<TMarker> value) where TMarker : class
        => source.Trackers.BigFlags.HasFlag(value);

    /// <summary>
    ///     Iterate the set bit indices in <paramref name="value" /> and return true if any single
    ///     bit is present in the stored big flags.
    /// </summary>
    public static bool HasAny<TMarker>(Aisling source, BigFlagsValue<TMarker> value) where TMarker : class
    {
        var anyChecked = false;

        foreach (var bitIndex in value.GetSetBitIndices())
        {
            anyChecked = true;
            var single = BigFlags.Create<TMarker>(bitIndex);

            if (source.Trackers.BigFlags.HasFlag(single))
                return true;
        }

        // If the query had no bits set (e.g., None), fall through to the standard HasFlag check
        // so callers asking "do you have None?" get a sensible answer.
        return !anyChecked && source.Trackers.BigFlags.HasFlag(value);
    }
}
