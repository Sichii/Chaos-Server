#region
using Chaos.Collections;
using Chaos.Common.CustomTypes;
using Chaos.Models.Menu;
using Chaos.Models.World;
#endregion

namespace Chaos.Utilities.QuestHelper;

/// <summary>
/// Non-generic base for QuestContext so framework collections (handlers, operations)
/// can hold contexts of mixed quest types.
/// </summary>
public abstract class QuestContext
{
    public required Aisling Source { get; init; }
    public Dialog? Subject { get; init; }

    /// <summary>
    /// 1-based index of the dialog option the player selected when advancing the dialog
    /// (e.g. menu choice or button option). Set by the dispatcher only when the lifecycle phase
    /// is <see cref="DialogPhase.Next" />; null in all other phases. Always null when the player
    /// advances a non-menu dialog (Normal, TextEntry, etc.) — Dialog.Next normalizes those to
    /// null before invoking the script.
    /// </summary>
    public byte? OptionIndex { get; init; }

    /// <summary>
    /// Service provider used to resolve framework dependencies for fluent builder operations
    /// (IItemFactory, ISkillFactory, ISpellFactory, ISimpleCache&lt;MapInstance&gt;). The dispatcher
    /// (QuestDialogScript) is responsible for passing this through. Tests can use
    /// <c>MockServiceProvider</c> from Chaos.Testing.Infrastructure.
    /// </summary>
    public required IServiceProvider Services { get; init; }

    internal bool OtherwiseTaken { get; set; }

    // ===== Sub-stage predicates =====
    //
    // A "sub-stage" is any enum stored in Source.Trackers.Enums under a type parameter other
    // than the quest's primary TStage. Quests can layer multiple parallel sub-stages — each
    // with its own enum type — to model orthogonal sub-state machines (e.g. a remake-bouquet
    // sub-track that runs alongside the main quest progression). The framework imposes no
    // relationship between primary and sub-stages; quest authors compose them via Branch /
    // WhenAtSub / RouteBySub as needed.
    //
    // Mutations (AdvanceSub / ClearSub) are exposed only through the fluent builder. To set
    // a sub-stage from a Run callback or test, write Source.Trackers.Enums.Set(value) directly.

    /// <summary>True if Source's Trackers.Enums currently holds <paramref name="value" /> for type <typeparamref name="TSub" />.</summary>
    public bool WhenAtSub<TSub>(TSub value) where TSub : struct, Enum => Source.Trackers.Enums.HasValue(value);

    /// <summary>True if Source's tracked TSub matches any of <paramref name="values" />. Returns false on empty input.</summary>
    public bool WhenAtAnySub<TSub>(params IReadOnlyCollection<TSub> values) where TSub : struct, Enum
    {
        foreach (var value in values)
            if (WhenAtSub(value))
                return true;

        return false;
    }

    /// <summary>
    /// True if Source's Trackers.Enums has no value stored for <typeparamref name="TSub" /> —
    /// i.e., this sub-track has never been advanced. Mirrors <c>NeverStarted</c> on the
    /// primary stage.
    /// </summary>
    public bool HasNoSub<TSub>() where TSub : struct, Enum => !Source.Trackers.Enums.TryGetValue<TSub>(out _);

    /// <summary>
    /// Attempt to read the current sub-stage value of type <typeparamref name="TSub" />. Returns
    /// false (with <paramref name="value" /> set to <c>default</c>) if the sub-track has never
    /// been advanced.
    /// </summary>
    public bool TryGetSub<TSub>(out TSub value) where TSub : struct, Enum
    {
        if (Source.Trackers.Enums.TryGetValue<TSub>(out var stored))
        {
            value = stored;

            return true;
        }

        value = default;

        return false;
    }
}

/// <summary>
/// Per-execution context passed to predicates and Run callbacks. Exposes read-only state
/// (Source, Subject, current stage) and predicates that mirror the fluent builder's guards.
/// All mutations live on <see cref="QuestStepBuilder{TStage}" /> — Run callbacks that need
/// state changes should add a fluent builder method or write through <c>Source</c> directly.
/// </summary>
public sealed class QuestContext<TStage> : QuestContext where TStage : struct, Enum
{
    public TStage CurrentStage { get; set; }

