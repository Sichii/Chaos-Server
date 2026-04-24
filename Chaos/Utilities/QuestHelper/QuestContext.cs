#region
using Chaos.Collections;
using Chaos.Common.CustomTypes;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
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
    /// Service provider used to lazy-resolve framework dependencies (IItemFactory, etc.).
    /// The dispatcher (QuestDialogScript) is responsible for passing this through. Tests
    /// can use <c>MockServiceProvider</c> from Chaos.Testing.Infrastructure.
    /// </summary>
    public required IServiceProvider Services { get; init; }

    internal bool OtherwiseTaken { get; set; }
}

/// <summary>
/// Per-execution context passed to predicates and Run callbacks.
/// </summary>
public sealed class QuestContext<TStage> : QuestContext where TStage : struct, Enum
{
    public TStage CurrentStage { get; set; }

    /// <summary>True if Source's Trackers.Enums currently holds the given stage value.</summary>
    public bool IsAt(TStage stage) => Source.Trackers.Enums.HasValue(stage);

    /// <summary>Set the stage in Source's Trackers.Enums and update CurrentStage.</summary>
    public void Advance(TStage stage)
    {
        Source.Trackers.Enums.Set(stage);
        CurrentStage = stage;
    }

    /// <summary>Remove the TStage value from Source's Trackers.Enums.</summary>
    public void ClearStage()
    {
        Source.Trackers.Enums.Remove<TStage>();
        CurrentStage = default;
    }

    // ===== Flag helpers (auto-route to Trackers.Flags vs Trackers.BigFlags by argument type) =====

    /// <summary>Check whether Source's Trackers.Flags contains the given enum flag.</summary>
    public bool HasFlag<TFlag>(TFlag flag) where TFlag : struct, Enum => FlagDispatch.Has(Source, flag);

    /// <summary>Set the given enum flag on Source's Trackers.Flags.</summary>
    public void SetFlag<TFlag>(TFlag flag) where TFlag : struct, Enum => FlagDispatch.Set(Source, flag);

    /// <summary>Clear the given enum flag from Source's Trackers.Flags.</summary>
    public void ClearFlag<TFlag>(TFlag flag) where TFlag : struct, Enum => FlagDispatch.Clear(Source, flag);

    /// <summary>True if every bit in <paramref name="combined" /> is set on Source's Trackers.Flags.</summary>
    public bool HasAllFlags<TFlag>(TFlag combined) where TFlag : struct, Enum => FlagDispatch.HasAll(Source, combined);

    /// <summary>True if any bit in <paramref name="combined" /> is set on Source's Trackers.Flags.</summary>
    public bool HasAnyFlag<TFlag>(TFlag combined) where TFlag : struct, Enum => FlagDispatch.HasAny(Source, combined);

    /// <summary>Check whether Source's Trackers.BigFlags contains the given big flag value.</summary>
    public bool HasFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker : class => FlagDispatch.Has(Source, flag);

    /// <summary>Set the given big flag on Source's Trackers.BigFlags.</summary>
    public void SetFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker : class => FlagDispatch.Set(Source, flag);

    /// <summary>Clear the given big flag from Source's Trackers.BigFlags.</summary>
    public void ClearFlag<TMarker>(BigFlagsValue<TMarker> flag) where TMarker : class => FlagDispatch.Clear(Source, flag);

    /// <summary>True if every bit in <paramref name="combined" /> is set on Source's Trackers.BigFlags.</summary>
    public bool HasAllFlags<TMarker>(BigFlagsValue<TMarker> combined) where TMarker : class
        => FlagDispatch.HasAll(Source, combined);

    /// <summary>True if any bit in <paramref name="combined" /> is set on Source's Trackers.BigFlags.</summary>
    public bool HasAnyFlag<TMarker>(BigFlagsValue<TMarker> combined) where TMarker : class
        => FlagDispatch.HasAny(Source, combined);

    // ===== Counter helpers =====

    /// <summary>True if Source's Trackers.Counters has a counter for <paramref name="key" /> at or above <paramref name="required" />.</summary>
    public bool HasCount(string key, int required) => Source.Trackers.Counters.CounterGreaterThanOrEqualTo(key, required);

    /// <summary>Increment Source's Trackers.Counters for <paramref name="key" /> by <paramref name="by" />.</summary>
    public void IncrementCounter(string key, int by = 1) => Source.Trackers.Counters.AddOrIncrement(key, by);

    /// <summary>Remove the counter at <paramref name="key" /> from Source's Trackers.Counters.</summary>
    public void ClearCounter(string key) => Source.Trackers.Counters.Remove(key, out _);

    // ===== Cooldown helpers =====

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

    /// <summary>
    /// Start an auto-consuming timed event keyed by <paramref name="key" /> on Source's
    /// Trackers.TimedEvents. Re-addable: completed AutoConsume events with the same key are
    /// pre-cleaned by <c>TimedEventCollection.AddEvent</c>.
    /// </summary>
    public void StartCooldown(string key, TimeSpan duration)
        => Source.Trackers.TimedEvents.AddEvent(key, duration, true);

    // ===== Item helpers =====

    private IItemFactory? _itemFactory;
    private ISkillFactory? _skillFactory;
    private ISpellFactory? _spellFactory;

    /// <summary>
    /// Lazy-resolved IItemFactory from the context's <see cref="QuestContext.Services" />.
    /// Resolution is deferred so contexts whose chains never touch items don't pay for
    /// (or require a registration of) IItemFactory.
    /// </summary>
    private IItemFactory ItemFactory => _itemFactory ??= Services.GetRequiredService<IItemFactory>();