    /// <summary>True if Source's Trackers.Enums currently holds the given stage value.</summary>
    public bool WhenAt(TStage stage) => Source.Trackers.Enums.HasValue(stage);

    /// <summary>True if Source's tracked TStage matches any of <paramref name="stages" />. Returns false on empty input.</summary>
    public bool WhenAtAny(params IReadOnlyCollection<TStage> stages)
    {
        foreach (var stage in stages)
            if (WhenAt(stage))
                return true;

        return false;
    }

    /// <summary>
    /// True if Source's Trackers.Enums has no value stored for <typeparamref name="TStage" />.
    /// Distinct from <c>CurrentStage == default</c>, which is also true when <c>default(TStage)</c>
    /// is the explicitly stored value.
    /// </summary>
    public bool NeverStarted => !Source.Trackers.Enums.TryGetValue<TStage>(out _);

    // ===== Flag predicates (auto-route to Trackers.Flags vs Trackers.BigFlags by argument type) =====

    /// <summary>Check whether Source's Trackers.Flags contains the given enum flag.</summary>
    public bool HasFlag<TFlag>(TFlag flag) where TFlag : struct, Enum => FlagDispatch.Has(Source, flag);

    /// <summary>True if every bit in <paramref name="combined" /> is set on Source's Trackers.Flags.</summary>
    public bool HasAllFlags<TFlag>(TFlag combined) where TFlag : struct, Enum => FlagDispatch.HasAll(Source, combined);

    /// <summary>True if any bit in <paramref name="combined" /> is set on Source's Trackers.Flags.</summary>
    public bool HasAnyFlag<TFlag>(TFlag combined) where TFlag : struct, Enum => FlagDispatch.HasAny(Source, combined);

    /// <summary>Check whether Source's Trackers.BigFlags contains the given big flag value.</summary>
    public bool HasFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker : class => FlagDispatch.Has(Source, flag);

    /// <summary>True if every bit in <paramref name="combined" /> is set on Source's Trackers.BigFlags.</summary>
    public bool HasAllFlags<TMarker>(BigFlagsValue<TMarker> combined) where TMarker : class
        => FlagDispatch.HasAll(Source, combined);

    /// <summary>True if any bit in <paramref name="combined" /> is set on Source's Trackers.BigFlags.</summary>
    public bool HasAnyFlag<TMarker>(BigFlagsValue<TMarker> combined) where TMarker : class
        => FlagDispatch.HasAny(Source, combined);

    // ===== Counter predicates =====

    /// <summary>True if Source's counter for <paramref name="key" /> is at or above <paramref name="value" />. A missing counter is treated as 0.</summary>
    public bool CounterGreaterThanOrEqualTo(string key, int value) => Source.Trackers.Counters.CounterGreaterThanOrEqualTo(key, value);

    /// <summary>True if Source's counter for <paramref name="key" /> is at or below <paramref name="value" />. A missing counter is treated as 0.</summary>
    public bool CounterLessThanOrEqualTo(string key, int value)
    {
        Source.Trackers.Counters.TryGetValue(key, out var current);

        return current <= value;
    }

    /// <summary>True if Source's counter for <paramref name="key" /> equals <paramref name="value" />. A missing counter is treated as 0.</summary>
    public bool CounterEqualTo(string key, int value)
    {
        Source.Trackers.Counters.TryGetValue(key, out var current);

        return current == value;
    }

    /// <summary>True if Source's counter for <paramref name="key" /> is strictly less than <paramref name="value" />. A missing counter is treated as 0.</summary>
    public bool CounterLessThan(string key, int value)
    {
        Source.Trackers.Counters.TryGetValue(key, out var current);

        return current < value;
    }

    /// <summary>True if Source's counter for <paramref name="key" /> is strictly greater than <paramref name="value" />. A missing counter is treated as 0.</summary>
    public bool CounterGreaterThan(string key, int value)
    {
        Source.Trackers.Counters.TryGetValue(key, out var current);

        return current > value;
    }