    /// <summary>
    /// Lazy-resolved ISkillFactory from the context's <see cref="QuestContext.Services" />.
    /// Resolution is deferred so contexts whose chains never touch skills don't pay for
    /// (or require a registration of) ISkillFactory.
    /// </summary>
    private ISkillFactory SkillFactory => _skillFactory ??= Services.GetRequiredService<ISkillFactory>();

    /// <summary>
    /// Lazy-resolved ISpellFactory from the context's <see cref="QuestContext.Services" />.
    /// Resolution is deferred so contexts whose chains never touch spells don't pay for
    /// (or require a registration of) ISpellFactory.
    /// </summary>
    private ISpellFactory SpellFactory => _spellFactory ??= Services.GetRequiredService<ISpellFactory>();

    /// <summary>
    /// True if Source's Inventory contains at least <paramref name="count" /> of the item
    /// identified by <paramref name="templateKey" />.
    /// </summary>
    public bool HasItem(string templateKey, int count) => Source.Inventory.HasCountByTemplateKey(templateKey, count);

    /// <summary>
    /// Attempt to remove <paramref name="count" /> of the item identified by
    /// <paramref name="templateKey" /> from Source's Inventory. Returns false if the source
    /// does not have enough; otherwise removes them and returns true.
    /// </summary>
    public bool TryConsumeItem(string templateKey, int count)
    {
        if (!HasItem(templateKey, count))
            return false;

        Source.Inventory.RemoveQuantityByTemplateKey(templateKey, count);

        return true;
    }

    /// <summary>
    /// Create a fresh item of <paramref name="templateKey" /> via IItemFactory, set its
    /// Count if greater than 1, and hand it to Source via GiveItemOrSendToBank.
    /// </summary>
    public void GiveItem(string templateKey, int count = 1)
    {
        var item = ItemFactory.Create(templateKey);

        if (count > 1)
            item.Count = count;

        Source.GiveItemOrSendToBank(item);
    }

    // ===== Reward helpers =====

    /// <summary>
    /// Award <paramref name="amount" /> experience to Source via the active
    /// experience distribution script (resolved through <c>FunctionalScriptRegistry</c>).
    /// </summary>
    public void GiveExperience(long amount) => DefaultExperienceDistributionScript.Create().GiveExp(Source, amount);

    /// <summary>
    /// Add <paramref name="amount" /> gold to Source via <c>TryGiveGold</c>. The chain continues
    /// even if it fails (e.g. would exceed MaxGoldHeld); use a guard if you need to halt on failure.
    /// </summary>
    public void GiveGold(int amount) => Source.TryGiveGold(amount);

    /// <summary>
    /// Add a new <see cref="LegendMark" /> with <see cref="GameTime.Now" /> to Source's Legend, or
    /// accumulate the count if a mark with the same key already exists.
    /// </summary>
    public void GiveLegendMark(string text, string key, MarkIcon icon, MarkColor color)
        => Source.Legend.AddOrAccumulate(new LegendMark(text, key, icon, color, 1, GameTime.Now));

    /// <summary>
    /// Resolve <see cref="ISkillFactory" /> from <see cref="QuestContext.Services" />, create a skill
    /// of <paramref name="templateKey" />, and learn it via <c>ComplexActionHelper.LearnSkill</c>
    /// (which checks SkillBook capacity).
    /// </summary>
    public void GiveSkill(string templateKey)
    {
        var skill = SkillFactory.Create(templateKey);
        ComplexActionHelper.LearnSkill(Source, skill);
    }

    /// <summary>
    /// Resolve <see cref="ISpellFactory" /> from <see cref="QuestContext.Services" />, create a spell
    /// of <paramref name="templateKey" />, and learn it via <c>ComplexActionHelper.LearnSpell</c>
    /// (which checks SpellBook capacity). If <paramref name="page" /> is provided and the spell was
    /// learned, swap it into that slot. If the spell wasn't learned (e.g., capacity full), the
    /// page placement is silently skipped — quest rewards never gate on placement success.
    /// </summary>
    public void GiveSpell(string templateKey, byte? page = null)
    {
        var spell = SpellFactory.Create(templateKey);
        ComplexActionHelper.LearnSpell(Source, spell);

        if (page.HasValue && Source.SpellBook.TryGetObjectByTemplateKey(templateKey, out var learned))
            Source.SpellBook.TrySwap(learned.Slot, page.Value);
    }

    /// <summary>Remove the skill matching <paramref name="templateKey" /> from Source's SkillBook.</summary>
    public void RemoveSkill(string templateKey) => Source.SkillBook.RemoveByTemplateKey(templateKey);

    /// <summary>Remove the spell matching <paramref name="templateKey" /> from Source's SpellBook.</summary>
    public void RemoveSpell(string templateKey) => Source.SpellBook.RemoveByTemplateKey(templateKey);

    // ===== Teleport helpers =====

    /// <summary>
    /// Resolve the destination map via <see cref="ISimpleCache{MapInstance}" /> and traverse
    /// Source to <paramref name="point" /> on it.
    /// </summary>
    public void Teleport(string mapKey, IPoint point)
    {
        var cache = Services.GetRequiredService<ISimpleCache<MapInstance>>();
        var map = cache.Get(mapKey);
        Source.TraverseMap(map, point);
    }

    /// <summary>
    /// Teleport every member of Source's group to <paramref name="point" /> on the resolved map.
    /// If Source has no group, only Source is teleported.
    /// </summary>
    public void GroupTeleport(string mapKey, IPoint point)
    {
        var cache = Services.GetRequiredService<ISimpleCache<MapInstance>>();
        var map = cache.Get(mapKey);

        if (Source.Group is null)
        {
            Source.TraverseMap(map, point);

            return;
        }

        foreach (var member in Source.Group)
            member.TraverseMap(map, point);
    }
}