    // ===== Cooldown predicate =====

    /// <summary>
    /// True if Source's Trackers.TimedEvents has an active (non-completed) event keyed by
    /// <paramref name="key" />. When true, <paramref name="remaining" /> is set to the event's
    /// remaining time; otherwise it is <see cref="TimeSpan.Zero" />.
    /// </summary>
    public bool HasActiveCooldown(string key, out TimeSpan remaining)
    {
        if (Source.Trackers.TimedEvents.HasActiveEvent(key, out var evt))
        {
            remaining = evt.Remaining;

            return true;
        }

        remaining = TimeSpan.Zero;

        return false;
    }

    // ===== Item / Equipment predicates =====

    /// <summary>
    /// True if Source's Inventory contains at least <paramref name="count" /> of the item
    /// identified by <paramref name="templateKey" />.
    /// </summary>
    public bool HasItemByTemplateKey(string templateKey, int count) => Source.Inventory.HasCountByTemplateKey(templateKey, count);

    /// <summary>
    /// True if Source's Inventory has at least <paramref name="count" /> of <paramref name="templateKey" />,
    /// or Source's Equipment contains at least one item with that template key. Use for deliverables
    /// that can be turned in either still in the bag or already worn (e.g. a quest bouquet that the
    /// player equipped before reaching the turn-in NPC).
    /// </summary>
    /// <remarks>
    /// Equipment slots are non-stacking, so the equipment side only contributes 1 toward the requirement.
    /// For <paramref name="count" /> &gt; 1, the inventory side must satisfy the full count on its own.
    /// </remarks>
    public bool HasItemOrEquippedByTemplateKey(string templateKey, int count)
        => HasItemByTemplateKey(templateKey, count) || ((count == 1) && Source.Equipment.ContainsByTemplateKey(templateKey));

    /// <summary>
    /// True if Source's Inventory holds at least <paramref name="count" /> of items whose
    /// display name equals <paramref name="name" /> (case-insensitive).
    /// Use <see cref="HasItemByTemplateKey" /> when the templateKey is the stable identifier.
    /// </summary>
    public bool HasItem(string name, int count) => Source.Inventory.HasCount(name, count);

    /// <summary>
    /// True if Source's Inventory holds at least <paramref name="count" /> of items whose display name equals
    /// <paramref name="name" /> (case-insensitive), or — when <paramref name="count" /> is 1 — an item with that
    /// name is equipped. Use <see cref="HasItemOrEquippedByTemplateKey" /> when the templateKey is the stable identifier.
    /// </summary>
    public bool HasItemOrEquipped(string name, int count)
        => HasItem(name, count) || ((count == 1) && Source.Equipment.Contains(name));

    /// <summary>
    /// True if Source's Equipment currently contains an item identified by
    /// <paramref name="templateKey" />. Use for outfit, insignia, or "must be wearing this" checks.
    /// </summary>
    /// <remarks>
    /// Equipment slots are non-stacking, so this is a binary "is it worn?" check.
    /// For the "in bag or worn" semantics, use <see cref="HasItemOrEquippedByTemplateKey" />.
    /// </remarks>
    public bool HasEquippedByTemplateKey(string templateKey) => Source.Equipment.ContainsByTemplateKey(templateKey);

    /// <summary>
    /// True if Source's Equipment currently contains an item whose display name equals
    /// <paramref name="name" /> (case-insensitive). Use <see cref="HasEquippedByTemplateKey" />
    /// when the templateKey is the stable identifier.
    /// </summary>
    /// <remarks>
    /// Equipment slots are non-stacking, so this is a binary "is it worn?" check.
    /// </remarks>
    public bool HasEquipped(string name) => Source.Equipment.Contains(name);

    // ===== Gold predicate =====

    /// <summary>
    /// True if Source's <c>Gold</c> is at least <paramref name="amount" />. Pure read — does not
    /// emit any client message.
    /// </summary>
    public bool HasGold(int amount) => Source.Gold >= amount;
}
